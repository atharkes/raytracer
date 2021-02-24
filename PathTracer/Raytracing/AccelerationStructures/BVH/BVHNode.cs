using OpenTK.Mathematics;
using PathTracer.Raytracing.SceneObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Raytracing.AccelerationStructures.BVH {
    /// <summary> A node of a bounding volume hierarchy tree </summary>
    public class BVHNode : IBVHNode {
        /// <summary> The AABB of this BVH node </summary>
        public AABB AABB { get; protected set; }
        /// <summary> The left child node if it has one </summary>
        public IBVHNode? Left { get; protected set; }
        /// <summary> The right child node if it has one </summary>
        public IBVHNode? Right { get; protected set; }
        /// <summary> Whether this node is a leaf or not </summary>
        public bool Leaf => Left == null || Right == null;
        /// <summary> On which axis the node is split </summary>
        public Vector3 SplitDirection { get; protected set; }

        /// <summary> Create an empty BVH node </summary>
        public BVHNode() {
            AABB = new AABB();
        }

        /// <summary> Create a BVH node, splitting into smaller nodes if benificial </summary>
        /// <param name="primitives">The primitives in the node</param>
        public BVHNode(List<Primitive> primitives) {
            if (primitives.Count == 0) throw new ArgumentException("Don't make an empty BVHNode");
            AABB = new AABB(primitives);
            TrySplit();
        }

        /// <summary> Create a BVH node, splitting into smaller nodes if bene ficial </summary>
        /// <param name="aabb">The aabb for the node</param>
        public BVHNode(AABB aabb) {
            AABB = aabb;
            TrySplit();
        }

        /// <summary> Try split node into two smaller nodes </summary>
        void TrySplit() {
            Split? split = GetSplit();
            if (split is null) return;
            else Split(split);
        }

        /// <summary> Split this node </summary>
        /// <param name="split">The split to split this node with</param>
        protected virtual void Split(Split split) {
            Left = new BVHNode(split.Left);
            Right = new BVHNode(split.Right);
            SplitDirection = split.Direction;
            AABB = new AABB(Left.AABB, Right.AABB);
        }

        /// <summary> Get the best split for this node </summary>
        /// <returns>The best split for this BVH node</returns>
        protected virtual Split? GetSplit() {
            if (BVHTree.Bin && AABB.Primitives.Count > BVHTree.BinAmount) {
                Vector3 size = AABB.Size;
                if (size.X > size.Y && size.X > size.Z) return BestBinSplit(Vector3.UnitX, v => v.X);
                else if (size.Y > size.Z) return BestBinSplit(Vector3.UnitY, v => v.Y);
                else return BestBinSplit(Vector3.UnitZ, v => v.Z);
            } else {
                return BestSweepSplit();
            }
        }

        /// <summary> Check all possible splits </summary>
        /// <returns>The best split for every axis</returns>
        Split? BestSweepSplit() {
            List<Split?> splits = new List<Split?> {
                BestLinearSplitAfterSort(Vector3.UnitX, p => p.Center.X),
                BestLinearSplitAfterSort(Vector3.UnitY, p => p.Center.Y),
                BestLinearSplitAfterSort(Vector3.UnitZ, p => p.Center.Z)
            };
            splits.RemoveAll(s => s == null);
            return splits.OrderBy(s => s?.SurfaceAreaHeuristic).FirstOrDefault();
        }

        /// <summary> Split linearly over primitives after using a sorting function </summary>
        /// <param name="axisSortingFunc">The sorting function to sort the primitives with before finding splits</param>
        /// <returns>The best split using the sorting funciton</returns>
        Split? BestLinearSplitAfterSort(Vector3 sortDirection, Func<IAABB, float> axisSortingFunc) {
            List<IAABB> orderedPrimitives = AABB.Primitives.OrderBy(axisSortingFunc).ToList();
            Split? split = new Split(sortDirection, new AABB(), new AABB(orderedPrimitives));
            IAABB? bestSplitPrimitive = null;
            float bestSplitCost = AABB.SurfaceAreaHeuristic;
            foreach (IAABB primitive in orderedPrimitives) {
                split.Left.Add(primitive);
                split.Right.Remove(primitive);
                float splitCost = split.SurfaceAreaHeuristic;
                if (splitCost < bestSplitCost) {
                    bestSplitCost = splitCost;
                    bestSplitPrimitive = primitive;
                }
            }
            if (bestSplitPrimitive == null) return null;
            int bestSplitPrimitiveIndex = orderedPrimitives.IndexOf(bestSplitPrimitive) + 1;
            List<IAABB> primitivesLeft = orderedPrimitives.GetRange(0, bestSplitPrimitiveIndex);
            List<IAABB> primitivesRight = orderedPrimitives.GetRange(bestSplitPrimitiveIndex, orderedPrimitives.Count - bestSplitPrimitiveIndex);
            return new Split(sortDirection, primitivesLeft, primitivesRight);
        }

        /// <summary> Get the best binning split for a certain axis </summary>
        /// <param name="binningDirection">The direction the binning proces is going</param>
        /// <param name="axis">The axis selector</param>
        /// <returns>The best binning split in the specified direction</returns>
        Split? BestBinSplit(Vector3 binningDirection, Func<Vector3, float> axis) {
            float k1 = BVHTree.BinAmount * BVHTree.BinningEpsilon / axis(AABB.Size);
            float bestSplitCost = AABB.SurfaceAreaHeuristic;
            Split? bestSplit = null;
            for (int i = 1; i < BVHTree.BinAmount; i++) {
                AABB left = new AABB();
                AABB right = new AABB();
                // Populate Split
                foreach (IAABB primitive in AABB.Primitives) {
                    int binID = (int)(k1 * (axis(primitive.Center) - axis(AABB.MinBound)));
                    if (binID < i) left.Add(primitive);
                    else right.Add(primitive);
                }
                // Evaluate Split
                Split split = new Split(binningDirection, left, right);
                float splitCost = split.SurfaceAreaHeuristic;
                if (splitCost < bestSplitCost) {
                    bestSplitCost = splitCost;
                    bestSplit = split;
                }
            }
            return bestSplit;
        }

        /// <summary> Intersect and traverse the node with a ray </summary>
        /// <param name="ray">The ray to intersect the node with</param>
        /// <returns>The intersection in the node or any of its children</returns>
        public Intersection? Intersect(Ray ray) {
            if (ray is CameraRay cameraRay) cameraRay.BVHTraversals++;
            if (!AABB.IntersectBool(ray)) {
                return null;
            } else if (Leaf) {
                return AABB.IntersectPrimitives(ray);
            } else {
                return IntersectChildren(ray); 
            }
        }

        /// <summary> Intersect the children of this node </summary>
        /// <param name="ray">The ray to intersect the children with</param>
        /// <returns>The intersection in the children if there is any</returns>
        Intersection? IntersectChildren(Ray ray) {
            Intersection? firstIntersection;
            Intersection? secondIntersection;
            if (Vector3.Dot(SplitDirection, ray.Direction) < 0) {
                firstIntersection = Left?.Intersect(ray);
                secondIntersection = Right?.Intersect(ray);
            } else {
                firstIntersection = Right?.Intersect(ray);
                secondIntersection = Left?.Intersect(ray);
            }
            if (secondIntersection == null) {
                return firstIntersection;
            } else {
                return secondIntersection;
            }
        }

        /// <summary> Intersect and traverse the node with a ray </summary>
        /// <param name="ray">The ray to intersect the node with</param>
        /// <returns>Whether there is an intersection in this node or any of its children</returns>
        public bool IntersectBool(Ray ray) {
            if (!AABB.IntersectBool(ray)) {
                return false;
            } else if (Leaf) {
                foreach (Primitive primitive in AABB.Primitives) {
                    if (primitive.IntersectBool(ray)) return true;
                }
                return false;
            } else {
                return (Left?.IntersectBool(ray) ?? false) || (Right?.IntersectBool(ray) ?? false);
            }
        }
    }
}