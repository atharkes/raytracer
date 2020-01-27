using OpenTK;
using System;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructures.BVH;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures.SBVH {
    /// <summary> A node of a split BVH </summary>
    public class SBVHNode : BVHNode {
        /// <summary> Create a SBVH node, splitting into smaller nodes if beneficial </summary>
        /// <param name="primitives">The primitives in the node</param>
        public SBVHNode(List<Primitive> primitives) : base(primitives) { }

        /// <summary> Create a SBVH node, splitting into smaller nodes if beneficial </summary>
        /// <param name="aabb">The AABB to add to the node</param>
        public SBVHNode(AABB aabb) : base(aabb) { }

        /// <summary> The SBVH thinks it can get a better split </summary>
        /// <returns>A better or equal split than the normal BVH would</returns>
        protected override Split GetSplit() {
            Split bvhSplit = base.GetSplit();
            Split sbvhSplit;
            Vector3 size = AABB.Size;
            if (size.X > size.Y && size.X > size.Z) {
                sbvhSplit = BestSpatialBinSplit(Vector3.UnitX, v => v.X);
            } else if (size.Y > size.Z) {
                sbvhSplit = BestSpatialBinSplit(Vector3.UnitY, v => v.Y);
            } else {
                sbvhSplit = BestSpatialBinSplit(Vector3.UnitZ, v => v.Z);
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
            Left = new SBVHNode(split.Left);
            Right = new SBVHNode(split.Right);
            SplitDirection = split.Direction;
            Leaf = false;
            AABB = new AABB(Left.AABB, Right.AABB);
        }

        /// <summary> Get the best spatial binning split for a certain axis </summary>
        /// <param name="binningDirection">The direction the binning proces is going</param>
        /// <param name="axis">The axis selector</param>
        /// <returns>The best spatial binning split in the specified direction</returns>
        Split BestSpatialBinSplit(Vector3 binningDirection, Func<Vector3, float> axis) {
            float binStart = axis(AABB.MinBound);
            float binEnd = axis(AABB.MaxBound);
            float binSize = axis(AABB.Size) / SBVHTree.SpatialBinAmount;
            float k1 = SBVHTree.SpatialBinAmount * BVHTree.BinningEpsilon / axis(AABB.Size);
            float bestSplitCost = AABB.SurfaceAreaHeuristic;
            Split bestSplit = null;
            for (int i = 1; i < SBVHTree.SpatialBinAmount; i++) {
                SpatialBin left = new SpatialBin(binningDirection, binStart, binSize * i);
                SpatialBin right = new SpatialBin(binningDirection, binSize * i, binEnd);
                // Populate Split
                foreach (Primitive primitive in AABB.Primitives) {
                    int binID1 = (int)(k1 * (axis(primitive.Bounds[0]) - axis(AABB.MinBound)));
                    int binID2 = (int)(k1 * (axis(primitive.Bounds[1]) - axis(AABB.MinBound)));
                    if (binID1 < i && binID2 < i) left.AABB.Add(primitive);
                    else if (binID1 > i && binID2 > i) right.AABB.Add(primitive);
                    else {
                        left.ClipAndAdd(primitive);
                        right.ClipAndAdd(primitive);
                    }
                }
                // Evaluate Split
                Split split = new Split(binningDirection, left.AABB, right.AABB);
                float splitCost = split.SurfaceAreaHeuristic;
                if (splitCost < bestSplitCost) {
                    bestSplitCost = splitCost;
                    bestSplit = split;
                }
            }
            return bestSplit;
        }
    }
}
