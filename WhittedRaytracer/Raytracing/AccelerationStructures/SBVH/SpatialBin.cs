using OpenTK;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructures.BVH;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures.SBVH {
    /// <summary> A bin for spatial splitting </summary>
    public class SpatialBin {
        /// <summary> The AABB of this bin </summary>
        public AABB AABB { get; } = new AABB();
        /// <summary> The clipping plane on the left </summary>
        public SplitPlane Left { get; }
        /// <summary> The clipping plane on the right </summary>
        public SplitPlane Right { get; }
        /// <summary> The fragments in this spatial bin </summary>
        public List<Fragment> Fragments { get; } = new List<Fragment>();

        /// <summary> Create a spatial bin </summary>
        /// <param name="direction">The direction that the bins are ordered in</param>
        /// <param name="start">The starting point of this bin in the specified direction</param>
        /// <param name="end">The ending point of this bin in the specified directio</param>
        public SpatialBin(Vector3 direction, float start, float end) {
            Left = new SplitPlane(direction, direction * start);
            Right = new SplitPlane(-direction, direction * end);
        }

        /// <summary> Add a primitive to the spatial bin and clip it </summary>
        /// <param name="primitive">The primitive to add to the bin</param>
        public void ClipAndAdd(Primitive primitive) {
            Fragment fragment = primitive.Clip(Left)?.Clip(Right);
            if (fragment == null) return;
            Fragments.Add(fragment);
            AABB.Add(fragment);
        }
    }
}