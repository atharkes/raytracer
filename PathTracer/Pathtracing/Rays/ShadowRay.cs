using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Rays {
    public class ShadowRay : LightRay {
        public ShadowRay(Vector3 origin, Vector3 direction, Primitive light, int recursionDepth = 0) : base(origin, light.Position - origin, light, recursionDepth) {

        }

        /// <summary> Trace the <see cref="Ray"/> through a <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to trace through</param>
        /// <returns>An <see cref="Intersection"/> if there is one</returns>
        public override Intersection? Trace(Scene scene) {
            if (!scene.IntersectBool(this)) {
                Intersection? intersection = Light.Intersect(this);
                Debug.Assert(intersection != null, "Shadowray didn't hit it's destined lightsource even though it is not occluded.");
                return intersection;
            } else {
                return null;
            }
        }
    }
}
