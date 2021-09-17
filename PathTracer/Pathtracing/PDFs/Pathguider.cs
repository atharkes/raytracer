using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.PDFs {
    public class Pathguider {
        /// <summary> Create a new path guider </summary>
        public Pathguider() { }

        public ICollection<Ray> Samples(Scene scene, int amount, Random random) {
            return scene.Camera.GetCameraRays(amount, random);
        }

        public ICollection<Ray> Sample(IRay incomingRay, SurfacePoint surfacePoint, Random random) {
            throw new NotImplementedException();
        }

        public ICollection<Ray> IndirectIllumination(IRay incomingRay, ISurfacePoint point, Random random) {
            List<Ray> rays = new();
            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();
            float phi = 2f * (float)Math.PI * r1;
            float z = (float)Math.Sqrt(1 - r2 * r2);
            Vector3 direction = new((float)Math.Cos(phi) * z, (float)Math.Sin(phi) * z, r2);
            rays.Add(new Ray(point, direction, float.MaxValue, incomingRay.RecursionDepth + 1));
            return rays;
        }

        public ICollection<ShadowRay> NextEventEstimation(IEnumerable<ISceneObject> lights, IRay incomingRay, ISurfacePoint point, Random random) {
            List<ShadowRay> rays = new();
            foreach (ISceneObject lightsource in lights) {
                Vector3 point = lightsource.PointOnSurface(random);
                Vector3 normal = lightsource.SurfaceNormal(point);
                SurfacePoint destination = new(lightsource, point, normal);
                ShadowRay shadowray = new(point, destination, incomingRay.RecursionDepth + 1);
                rays.Add(shadowray);
            }
            return rays;
        }
    }
}
