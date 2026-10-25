using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;

/// <summary> A Split Bounding Volume Hierarchy tree 
/// Possible Additions:
/// - Check for each fragments if it is better to move to either side of the split
/// - Implement Triangle and/or Polygon clipping to decrease AABB sizes
/// </summary>
public class SpatialBVH : BoundingVolumeHierarchy {
    /// <summary> Alpha blends between a regular BVH without any duplication (α = 1) and a full SBVH (α = 0). 
    /// A high alpha reduces the memory footprint of the SBVH due to a lower amount of fragments created. </summary>
    public const float Alpha = 0.00001f;
    /// <summary> The amount of bins considered during spatial splitting </summary>
    public const int SpatialBinAmount = 256;
    /// <summary> The depth below which no more spatial splits are tried, bounding how often a primitive is split </summary>
    public const int MaxSpatialSplitDepth = 32;

    /// <summary> The surface area of the root node, which <see cref="Alpha"/> is relative to </summary>
    public float RootSurfaceArea { get; }
    /// <summary> The depth of this node, the root being 0 </summary>
    public int Depth { get; }

    /// <summary> Create a SBVH node, splitting into smaller nodes if beneficial </summary>
    /// <param name="primitives">The primitives in the node</param>
    public SpatialBVH(IEnumerable<ISceneObject> primitives) : this(primitives, null, 0) { }

    private SpatialBVH(IEnumerable<ISceneObject> items, float? rootSurfaceArea, int depth) : base(items, build: false) {
        RootSurfaceArea = rootSurfaceArea ?? Shape.BoundingBox.SurfaceArea;
        Depth = depth;
        Build();
    }

    /// <summary> The SBVH thinks it can get a better split </summary>
    /// <returns>A better or equal split than the normal BVH would</returns>
    protected override Split? GetSplit() {
        var bvhSplit = base.GetSplit();
        if (Depth >= MaxSpatialSplitDepth || (bvhSplit is not null && OverlapArea(bvhSplit) <= Alpha * RootSurfaceArea)) return bvhSplit;
        Split? sbvhSplit;
        var size = Shape.BoundingBox.Size;
        sbvhSplit = size.X > size.Y && size.X > size.Z
            ? BestSpatialBinSplit(Normal3.UnitX, AxisAlignedPlane.X, v => v.X)
            : size.Y > size.Z
                ? BestSpatialBinSplit(Normal3.UnitY, AxisAlignedPlane.Y, v => v.Y)
                : BestSpatialBinSplit(Normal3.UnitZ, AxisAlignedPlane.Z, v => v.Z);
        if (bvhSplit == null) return sbvhSplit;
        if (sbvhSplit == null) return bvhSplit;
        var bvhSplitSAH = bvhSplit.SurfaceAreaHeuristic;
        var sbvhSplitSAH = sbvhSplit.SurfaceAreaHeuristic;
        return bvhSplitSAH < sbvhSplitSAH ? bvhSplit : sbvhSplit;
    }

    /// <summary> Split this node </summary>
    /// <param name="split">The split to split this node with</param>
    protected override void Split(Split split) {
        Left = new SpatialBVH(split.LeftItems, RootSurfaceArea, Depth + 1);
        Right = new SpatialBVH(split.RightItems, RootSurfaceArea, Depth + 1);
        SplitDirection = split.Direction;
        Clear();
        Add(Left);
        Add(Right);
    }

    /// <summary> Get the best spatial binning split for a certain axis </summary>
    /// <param name="clipPlaneCreator">The direction the binning proces is going</param>
    /// <param name="axis">The axis selector</param>
    /// <returns>The best spatial binning split in the specified direction</returns>
    private Split? BestSpatialBinSplit(Normal3 splitDirection, Func<bool, float, AxisAlignedPlane> clipPlaneCreator, Func<Vector3, float> axis) {
        var bounds = Shape.BoundingBox;
        var binStart = axis(bounds.MinCorner.Vector);
        var binEnd = axis(bounds.MaxCorner.Vector);
        var binSize = axis(bounds.Size.Vector) / SpatialBinAmount;
        var k1 = SpatialBinAmount * BinningEpsilon / axis(bounds.Size.Vector);
        var bestSplitCost = LeafCost;
        var binnedItems = Items.Select(item => {
            var box = item.Shape.BoundingBox;
            return (Item: item, BinID1: (int)(k1 * (axis(box.MinCorner.Vector) - binStart)), BinID2: (int)(k1 * (axis(box.MaxCorner.Vector) - binStart)));
        }).ToArray();
        Split? bestSplit = null;
        for (var i = 1; i < SpatialBinAmount; i++) {
            SpatialBin left = new(clipPlaneCreator, binStart, binStart + binSize * i);
            SpatialBin right = new(clipPlaneCreator, binStart + binSize * i, binEnd);
            // Populate Split
            foreach (var (primitive, binID1, binID2) in binnedItems) {
                if (binID2 < i) left.Aggregate.Add(primitive);
                else if (binID1 >= i) right.Aggregate.Add(primitive);
                else {
                    left.ClipAndAdd(primitive);
                    right.ClipAndAdd(primitive);
                }
            }
            // Evaluate Split
            if (left.Aggregate.ItemCount == 0 || right.Aggregate.ItemCount == 0) continue;
            Split split = new(splitDirection, left.Aggregate, right.Aggregate);
            var splitCost = SplitCost(split.SurfaceAreaHeuristic);
            if (splitCost < bestSplitCost) {
                bestSplitCost = splitCost;
                bestSplit = split;
            }
        }
        // TODO: Reference Unsplitting on best Split
        return bestSplit;
    }

    /// <summary> The surface area of the overlap between the two sides of a <paramref name="split"/> </summary>
    private static float OverlapArea(Split split) {
        var left = split.Left.Shape.BoundingBox;
        var right = split.Right.Shape.BoundingBox;
        var min = Position3.ComponentMax(left.MinCorner, right.MinCorner);
        var max = Position3.ComponentMin(left.MaxCorner, right.MaxCorner);
        return min.X <= max.X && min.Y <= max.Y && min.Z <= max.Z ? new AxisAlignedBox(min, max).SurfaceArea : 0;
    }
}
