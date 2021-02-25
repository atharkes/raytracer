using OpenTK.Mathematics;
using PathTracer.Pathtracing.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneObjects;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.AccelerationStructures.SBVH {
    /// <summary> A bin for spatial splitting </summary>
    public class SpatialBin {
        /// <summary> The AABB of this bin </summary>
        public AABB AABB { get; } = new AABB();
        /// <summary> The clipping plane on the left </summary>
        public AxisAlignedPlane SplitPlaneLeft { get; }
        /// <summary> The clipping plane on the right </summary>
        public AxisAlignedPlane SplitPlaneRight { get; }
        /// <summary> The fragments in this spatial bin </summary>
        public List<PrimitiveFragment> Fragments { get; } = new List<PrimitiveFragment>();

        /// <summary> Create a spatial bin </summary>
        /// <param name="direction">The direction that the bins are ordered in</param>
        /// <param name="start">The starting point of this bin in the specified direction</param>
        /// <param name="end">The ending point of this bin in the specified directio</param>
        public SpatialBin(Vector3 direction, float start, float end) {
            SplitPlaneLeft = new AxisAlignedPlane(direction, direction * start);
            SplitPlaneRight = new AxisAlignedPlane(-direction, direction * end);
        }

        /// <summary> Add a primitive to the spatial bin and clip it </summary>
        /// <param name="primitive">The primitive to add to the bin</param>
        public void ClipAndAdd(Primitive primitive) {
            PrimitiveFragment? fragment = primitive.Clip(SplitPlaneLeft)?.Clip(SplitPlaneRight);
            if (fragment == null) return;
            Fragments.Add(fragment);
            AABB.Add(fragment);
        }
    }
}