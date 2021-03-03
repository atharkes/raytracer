using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;

namespace PathTracer.Pathtracing.Rays {
    /// <summary> A ray that is connected to a light source </summary>
    public class LightRay : Ray {
        /// <summary> The light connected to this ray </summary>
        public Primitive Light { get; }

        /// <summary> Create a ray from a light </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="light">The light connected to this ray</param>
        public LightRay(Vector3 origin, Vector3 direction, Primitive light, float length = float.MaxValue) : base(origin, direction, length, 0) {
            Light = light;
        }

        protected LightRay(Vector3 origin, Vector3 destination, Primitive light, int recursionDepth) : base(origin, destination, recursionDepth) {
            Light = light;
        }
    }
}
