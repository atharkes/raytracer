using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructure {
    /// <summary> A Bounding Volume Hierarchy tree used as acceleration structure for ray intersections in world space
    /// Possible Additions:
    /// - Refitting (enable animation/movement, adding and removing primitives)
    /// - Top-level BHV's for static and non-static parts
    /// </summary>
    public class BVH : IAccelerationStructure {
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

        /// <summary> The root node of the BVH </summary>
        public BVHNode Root { get; }

        /// <summary> Create a bounding volume hierarchy tree, splitting into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVH(List<Primitive> primitives) {
            Root = new BVHNode(primitives);
        }

        /// <summary> Intersect the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>The intersection in the BVH</returns>
        public Intersection Intersect(Ray ray) {
            return Root.Intersect(ray);
        }

        /// <summary> Intersect the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>Whether there is an intersection in the BVH</returns>
        public bool IntersectBool(Ray ray) {
            return Root.IntersectBool(ray);
        }
    }
}
