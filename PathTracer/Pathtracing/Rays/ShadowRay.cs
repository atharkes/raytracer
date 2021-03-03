using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;

namespace PathTracer.Pathtracing.Rays {
    public class ShadowRay : LightRay {
        /// <summary> The intersection at the light being checked for occlusion </summary>
        public Interaction Intersection { get; }

        public ShadowRay(Vector3 origin, Vector3 destination, Primitive light, int recursionDepth = 0) : base(origin, destination, light, recursionDepth) { }

        /// <summary> Trace the <see cref="Ray"/> through a <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to trace through</param>
        /// <returns>An <see cref="Intersection"/> if there is one</returns>
        public override Interaction? Trace(Scene scene) {
            if (!scene.IntersectBool(this)) {
                return new Interaction(this, Length, Light);
            } else {
                return null;
            }
        }
    }
}
