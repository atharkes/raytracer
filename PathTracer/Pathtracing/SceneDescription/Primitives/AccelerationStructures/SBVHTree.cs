using PathTracer.Pathtracing.AccelerationStructures.SBVH;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Primitives.AccelerationStructures {
    /// <summary> A Split Bounding Volume Hierarchy tree 
    /// Possible Additions:
    /// - Check for each fragments if it is better to move to either side of the split
    /// - Implement Triangle and/or Polygon clipping to decrease AABB sizes
    /// - Use alpha to reduce memory footprint
    /// </summary>
    public class SBVHTree : AccelerationStructure {
        /// <summary> Alpha blends between a regular BVH without any duplication (α = 1) and a full SBVH (α = 0). 
        /// A high alpha reduces the memory footprint of the SBVH due to a lower amount of fragments created. </summary>
        public const float Alpha = 0.00001f;
        /// <summary> The amount of bins considered during spatial splitting </summary>
        public const int SpatialBinAmount = 256;
        
        /// <summary> Create a spatial BVH </summary>
        /// <param name="primitives">The primitives to build the SBVH with</param>
        public SBVHTree(List<Shape> primitives) {
            Root = new SBVHNode(primitives);
        }

        /// <summary> Intersect the <see cref="SBVHTree"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="SBVHTree"/> with</param>
        /// <returns>The found <see cref="Shape"/> and distance along the <paramref name="ray"/></returns>
        public override (Shape Primitive, float Distance)? Intersect(Ray ray) {
            (Shape Primitive, float Distance)? intersection = base.Intersect(ray);
            if (intersection.HasValue && intersection.Value.Primitive is PrimitiveFragment fragment) {
                return (fragment.Original, intersection.Value.Distance);
            } else {
                return intersection;
            }
        }
    }
}
