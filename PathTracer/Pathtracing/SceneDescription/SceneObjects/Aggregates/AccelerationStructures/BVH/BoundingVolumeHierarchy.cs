using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;

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
        Items = new HashSet<ISceneObject>() { Left, Right };
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
        List<Split?> splits = new() {
            BestLinearSplitAfterSort(Normal3.UnitX, p => p.Shape.BoundingBox.Center.X),
            BestLinearSplitAfterSort(Normal3.UnitY, p => p.Shape.BoundingBox.Center.Y),
            BestLinearSplitAfterSort(Normal3.UnitZ, p => p.Shape.BoundingBox.Center.Z)
        };
        splits.RemoveAll(s => s == null);
        return splits.OrderBy(s => s?.SurfaceAreaHeuristic).FirstOrDefault();
    }

    /// <summary> Split linearly over primitives after using a sorting function </summary>
    /// <param name="axisSortingFunc">The sorting function to sort the primitives with before finding splits</param>
    /// <returns>The best split using the sorting funciton</returns>
    private Split? BestLinearSplitAfterSort(Normal3 sortDirection, Func<ISceneObject, float> axisSortingFunc) {
        var orderedPrimitives = Items.OrderBy(axisSortingFunc).ToList();
        Split? split = new(sortDirection, new Aggregate(), new Aggregate(orderedPrimitives));
        ISceneObject? bestSplitPrimitive = null;
        var bestSplitCost = SurfaceAreaHeuristic(Items.Count, Shape.BoundingBox.SurfaceArea);
        foreach (var primitive in orderedPrimitives) {
            split.Left.Add(primitive);
            split.Right.Remove(primitive);
            var splitCost = split.SurfaceAreaHeuristic;
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestSplitPrimitive = primitive;
            }
        }
        if (bestSplitPrimitive == null) return null;
        var bestSplitPrimitiveIndex = orderedPrimitives.IndexOf(bestSplitPrimitive) + 1;
        var primitivesLeft = orderedPrimitives.GetRange(0, bestSplitPrimitiveIndex);
        var primitivesRight = orderedPrimitives.GetRange(bestSplitPrimitiveIndex, orderedPrimitives.Count - bestSplitPrimitiveIndex);
        return new Split(sortDirection, primitivesLeft, primitivesRight);
    }

    /// <summary> Get the best binning split for a certain axis </summary>
    /// <param name="binningDirection">The direction the binning proces is going</param>
    /// <param name="axis">The axis selector</param>
    /// <returns>The best binning split in the specified direction</returns>
    private Split? BestBinSplit(Normal3 binningDirection, Func<Vector3, float> axis) {
        var k1 = BinAmount * BinningEpsilon / axis(Shape.BoundingBox.Size.Vector);
        var bestSplitCost = SurfaceAreaHeuristic(Items.Count, Shape.BoundingBox.SurfaceArea);
        Split? bestSplit = null;
        for (var i = 1; i < BinAmount; i++) {
            Aggregate left = new();
            Aggregate right = new();
            // Populate Split
            foreach (var primitive in Items) {
                var binID = (int)(k1 * (axis(primitive.Shape.BoundingBox.Center.Vector) - axis(Shape.BoundingBox.MinCorner.Vector)));
                if (binID < i) left.Add(primitive);
                else right.Add(primitive);
            }
            // Evaluate Split
            Split split = new(binningDirection, left, right);
            var splitCost = split.SurfaceAreaHeuristic;
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestSplit = split;
            }
        }
        return bestSplit;
    }
}