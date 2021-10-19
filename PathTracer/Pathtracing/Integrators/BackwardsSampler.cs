using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Integrators {
    public class BackwardsSampler : Integrator {
        /// <summary> The maximum recursion depth for sampling </summary>
        public int MaxRecursionDepth { get; } = 5;
        /// <summary> The amount of evaluated samples </summary>
        public override int SampleCount { get; } = 0;

        public override void Integrate(IScene scene, TimeSpan integrationTime) {
            int rayCount = ;
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = rayCount / Program.Threadpool.MultithreadingTaskCount;
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                tasks[i] = () => TraceRays(scene, lowerbound, higherbound);
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        void TraceRays(IScene scene, int from, int to) {
            ICollection<CameraRay> rays = scene.Camera.GetCameraRays(to - from, Utils.Random);
            foreach (CameraRay ray in rays) {
                Vector3 pixelColor = Sample(scene, ray);
                ray.Cavity.AddSample(pixelColor, ray.BVHTraversals, ray.Intersection);
            }
        }

        /// <summary> Sample the <paramref name="scene"/> with a <paramref name="sample"/> returning a color found </summary>
        /// <param name="scene">The <see cref="IScene"/> to sample </param>
        /// <param name="sample">The <see cref="ISample"/> to sample the <paramref name="scene"/> with </param>
        /// <returns>The color found for the <see cref="ISample"/></returns>
        public Vector3 Sample(IScene scene, ISample sample) {
            if (sample.RecursionDepth > MaxRecursionDepth) return Vector3.Zero;

            /// Distance Sampling
            IDistanceDistribution? distancePDF = scene.Trace(sample.Ray, );
            if (distancePDF == null) return Vector3.Zero;
            IDistanceMaterial distanceMaterial = distancePDF.SampleWithMaterial(Utils.Random);
            sample.Ray.Length = (float)distanceMaterial.Distance;
            ISurfacePoint surfacePoint = new SurfacePoint();

            /// Direction Sampling
            distanceMaterial.Material.DirectionMediumPDF(sample.Ray.Direction, sample.Color, )

            /// Direct Illumination
            Vector3 outgoingLight = distancePDF.Emittance(sample.Ray);
            /// Indirect Illumination
            ISample incSample = distancePDF.BSDF().Sample(sample, Utils.Random);
            Vector3 incLight = Sample(scene, incSample);
            Vector3 scatteredLight = incLight * distancePDF.Absorption(incSample.Ray);
            Vector3 relevantLight = scatteredLight * Vector3.Dot(incSample.Ray.Direction, distancePDF.Normal);
            outgoingLight += relevantLight;

            return outgoingLight;

            //Vector3 radianceOut;
            //if (surfacePoint.Primitive.Material.Specularity > 0) {
            //    // Specular
            //    Vector3 reflectedIn = Sample(intersection.Reflect());
            //    Vector3 reflectedOut = reflectedIn * intersection.SurfacePoint.Primitive.Material.Color;
            //    radianceOut = irradianceIn * (1 - intersection.SurfacePoint.Primitive.Material.Specularity) + reflectedOut * surfacePoint.Primitive.Material.Specularity;
            //} else if (surfacePoint.Primitive.Material.Dielectric > 0) {
            //    // Dielectric
            //    float reflected = intersection.Reflectivity();
            //    float refracted = 1 - reflected;
            //    Ray? refractedRay = intersection.Refract();
            //    Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay) : Vector3.Zero;
            //    Vector3 incReflectedLight = Sample(intersection.Reflect());
            //    radianceOut = irradianceIn * (1f - surfacePoint.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * surfacePoint.Primitive.Material.Dielectric * surfacePoint.Primitive.Material.Color;
            //} else {
            //    // Diffuse
            //    radianceOut = irradianceIn;
            //}
        }
    }
}
