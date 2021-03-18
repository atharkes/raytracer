using System.Collections.Generic;
using PathTracer.Pathtracing.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneObjects;

namespace PathTracer.Pathtracing.AccelerationStructures {
    /// <summary> A Split Bounding Volume Hierarchy tree 
    /// Possible Additions:
    /// - Check for each fragments if it is better to move to either side of the split
    /// - Implement Triangle and/or Polygon clipping to decrease AABB sizes
    /// - Use alpha to reduce memory footprint
    /// </summary>
    public class SBVHTree : BVHTree, IAccelerationStructure {
        /// <summary> Alpha blends between a regular BVH without any duplication (α = 1) and a full SBVH (α = 0). 
        /// A high alpha reduces the memory footprint of the SBVH due to a lower amount of fragments created. </summary>
        public const float Alpha = 0.00001f;
        /// <summary> The amount of bins considered during spatial splitting </summary>
        public const int SpatialBinAmount = 256;
        
        /// <summary> Create a spatial BVH </summary>
        /// <param name="primitives">The primitives to build the SBVH with</param>
        public SBVHTree(List<Primitive> primitives) {
            Root = new SBVHNode(primitives);
        }

        /// <summary> Intersect the <see cref="SBVHTree"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="SBVHTree"/> with</param>
        /// <returns>The found <see cref="Primitive"/> and distance along the <paramref name="ray"/></returns>
        public override (Primitive Primitive, float Distance)? Intersect(Ray ray) {
            (Primitive Primitive, float Distance)? intersection = base.Intersect(ray);
            if (intersection.HasValue && intersection.Value.Primitive is PrimitiveFragment fragment) {
                return (fragment.Original, intersection.Value.Distance);
            } else {
                return intersection;
            }
        }
    }
}
