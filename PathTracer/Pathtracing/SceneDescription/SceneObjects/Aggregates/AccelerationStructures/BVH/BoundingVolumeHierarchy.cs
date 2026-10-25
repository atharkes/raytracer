using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;

/// <summary> A Bounding Volume Hierarchy tree used as acceleration structure for ray intersections in world space
/// Possible Additions:
/// - Refitting (enable animation/movement, adding and removing primitives)
/// - Top-level BHV's for static and non-static parts
/// </summary>
public class BoundingVolumeHierarchy : AccelerationStructure, IBinaryTree {
    /// <summary> The estimated cost of traversing the BVH for the SAH </summary>
    public const float TraversalCost = 1f;
    /// <summary> The estimated cost of intersecting a primitive for the SAH </summary>
    public const float IntersectionCost = 1f;
    /// <summary> Whether to bin or just find the optimal greedy BVH </summary>
    public const bool Bin = true;
    /// <summary> The amount of bins used for the binning process </summary>
    public const int BinAmount = 16;
    /// <summary> An epsilon for the binning process </summary>
    public const float BinningEpsilon = 0.99999f;

    /// <summary> The cost of intersecting <paramref name="itemCount"/> items in a box of <paramref name="surfaceArea"/>, weighted by that area </summary>
    /// <param name="itemCount">The amount of items</param>
    /// <param name="surfaceArea">The surface area of the box around the items</param>
    /// <returns>The area weighted intersection cost</returns>
    public static float SurfaceAreaHeuristic(int itemCount, float surfaceArea) => IntersectionCost * itemCount * surfaceArea;

    /// <summary> The area weighted cost of keeping this node a leaf </summary>
    protected float LeafCost => SurfaceAreaHeuristic(Items.Count, Shape.BoundingBox.SurfaceArea);

    /// <summary> The area weighted cost of splitting this node </summary>
    /// <param name="childrenCost">The summed <see cref="SurfaceAreaHeuristic"/> of the children</param>
    /// <returns>The cost of traversing this node plus that of its children</returns>
    protected float SplitCost(float childrenCost) => TraversalCost * Shape.BoundingBox.SurfaceArea + childrenCost;

    /// <summary> The left child node if it has one </summary>
    public BoundingVolumeHierarchy? Left { get; protected set; }
    IBinaryTree? IBinaryTree.Left => Left;
    /// <summary> The right child node if it has one </summary>
    public BoundingVolumeHierarchy? Right { get; protected set; }
    IBinaryTree? IBinaryTree.Right => Right;
    /// <summary> Whether this node is a leaf or not </summary>
    public bool Leaf => Left == null || Right == null;
    /// <summary> On which axis the node is split </summary>
    public Normal3 SplitDirection { get; protected set; }

    /// <summary> Create an empty BVH node </summary>
    public BoundingVolumeHierarchy() : base() { }

    /// <summary> Create a BVH node, splitting into smaller nodes if beneficial </summary>
    /// <param name="sceneObjects">The scene objects for the node</param>
    public BoundingVolumeHierarchy(IEnumerable<ISceneObject> sceneObjects) : this(sceneObjects, build: true) { }

    /// <summary> Create a BVH node, leaving the splitting to <see cref="Build"/> when <paramref name="build"/> is false </summary>
    /// <param name="sceneObjects">The scene objects for the node</param>
    /// <param name="build">Whether to build the node right away</param>
    protected BoundingVolumeHierarchy(IEnumerable<ISceneObject> sceneObjects, bool build) : base(sceneObjects) {
        if (build) Build();
    }

    private AxisAlignedBox? bounds;

    /// <summary> Split this node into smaller nodes if beneficial </summary>
    protected void Build() {
        TrySplit();
        // Fix the bounds now, so render threads only ever read them
        bounds = Leaf ? ItemBounds() : AxisAlignedBox.Encompass(Left!.Bounds, Right!.Bounds);
    }

