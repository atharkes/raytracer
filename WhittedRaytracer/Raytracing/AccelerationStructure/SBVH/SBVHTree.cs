using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructure.BVH;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructure.SBVH {
    /// <summary> A Spatial Bounding Volume Hierarchy tree </summary>
    public class SBVHTree : BVHTree, IAccelerationStructure {
        /// <summary> Create a spatial BVH </summary>
        /// <param name="primitives">The primitives to build the SBVH with</param>
        public SBVHTree(List<Primitive> primitives) {
            Root = new SBVHNode(primitives);
        }
    }
}
