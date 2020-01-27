using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructure.BVH;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructure.SBVH {
    /// <summary> A node of a spatial BVH </summary>
    public class SBVHNode : BVHNode {
        /// <summary> Create a SBVH node, splitting into smaller nodes if benificial </summary>
        /// <param name="primitives">The primitives in the node</param>
        public SBVHNode(List<Primitive> primitives) {
        }
    }
}