    /// <summary> The bounding box of this node, cached by <see cref="Build"/> until the items change </summary>
    private AxisAlignedBox Bounds => bounds ?? Shape.BoundingBox;

    protected override void ItemsChanged() => bounds = null;

    /// <summary> Trace a <paramref name="ray"/> through the nearest child first, skipping the other child when nothing passes the first hit </summary>
    /// <param name="ray">The <see cref="IRay"/> to trace</param>
    /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
    /// <returns>The <see cref="IDistanceQuery"/> through the hierarchy</returns>
    public override IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
        var boxRay = new BoxRay(ray.Origin.Vector, ray.InvDirection.Vector, ray.Length);
        var boxTests = 1;
        var result = Bounds.EntryDistance(boxRay.Origin, boxRay.InverseDirection, boxRay.Length) is null ? null : TraceEntered(ray, boxRay, spectrum, ref boxTests);
        if (ray is CameraRay cameraRay) cameraRay.BVHTraversals += boxTests;
        return result;
    }

    private IDistanceQuery? TraceEntered(IRay ray, in BoxRay boxRay, ISpectrum spectrum, ref int boxTests) {
        if (Leaf) return base.Trace(ray, spectrum);
        boxTests += 2;
        var leftEntry = Left!.Bounds.EntryDistance(boxRay.Origin, boxRay.InverseDirection, boxRay.Length);
        var rightEntry = Right!.Bounds.EntryDistance(boxRay.Origin, boxRay.InverseDirection, boxRay.Length);
        if (leftEntry is null) return rightEntry is null ? null : Right.TraceEntered(ray, boxRay, spectrum, ref boxTests);
        if (rightEntry is null) return Left.TraceEntered(ray, boxRay, spectrum, ref boxTests);
        var (near, far, farEntry) = leftEntry <= rightEntry ? (Left, Right, rightEntry.Value) : (Right, Left, leftEntry.Value);
        var result = near.TraceEntered(ray, boxRay, spectrum, ref boxTests);
        if (result is not null && result.DistanceDistribution.CumulativeProbability(farEntry) >= 1) return result;
        return result + far.TraceEntered(ray, boxRay, spectrum, ref boxTests);
    }

    /// <summary> The parts of a ray that box tests read, taken from the <see cref="IRay"/> once per trace </summary>
    private readonly record struct BoxRay(Vector3 Origin, Vector3 InverseDirection, float Length);

    /// <summary> Try split node into two smaller nodes </summary>
    private void TrySplit() {
        var split = GetSplit();
        if (split is null) return;
        else Split(split);
    }

    /// <summary> Split this node </summary>
    /// <param name="split">The split to split this node with</param>
    protected virtual void Split(Split split) {
        Left = new BoundingVolumeHierarchy(split.LeftItems);
        Right = new BoundingVolumeHierarchy(split.RightItems);
        SplitDirection = split.Direction;
        Clear();
        Add(Left);
        Add(Right);
    }

    /// <summary> Get the best split for this node </summary>
    /// <returns>The best split for this BVH node</returns>
    protected virtual Split? GetSplit() {
        var items = new (ISceneObject Item, AxisAlignedBox Box)[Items.Count];
        var index = 0;
        foreach (var item in Items) {
            items[index++] = (item, item.Shape.BoundingBox);
        }
        var surfaceArea = Encompass(items).SurfaceArea;
        return Bin && items.Length > BinAmount ? BestBinSplit(items, surfaceArea) : BestSweepSplit(items, surfaceArea);
    }

    /// <summary> Check all possible splits </summary>
    /// <param name="items">The items of this node with their bounding boxes</param>
    /// <param name="surfaceArea">The surface area of this node</param>
    /// <returns>The best split for every axis</returns>
    private static Split? BestSweepSplit((ISceneObject Item, AxisAlignedBox Box)[] items, float surfaceArea) {
        var bestSplitCost = SurfaceAreaHeuristic(items.Length, surfaceArea);
        Split? bestSplit = null;
        var order = new (float Center, int Index)[items.Length];
        var rightAreas = new float[items.Length];
        foreach (var (direction, axis) in new (Normal3, Func<Vector3, float>)[] { (Normal3.UnitX, v => v.X), (Normal3.UnitY, v => v.Y), (Normal3.UnitZ, v => v.Z) }) {
            var split = BestLinearSplitAfterSort(items, surfaceArea, direction, axis, order, rightAreas, ref bestSplitCost);
            if (split is not null) bestSplit = split;
        }
        return bestSplit;
    }

    /// <summary> Split linearly over primitives after sorting them along an axis </summary>
    /// <param name="items">The items of this node with their bounding boxes</param>
    /// <param name="surfaceArea">The surface area of this node</param>
    /// <param name="sortDirection">The direction the primitives are sorted in</param>
    /// <param name="axis">The axis selector to sort the bounding box centers with</param>
    /// <param name="order">A buffer for the sorted order of the <paramref name="items"/></param>
    /// <param name="rightAreas">A buffer for the surface areas right of each split</param>
    /// <param name="bestSplitCost">The cost to beat, lowered when a better split is found</param>
    /// <returns>The best split along the axis if it beats <paramref name="bestSplitCost"/></returns>
    private static Split? BestLinearSplitAfterSort((ISceneObject Item, AxisAlignedBox Box)[] items, float surfaceArea, Normal3 sortDirection, Func<Vector3, float> axis,
        (float Center, int Index)[] order, float[] rightAreas, ref float bestSplitCost) {
        for (var i = 0; i < items.Length; i++) {
            order[i] = (axis(items[i].Box.Center.Vector), i);
        }
        Array.Sort(order);
        AxisAlignedBox? right = null;
        for (var i = items.Length - 1; i > 0; i--) {
            right = Encompass(right, items[order[i].Index].Box);
            rightAreas[i] = right.Value.SurfaceArea;
        }
        var bestLeftCount = 0;
        AxisAlignedBox? left = null;
        for (var leftCount = 1; leftCount < items.Length; leftCount++) {
            left = Encompass(left, items[order[leftCount - 1].Index].Box);
            var splitCost = SplitCost(surfaceArea, SurfaceAreaHeuristic(leftCount, left.Value.SurfaceArea) + SurfaceAreaHeuristic(items.Length - leftCount, rightAreas[leftCount]));
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestLeftCount = leftCount;
            }
        }
        if (bestLeftCount == 0) return null;
        var leftItems = new ISceneObject[bestLeftCount];
        var rightItems = new ISceneObject[items.Length - bestLeftCount];
        for (var i = 0; i < items.Length; i++) {
            if (i < bestLeftCount) leftItems[i] = items[order[i].Index].Item;
            else rightItems[i - bestLeftCount] = items[order[i].Index].Item;
        }
        return new Split(sortDirection, leftItems, rightItems);
    }

    /// <summary> Get the best binning split along the axis on which the centers of the items are spread widest </summary>
    /// <param name="items">The items of this node with their bounding boxes</param>
    /// <param name="surfaceArea">The surface area of this node</param>
    /// <returns>The best binning split, if any beats keeping this node a leaf</returns>
    private static Split? BestBinSplit((ISceneObject Item, AxisAlignedBox Box)[] items, float surfaceArea) {
        var minCenter = Position3.PositiveInfinity;
        var maxCenter = Position3.NegativeInfinity;
        foreach (var (_, box) in items) {
            minCenter = Position3.ComponentMin(minCenter, box.Center);
            maxCenter = Position3.ComponentMax(maxCenter, box.Center);
        }
        var centers = new AxisAlignedBox(minCenter, maxCenter);
        var size = centers.Size;
        var (binningDirection, axis) = size.X > size.Y && size.X > size.Z
            ? (Normal3.UnitX, (Func<Vector3, float>)(v => v.X))
            : size.Y > size.Z ? (Normal3.UnitY, v => v.Y) : (Normal3.UnitZ, v => v.Z);
        var extent = axis(size.Vector);
        // All centers coincide, so no bin boundary can separate them
        if (!(extent > 0) || !float.IsFinite(extent)) return null;
        var k1 = BinAmount * BinningEpsilon / extent;
        var binStart = axis(centers.MinCorner.Vector);
        // Populate bins
        var binIDs = new int[items.Length];
        var binCounts = new int[BinAmount];
        var binBoxes = new AxisAlignedBox?[BinAmount];
        for (var i = 0; i < items.Length; i++) {
            var box = items[i].Box;
            var binID = Math.Clamp((int)(k1 * (axis(box.Center.Vector) - binStart)), 0, BinAmount - 1);
            binIDs[i] = binID;
            binCounts[binID]++;
            binBoxes[binID] = Encompass(binBoxes[binID], box);
        }
        // Evaluate splits between bins
        var rightCounts = new int[BinAmount];
        var rightAreas = new float[BinAmount];
        AxisAlignedBox? right = null;
        for (int i = BinAmount - 1, count = 0; i > 0; i--) {
            count += binCounts[i];
            if (binBoxes[i] is { } binBox) right = Encompass(right, binBox);
            rightCounts[i] = count;
            rightAreas[i] = right?.SurfaceArea ?? 0;
        }
        var bestSplitCost = SurfaceAreaHeuristic(items.Length, surfaceArea);
        var bestSplitBin = 0;
        var bestLeftCount = 0;
        AxisAlignedBox? left = null;
        for (int i = 1, leftCount = 0; i < BinAmount; i++) {
            leftCount += binCounts[i - 1];
            if (binBoxes[i - 1] is { } binBox) left = Encompass(left, binBox);
            if (leftCount == 0 || rightCounts[i] == 0) continue;
            var splitCost = SplitCost(surfaceArea, SurfaceAreaHeuristic(leftCount, left!.Value.SurfaceArea) + SurfaceAreaHeuristic(rightCounts[i], rightAreas[i]));
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestSplitBin = i;
                bestLeftCount = leftCount;
            }
        }
        if (bestSplitBin == 0) return null;
        var leftItems = new ISceneObject[bestLeftCount];
        var rightItems = new ISceneObject[items.Length - bestLeftCount];
        for (int i = 0, leftIndex = 0, rightIndex = 0; i < items.Length; i++) {
            if (binIDs[i] < bestSplitBin) leftItems[leftIndex++] = items[i].Item;
            else rightItems[rightIndex++] = items[i].Item;
        }
        return new Split(binningDirection, leftItems, rightItems);
    }

    /// <summary> The area weighted cost of splitting a node </summary>
    /// <param name="surfaceArea">The surface area of the node</param>
    /// <param name="childrenCost">The summed <see cref="SurfaceAreaHeuristic"/> of the children</param>
    /// <returns>The cost of traversing the node plus that of its children</returns>
    private static float SplitCost(float surfaceArea, float childrenCost) => TraversalCost * surfaceArea + childrenCost;

    private static AxisAlignedBox Encompass(AxisAlignedBox? box, AxisAlignedBox other) => box is { } b ? AxisAlignedBox.Encompass(b, other) : other;

    private static AxisAlignedBox Encompass((ISceneObject Item, AxisAlignedBox Box)[] items) {
        AxisAlignedBox? bounds = null;
        foreach (var (_, box) in items) {
            bounds = Encompass(bounds, box);
        }
        return bounds ?? new AxisAlignedBox(ReadOnlySpan<AxisAlignedBox>.Empty);
    }

    /// <summary> The box around the items of this node </summary>
    /// <returns>The <see cref="AxisAlignedBox"/> encompassing the bounding boxes of the items</returns>
    private AxisAlignedBox ItemBounds() {
        AxisAlignedBox? bounds = null;
        foreach (var item in Items) {
            bounds = Encompass(bounds, item.Shape.BoundingBox);
        }
        return bounds ?? new AxisAlignedBox(ReadOnlySpan<AxisAlignedBox>.Empty);
    }
}
