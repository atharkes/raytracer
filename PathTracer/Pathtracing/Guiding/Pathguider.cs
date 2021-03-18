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

        public ICollection<Ray> Sample(Ray incomingRay, SurfacePoint surfacePoint, Random random) {
            throw new NotImplementedException();
        }

        public ICollection<Ray> IndirectIllumination(Ray incomingRay, SurfacePoint surfacePoint, Random random) {
            List<Ray> rays = new();
            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();
            float phi = 2f * (float)Math.PI * r1;
            float z = (float)Math.Sqrt(1 - r2 * r2);
            Vector3 direction = new((float)Math.Cos(phi) * z, (float)Math.Sin(phi) * z, r2);
            rays.Add(new Ray(surfacePoint, direction, float.MaxValue, incomingRay.RecursionDepth + 1));
            return rays;
        }

        public ICollection<ShadowRay> NextEventEstimation(Ray incomingRay, SurfacePoint surfacePoint, Random random) {
            List<ShadowRay> rays = new();
            foreach (Primitive lightsource in Scene.Lights) {
                Vector3 point = lightsource.PointOnSurface(random);
                Vector3 normal = lightsource.GetNormal(point);
                SurfacePoint destination = new(lightsource, point, normal);
                ShadowRay shadowray = new(surfacePoint, destination, incomingRay.RecursionDepth + 1);
                rays.Add(shadowray);
            }
            return rays;
        }
    }
}
