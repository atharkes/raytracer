﻿using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructure {
    /// <summary> A Bounding Volume Hierarchy tree used as acceleration structure for ray intersections in world space
    /// Possible Additions:
    /// - Refitting (enable animation/movement, adding and removing primitives)
    /// - Top-level BHV's for static and non-static parts
    /// - Geometricly ordered traversal using Node Split Signs (increase performance). Using DistanceSquared from AABB decreases performance in a random scene
    /// - Use two arrays for primitives and BHV nodes, and using only a Left index and Count (decrease storage and increase performance)
    /// </summary>
    class BVH : IAccelerationStructure {
        /// <summary> The estimated cost of traversing the BVH for the SAH </summary>
        public const float TraversalCost = 1f;
        /// <summary> The estimated cost of intersecting a primitive for the SAH </summary>
        public const float IntersectionCost = 1f;
        /// <summary> The amount of bins used for the binning process </summary>
        public const int BinAmount = 16;
        /// <summary> An epsilon for the binning process </summary>
        public const float BinningEpsilon = 0.99999f;

        /// <summary> The root node of the BVH </summary>
        readonly BVHNode root;

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVH(List<Primitive> primitives) {
            root = new BVHNode(primitives);
        }

        /// <summary> Intersect the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>The intersection in the BVH</returns>
        public Intersection Intersect(Ray ray) {
            return root.Intersect(ray);
        }

        /// <summary> Intersect the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>Whether there is an intersection in the BVH</returns>
        public bool IntersectBool(Ray ray) {
            return root.IntersectBool(ray);
        }
    }
}