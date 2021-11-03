using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH {
    /// <summary> A Split Bounding Volume Hierarchy tree 
    /// Possible Additions:
    /// - Check for each fragments if it is better to move to either side of the split
    /// - Implement Triangle and/or Polygon clipping to decrease AABB sizes
    /// - Use alpha to reduce memory footprint
    /// </summary>
    public class SpatialBVH : BoundingVolumeHierarchy {
        /// <summary> Alpha blends between a regular BVH without any duplication (α = 1) and a full SBVH (α = 0). 
        /// A high alpha reduces the memory footprint of the SBVH due to a lower amount of fragments created. </summary>
        public const float Alpha = 0.00001f;
        /// <summary> The amount of bins considered during spatial splitting </summary>
        public const int SpatialBinAmount = 256;

        /// <summary> Create a SBVH node, splitting into smaller nodes if beneficial </summary>
        /// <param name="primitives">The primitives in the node</param>
        public SpatialBVH(List<ISceneObject> primitives) : base(primitives) { }

        /// <summary> Create a SBVH node, splitting into smaller nodes if beneficial </summary>
        /// <param name="aggregate">The AABB to add to the node</param>
        public SpatialBVH(IAggregate aggregate) : base(aggregate) { }

        /// <summary> The SBVH thinks it can get a better split </summary>
        /// <returns>A better or equal split than the normal BVH would</returns>
        protected override Split? GetSplit() {
            Split? bvhSplit = base.GetSplit();
            // TODO: Introduce Alpha to just use regular split
            Split? sbvhSplit;
            IDirection3 size = BoundingBox.Size;
            if (size.X > size.Y && size.X > size.Z) {
                sbvhSplit = BestSpatialBinSplit(Normal3.UnitX, AxisAlignedPlane.X, v => v.X);
            } else if (size.Y > size.Z) {
                sbvhSplit = BestSpatialBinSplit(Normal3.UnitY, AxisAlignedPlane.Y, v => v.Y);
            } else {
                sbvhSplit = BestSpatialBinSplit(Normal3.UnitZ, AxisAlignedPlane.Z, v => v.Z);
            }
            if (bvhSplit == null) return sbvhSplit;
            if (sbvhSplit == null) return bvhSplit;
            float bvhSplitSAH = bvhSplit.SurfaceAreaHeuristic;
            float sbvhSplitSAH = sbvhSplit.SurfaceAreaHeuristic;
            return bvhSplitSAH < sbvhSplitSAH ? bvhSplit : sbvhSplit;
        }

        /// <summary> Split this node </summary>
        /// <param name="split">The split to split this node with</param>
        protected override void Split(Split split) {
            Left = new SpatialBVH(split.Left);
            Right = new SpatialBVH(split.Right);
            SplitDirection = split.Direction;
            Items = new HashSet<ISceneObject>() { Left, Right };
        }

        /// <summary> Get the best spatial binning split for a certain axis </summary>
        /// <param name="clipPlaneCreator">The direction the binning proces is going</param>
        /// <param name="axis">The axis selector</param>
        /// <returns>The best spatial binning split in the specified direction</returns>
        Split? BestSpatialBinSplit(Normal3 splitDirection, Func<bool, float, AxisAlignedPlane> clipPlaneCreator, Func<Vector3, float> axis) {
            float binStart = axis(BoundingBox.MinCorner.Vector);
            float binEnd = axis(BoundingBox.MaxCorner.Vector);
            float binSize = axis(BoundingBox.Size.Vector) / SpatialBinAmount;
            float k1 = SpatialBinAmount * BinningEpsilon / axis(BoundingBox.Size.Vector);
            float bestSplitCost = SurfaceAreaHeuristic(Items.Count, BoundingBox.SurfaceArea);
            Split? bestSplit = null;
            for (int i = 1; i < SpatialBinAmount; i++) {
                SpatialBin left = new(clipPlaneCreator, binStart, binSize * i);
                SpatialBin right = new(clipPlaneCreator, binSize * i, binEnd);
                // Populate Split
                foreach (ISceneObject primitive in Items) {
                    int binID1 = (int)(k1 * (axis(primitive.BoundingBox.MinCorner.Vector) - axis(BoundingBox.MinCorner.Vector)));
                    int binID2 = (int)(k1 * (axis(primitive.BoundingBox.MaxCorner.Vector) - axis(BoundingBox.MinCorner.Vector)));
                    if (binID1 < i && binID2 < i) left.Aggregate.Add(primitive);
                    else if (binID1 > i && binID2 > i) right.Aggregate.Add(primitive);
                    else {
                        left.ClipAndAdd(primitive);
                        right.ClipAndAdd(primitive);
                    }
                }
                // Evaluate Split
                Split split = new(splitDirection, left.Aggregate, right.Aggregate);
                float splitCost = split.SurfaceAreaHeuristic;
                if (splitCost < bestSplitCost) {
                    bestSplitCost = splitCost;
                    bestSplit = split;
                }
            }
            // TODO: Reference Unsplitting on best Split
            return bestSplit;
        }
    }
}
