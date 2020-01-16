using OpenTK;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructure {
    /// <summary> A node of a bounding volume hierarchy tree </summary>
    class BVHNode {
        /// <summary> The AABB of this BVH node </summary>
        public readonly AABB AABB;
        /// <summary> The left child node if it has one </summary>
        public BVHNode Left { get; private set; }
        /// <summary> The right child node if it has one </summary>
        public BVHNode Right { get; private set; }
        /// <summary> Whether this node is a leaf or not </summary>
        public bool Leaf { get; private set; } = true;

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVHNode(List<Primitive> primitives) {
            AABB = new AABB(primitives);
            TrySplit();
        }

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="aabb">The aabb to create the tree from</param>
        public BVHNode(AABB aabb) {
            AABB = aabb;
            TrySplit();
        }

        /// <summary> Try split the BVH Node into 2 smaller Nodes </summary>
        void TrySplit() {
            (AABB left, AABB right)? split = ComputeBestSplit();
            if (!split.HasValue) return;
            Left = new BVHNode(split.Value.left);
            Right = new BVHNode(split.Value.right);
            Leaf = false;
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>The intersection in the BVH</returns>
        public Intersection Intersect(Ray ray) {
            ray.BVHTraversals++;
            if (!AABB.Intersect(ray)) {
                return null;
            } else if (Leaf) {
                return AABB.IntersectPrimitives(ray);
            } else {
                return IntersectChildren(ray); 
            }
        }

        /// <summary> Intersect the children of this BHV node </summary>
        /// <param name="ray">The ray to intersect the children with</param>
        /// <returns>The intersection in the children if there is any</returns>
        Intersection IntersectChildren(Ray ray) {
            Intersection intersectionLeft = Left.Intersect(ray);
            Intersection intersectionRight = Right.Intersect(ray);
            if (intersectionLeft == null) {
                return intersectionRight;
            } else if (intersectionRight == null) {
                return intersectionLeft;
            } else if (intersectionLeft.Distance < intersectionRight.Distance) {
                return intersectionLeft;
            } else {
                return intersectionRight;
            }
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>Whether there is an intersection with the BVH and the ray</returns>
        public bool IntersectBool(Ray ray) {
            if (!AABB.Intersect(ray)) {
                return false;
            } else if (Leaf) {
                foreach (Primitive primitive in AABB.Primitives) {
                    if (primitive.IntersectBool(ray)) return true;
                }
                return false;
            } else {
                return Left.IntersectBool(ray) || Right.IntersectBool(ray);
            }
        }

        /// <summary> Compute the best split for this BVH node </summary>
        /// <returns>Either a tuple with the best split or null if there is no good split</returns>
        (AABB left, AABB right)? ComputeBestSplit() { 
            ICollection<(AABB left, AABB right)> splits = BVH.Bin && BVH.BinAmount < AABB.Primitives.Count ? BinSplits() : AllSplits();
            float bestCost = AABB.SurfaceAreaHeuristic;
            (AABB left, AABB right)? bestSplit = null;
            foreach ((AABB, AABB) split in splits) {
                float splitCost = SplitSurfaceAreaHeuristic(split);
                if (splitCost < bestCost) {
                    bestSplit = split;
                    bestCost = splitCost;
                }
            }
            return bestSplit;
        }

        /// <summary> Calculate the cost of a split. Uses the Surface Area Heuristic </summary>
        /// <param name="split">The split to calculate the cost for</param>
        /// <returns>The cost of the split</returns>
        float SplitSurfaceAreaHeuristic((AABB left, AABB right) split) {
            return BVH.TraversalCost * AABB.SurfaceArea + split.left.SurfaceAreaHeuristic + split.right.SurfaceAreaHeuristic;
        }

        ICollection<(AABB left, AABB right)> AllSplits() {
            List<(AABB left, AABB right)> splits = new List<(AABB, AABB)>();
            foreach (Primitive primitive in AABB.Primitives) {
                splits.Add(SplitX(primitive));
                splits.Add(SplitY(primitive));
                splits.Add(SplitZ(primitive));
            }
            return splits;
        }

        ICollection<(AABB left, AABB right)> BinSplits() {
            Vector3 size = AABB.Size;
            AABB[] bins = size.X > size.Y && size.X > size.Z ? BinX() : (size.Y > size.Z ? BinY() : BinZ());
            List<(AABB, AABB)> splits = new List<(AABB, AABB)>(bins.Length - 1);
            for (int i = 1; i < bins.Length; i++) {
                AABB left = new AABB();
                for (int bin = 0; bin < i; bin++) left.AddRange(bins[bin].Primitives);
                AABB right = new AABB();
                for (int bin = i; bin < bins.Length; bin++) right.AddRange(bins[bin].Primitives);
                splits.Add((left, right));
            }
            return splits;
        }

        AABB[] BinX() {
            AABB[] bins = new AABB[BVH.BinAmount];
            for (int i = 0; i < BVH.BinAmount; i++) bins[i] = new AABB();
            float k1 = BVH.BinAmount * BVH.BinningEpsilon / (AABB.MaxBound.X - AABB.MinBound.X);
            foreach (Primitive primitive in AABB.Primitives) {
                int binID = (int)(k1 * (primitive.GetCenter().X - AABB.MinBound.X));
                bins[binID].Add(primitive);
            }
            return bins;
        }

        AABB[] BinY() {
            AABB[] bins = new AABB[BVH.BinAmount];
            for (int i = 0; i < BVH.BinAmount; i++) bins[i] = new AABB();
            float k1 = BVH.BinAmount * BVH.BinningEpsilon / (AABB.MaxBound.Y - AABB.MinBound.Y);
            foreach (Primitive primitive in AABB.Primitives) {
                int binID = (int)(k1 * (primitive.GetCenter().Y - AABB.MinBound.Y));
                bins[binID].Add(primitive);
            }
            return bins;
        }

        AABB[] BinZ() {
            AABB[] bins = new AABB[BVH.BinAmount];
            for (int i = 0; i < BVH.BinAmount; i++) bins[i] = new AABB();
            float k1 = BVH.BinAmount * BVH.BinningEpsilon / (AABB.MaxBound.Z - AABB.MinBound.Z);
            foreach (Primitive primitive in AABB.Primitives) {
                int binID = (int)(k1 * (primitive.GetCenter().Z - AABB.MinBound.Z));
                bins[binID].Add(primitive);
            }
            return bins;
        }

        /// <summary> Split on X-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (AABB left, AABB right) SplitX(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().X;
            AABB left = new AABB();
            AABB right = new AABB();
            foreach (Primitive primitive in AABB.Primitives) {
                if (primitive.GetCenter().X <= split) left.Add(primitive);
                else right.Add(primitive);
            }
            return (left, right);
        }

        /// <summary> Split on Y-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (AABB left, AABB right) SplitY(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().Y;
            AABB left = new AABB();
            AABB right = new AABB();
            foreach (Primitive primitive in AABB.Primitives) {
                if (primitive.GetCenter().Y <= split) left.Add(primitive);
                else right.Add(primitive);
            }
            return (left, right);
        }

        /// <summary> Split on Z-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (AABB left, AABB right) SplitZ(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().Z;
            AABB left = new AABB();
            AABB right = new AABB();
            foreach (Primitive primitive in AABB.Primitives) {
                if (primitive.GetCenter().Z <= split) left.Add(primitive);
                else right.Add(primitive);
            }
            return (left, right);
        }
    }
}