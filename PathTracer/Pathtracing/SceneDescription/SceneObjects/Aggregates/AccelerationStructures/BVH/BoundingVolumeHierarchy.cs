using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
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

    public static float SurfaceAreaHeuristic(int itemCount, float surfaceArea) => TraversalCost + IntersectionCost * itemCount * surfaceArea;

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
    public BoundingVolumeHierarchy(IEnumerable<ISceneObject> sceneObjects) : base(sceneObjects) {
        TrySplit();
    }

    /// <summary> Try split node into two smaller nodes </summary>
    private void TrySplit() {
        var split = GetSplit();
        if (split is null) return;
        else Split(split);
    }

    /// <summary> Split this node </summary>
    /// <param name="split">The split to split this node with</param>
    protected virtual void Split(Split split) {
        Left = new BoundingVolumeHierarchy(split.Left);
        Right = new BoundingVolumeHierarchy(split.Right);
        SplitDirection = split.Direction;
        Clear();
        Add(Left);
        Add(Right);
    }

    /// <summary> Get the best split for this node </summary>
    /// <returns>The best split for this BVH node</returns>
    protected virtual Split? GetSplit() {
        if (Bin && Items.Count > BinAmount) {
            var size = Shape.BoundingBox.Size;
            return size.X > size.Y && size.X > size.Z
                ? BestBinSplit(Normal3.UnitX, v => v.X)
                : size.Y > size.Z ? BestBinSplit(Normal3.UnitY, v => v.Y) : BestBinSplit(Normal3.UnitZ, v => v.Z);
        } else {
            return BestSweepSplit();
        }
    }

    /// <summary> Check all possible splits </summary>
    /// <returns>The best split for every axis</returns>
    private Split? BestSweepSplit() {
        var bestSplitCost = SurfaceAreaHeuristic(Items.Count, Shape.BoundingBox.SurfaceArea);
        Split? bestSplit = null;
        foreach (var (direction, axis) in new (Normal3, Func<Vector3, float>)[] { (Normal3.UnitX, v => v.X), (Normal3.UnitY, v => v.Y), (Normal3.UnitZ, v => v.Z) }) {
            var split = BestLinearSplitAfterSort(direction, axis, ref bestSplitCost);
            if (split is not null) bestSplit = split;
        }
        return bestSplit;
    }

    /// <summary> Split linearly over primitives after sorting them along an axis </summary>
    /// <param name="sortDirection">The direction the primitives are sorted in</param>
    /// <param name="axis">The axis selector to sort the bounding box centers with</param>
    /// <param name="bestSplitCost">The cost to beat, lowered when a better split is found</param>
    /// <returns>The best split along the axis if it beats <paramref name="bestSplitCost"/></returns>
    private Split? BestLinearSplitAfterSort(Normal3 sortDirection, Func<Vector3, float> axis, ref float bestSplitCost) {
        var sorted = Items.Select(item => (Item: item, Box: item.Shape.BoundingBox)).OrderBy(p => axis(p.Box.Center.Vector)).ToArray();
        var rightAreas = new float[sorted.Length];
        AxisAlignedBox? right = null;
        for (var i = sorted.Length - 1; i > 0; i--) {
            right = Encompass(right, sorted[i].Box);
            rightAreas[i] = right.Value.SurfaceArea;
        }
        var bestLeftCount = 0;
        AxisAlignedBox? left = null;
        for (var leftCount = 1; leftCount < sorted.Length; leftCount++) {
            left = Encompass(left, sorted[leftCount - 1].Box);
            var splitCost = SurfaceAreaHeuristic(leftCount, left.Value.SurfaceArea) + SurfaceAreaHeuristic(sorted.Length - leftCount, rightAreas[leftCount]);
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestLeftCount = leftCount;
            }
        }
        if (bestLeftCount == 0) return null;
        return new Split(sortDirection, sorted[..bestLeftCount].Select(p => p.Item), sorted[bestLeftCount..].Select(p => p.Item));
    }

    /// <summary> Get the best binning split for a certain axis </summary>
    /// <param name="binningDirection">The direction the binning proces is going</param>
    /// <param name="axis">The axis selector</param>
    /// <returns>The best binning split in the specified direction</returns>
    private Split? BestBinSplit(Normal3 binningDirection, Func<Vector3, float> axis) {
        var bounds = Shape.BoundingBox;
        var k1 = BinAmount * BinningEpsilon / axis(bounds.Size.Vector);
        if (!float.IsFinite(k1)) return BestSweepSplit();
        var binStart = axis(bounds.MinCorner.Vector);
        // Populate bins
        var binIDs = new Dictionary<ISceneObject, int>(Items.Count);
        var binCounts = new int[BinAmount];
        var binBoxes = new AxisAlignedBox?[BinAmount];
        foreach (var item in Items) {
            var box = item.Shape.BoundingBox;
            var binID = Math.Clamp((int)(k1 * (axis(box.Center.Vector) - binStart)), 0, BinAmount - 1);
            binIDs[item] = binID;
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
        var bestSplitCost = SurfaceAreaHeuristic(Items.Count, bounds.SurfaceArea);
        var bestSplitBin = 0;
        AxisAlignedBox? left = null;
        for (int i = 1, leftCount = 0; i < BinAmount; i++) {
            leftCount += binCounts[i - 1];
            if (binBoxes[i - 1] is { } binBox) left = Encompass(left, binBox);
            if (leftCount == 0 || rightCounts[i] == 0) continue;
            var splitCost = SurfaceAreaHeuristic(leftCount, left!.Value.SurfaceArea) + SurfaceAreaHeuristic(rightCounts[i], rightAreas[i]);
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestSplitBin = i;
            }
        }
        if (bestSplitBin == 0) return null;
        return new Split(binningDirection, Items.Where(item => binIDs[item] < bestSplitBin), Items.Where(item => binIDs[item] >= bestSplitBin));
    }

    private static AxisAlignedBox Encompass(AxisAlignedBox? box, AxisAlignedBox other) => box is { } b ? AxisAlignedBox.Encompass(b, other) : other;
}
