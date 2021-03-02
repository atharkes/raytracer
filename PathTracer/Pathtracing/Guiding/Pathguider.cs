using OpenTK.Mathematics;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneObjects;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Guiding {
    public class Pathguider {
        /// <summary> The <see cref="Scene"/> to guide paths for </summary>
        public Scene Scene { get; }

        /// <summary> Create a new path guider for the specified <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to guide paths for</param>
        public Pathguider(Scene scene) {
            Scene = scene;
        }

        public ICollection<Ray> Samples(int amount, Random random) {
            return Scene.Camera.GetCameraRays(amount, random);
        }

        public ICollection<Ray> IndirectIllumination(Intersection intersection, Random random) {
            List<Ray> rays = new List<Ray>();
            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();
            float phi = 2f * (float)Math.PI * r1;
            float z = (float)Math.Sqrt(1 - r2 * r2);
            Vector3 direction = new Vector3((float)Math.Cos(phi) * z, (float)Math.Sin(phi) * z, r2);
            rays.Add(new Ray(intersection.Position, direction, float.MaxValue, intersection.Ray.RecursionDepth + 1));
            return rays;
        }

        public ICollection<ShadowRay> NextEventEstimation(Intersection intersection, Random random) {
            List<ShadowRay> rays = new List<ShadowRay>();
            foreach (Primitive lightsource in Scene.Lights) {
                rays.Add(new ShadowRay(intersection.Position, lightsource.Position - intersection.Position, lightsource, intersection.Ray.RecursionDepth + 1));
            }
            return rays;
        }
    }
}
