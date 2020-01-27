using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructures.SBVH;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures {
    /// <summary> A Split Bounding Volume Hierarchy tree 
    /// Possible Additions:
    /// - Check for each fragments if it is better to move to either side of the split
    /// - Enable Triangle and/or Polygon clipping to decrease AABB sizes
    /// </summary>
    public class SBVHTree : BVHTree, IAccelerationStructure {
        /// <summary> The amount of bins used for spatial binning </summary>
        public const int SpatialBinAmount = 256;

        /// <summary> Create a spatial BVH </summary>
        /// <param name="primitives">The primitives to build the SBVH with</param>
        public SBVHTree(List<Primitive> primitives) {
            Root = new SBVHNode(primitives);
        }
    }
}
