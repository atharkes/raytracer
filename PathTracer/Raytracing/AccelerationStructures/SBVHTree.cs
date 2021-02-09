using System.Collections.Generic;
using PathTracer.Raytracing.AccelerationStructures.SBVH;
using PathTracer.Raytracing.SceneObjects;

namespace PathTracer.Raytracing.AccelerationStructures {
    /// <summary> A Split Bounding Volume Hierarchy tree 
    /// Possible Additions:
    /// - Check for each fragments if it is better to move to either side of the split
    /// - Enable Triangle and/or Polygon clipping to decrease AABB sizes
    /// - Add an alpha to promote spatial splits closer to the root
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
