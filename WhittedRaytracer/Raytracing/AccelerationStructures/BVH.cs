using OpenTK;
using System;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures {
    /// <summary> A node of a bounding volume hierarchy tree
    /// TODO:
    /// - Refitting (enable animation/movement)
    /// - Binning SAH while splitting (increase splitting performance)
    /// - Geometricly ordered traversal (increase performance)
    /// - Top-level BHV's for static and non-static parts
    /// - Use two arrays for primitives and BHV nodes, and using only a Left index and Count (decrease storage and increase performance) </summary>
    class BVHNode : IAccelerationStructure {
        /// <summary> The AABB of this BVH node </summary>
        public readonly AABB AABB;
        /// <summary> The left child node if it has one </summary>
        public BVHNode Left { get; private set; }
        /// <summary> The right child node if it has one </summary>
        public BVHNode Right { get; private set; }
        /// <summary> Whether this node is a leaf or not </summary>
        public bool Leaf { get; private set; } = true;

        /// <summary> The size of the AABB of the node </summary>
        public Vector3 Size => AABB.MaxBound - AABB.MinBound;

        /// <summary> The estimated cost of traversing the BVH for the SAH </summary>
        public const float TraversalCost = 1f;
        /// <summary> The estimated cost of intersecting a primitive for the SAH </summary>
        public const float IntersectionCost = 1f;
        /// <summary> The amount of bins used for the binning process </summary>
        public const int BinAmount = 4;
        /// <summary> An epsilon for the binning process </summary>
        public const float BinningEpsilon = 0.99999f;

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVHNode(List<Primitive> primitives) {
            AABB = new AABB(primitives);
            Split();
        }

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="aabb">The aabb to create the tree from</param>
        public BVHNode(AABB aabb) {
            AABB = aabb;
            Split();
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>The intersection in the BVH</returns>
        public Intersection Intersect(Ray ray) {
            if (!IntersectAABB(ray)) {
                return null;
            } else if (Leaf) {
                return IntersectPrimitives(ray);
            } else {
                return IntersectChildren(ray); 
            }
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>Whether there is an intersection with the BVH and the ray</returns>
        public bool IntersectBool(Ray ray) {
            if (!IntersectAABB(ray)) {
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

        /// <summary> Intersect the axis aligned bounding box of this Node (Amy Williams's An Efficient and Robust Ray–Box Intersection Algorithm) </summary>
        /// <param name="ray">The ray to calculate intersection for</param>
        /// <returns>Whether the ray intersects the bounding box</returns>
        public bool IntersectAABB(Ray ray) {
            float tmin = (AABB.Bounds[ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmax = (AABB.Bounds[1 - ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;

            float tymin = (AABB.Bounds[ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymax = (AABB.Bounds[1 - ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return false;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (AABB.Bounds[ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmax = (AABB.Bounds[1 - ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return false;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            return tmin > 0 || tmax > 0;
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

        /// <summary> Intersect the primitives of this BHV node </summary>
        /// <param name="ray">The ray to intersect the primitives with</param>
        /// <returns>The intersection if there is any</returns>
        Intersection IntersectPrimitives(Ray ray) {
            float intersectionDistance = ray.Length;
            Primitive intersectionPrimitive = null;
            foreach (Primitive primitive in AABB.Primitives) {
                float distance = primitive.Intersect(ray);
                if (0 < distance && distance < intersectionDistance) {
                    intersectionPrimitive = primitive;
                    intersectionDistance = distance;
                }
            }
            if (intersectionPrimitive == null) return null;
            else return new Intersection(ray, intersectionPrimitive, intersectionDistance);
        }

        /// <summary> Try split the BVH Node into 2 smaller Nodes </summary>
        void Split() {
            (AABB left, AABB right)? split = ComputeBestSplit();
            if (!split.HasValue) return;
            Left = new BVHNode(split.Value.left);
            Right = new BVHNode(split.Value.right);
            Leaf = false;
        }

        /// <summary> Compute the best split for this BVH node </summary>
        /// <returns>Either a tuple with the best split or null if there is no good split</returns>
        (AABB left, AABB right)? ComputeBestSplit() { 
            ICollection<(AABB left, AABB right)> splits = BinSplits();
            float bestCost = IntersectionCost * AABB.SurfaceArea * AABB.Primitives.Count;
            (AABB left, AABB right)? bestSplit = null;
            foreach ((AABB, AABB) split in splits) {
                float splitCost = SurfaceAreaHeuristic(split);
                if (splitCost < bestCost) {
                    bestSplit = split;
                    bestCost = splitCost;
                }
            }
            return bestSplit;
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
            AABB[] bins = Size.X > Size.Y && Size.X > Size.Z ? BinX() : (Size.Y > Size.Z ? BinY() : BinZ());
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
            AABB[] bins = new AABB[BinAmount];
            for (int i = 0; i < BinAmount; i++) bins[i] = new AABB();
            float k1 = BinAmount * BinningEpsilon / (AABB.MaxBound.X - AABB.MinBound.X);
            foreach (Primitive primitive in AABB.Primitives) {
                int binID = (int)(k1 * (primitive.GetCenter().X - AABB.MinBound.X));
                bins[binID].Add(primitive);
            }
            return bins;
        }

        AABB[] BinY() {
            AABB[] bins = new AABB[BinAmount];
            for (int i = 0; i < BinAmount; i++) bins[i] = new AABB();
            float k1 = BinAmount * BinningEpsilon / (AABB.MaxBound.Y - AABB.MinBound.Y);
            foreach (Primitive primitive in AABB.Primitives) {
                int binID = (int)(k1 * (primitive.GetCenter().Y - AABB.MinBound.Y));
                bins[binID].Add(primitive);
            }
            return bins;
        }

        AABB[] BinZ() {
            AABB[] bins = new AABB[BinAmount];
            for (int i = 0; i < BinAmount; i++) bins[i] = new AABB();
            float k1 = BinAmount * BinningEpsilon / (AABB.MaxBound.Z - AABB.MinBound.Z);
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

        /// <summary> Calculate the cost of a split. Uses the Surface Area Heuristic </summary>
        /// <param name="split">The split to calculate the cost for</param>
        /// <returns>The cost of the split</returns>
        public float SurfaceAreaHeuristic((AABB left, AABB right) split) {
            return TraversalCost * AABB.SurfaceArea + IntersectionCost * (split.left.SurfaceArea * split.left.Primitives.Count + split.right.SurfaceArea * split.right.Primitives.Count);
        }
    }
}