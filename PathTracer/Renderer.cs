using OpenTK.Mathematics;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.Guiding;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneObjects;
using PathTracer.Pathtracing.SceneObjects.CameraParts;
using PathTracer.Pathtracing.SceneObjects.Primitives;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;

namespace PathTracer {
    /// <summary> Main class of the raytracer </summary>
    public class Renderer {
        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public Scene Scene { get; }
        /// <summary> The path guiding method used to guide random decisions </summary>
        public Pathguider Guider { get; }

        /// <summary> Create a raytracing application </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        public Renderer(IScreen screen) {
            Scene = Scene.Default(screen);
            Guider = new Pathguider(Scene);
        }

        /// <summary> Process a single frame </summary>
        public void Tick() {
            Scene.Camera.Statistics.LogFrameTime();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.OpenTKTime);
            int rayCount = Scene.Camera.RayCountNextTick();
            Scene.Camera.Statistics.LogTickRays(rayCount);
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = rayCount / Program.Threadpool.MultithreadingTaskCount;
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                tasks[i] = () => TraceRays(lowerbound, higherbound);
            }
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.MultithreadingOverhead);
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.TracingTime);
            Scene.Camera.ScreenPlane.Draw();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.DrawingTime);
        }

        void TraceRays(int from, int to) {
            ICollection<Ray> rays = Guider.Samples(to - from, Utils.Random);
            foreach(Ray ray in rays) {
                Vector3 pixelColor = Sample(ray, 0);
                if (ray is CameraRay cameraRay) {
                    cameraRay.Cavity.AddSample(pixelColor, cameraRay.BVHTraversals);
                }
            }
        }

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth of tracing rays</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 Sample(Ray ray, int recursionDepth = 0) {
            if (recursionDepth > Ray.MaxRecursionDepth) {
                return Vector3.Zero;
            }
            // Intersect with Scene
            Intersection? intersection = ray.Trace(Scene);
            if (intersection == null) return Vector3.Zero;
            Vector3 directIllumination = intersection.Primitive.Material.Specularity < 1 ? NextEventEstimation(intersection) : Vector3.Zero;
            Vector3 indirectIllumination = Vector3.Zero;
            ICollection<Ray> samples = Guider.IndirectIllumination(intersection, Utils.Random);
            foreach (Ray sample in samples) {
                float NdotL = Vector3.Dot(intersection.Normal, sample.Direction);
                indirectIllumination += NdotL * intersection.Primitive.Material.Color * Sample(sample, recursionDepth + 1);
            }
            Vector3 incomingLight = directIllumination + indirectIllumination;

            Vector3 radianceOut;
            if (intersection.Primitive.Material.Specularity > 0) {
                // Specular
                Vector3 reflectedIn = Sample(intersection.GetReflectedRay(), recursionDepth + 1);
                Vector3 reflectedOut = reflectedIn * intersection.Primitive.Material.Color;
                radianceOut = incomingLight * (1 - intersection.Primitive.Material.Specularity) + reflectedOut * intersection.Primitive.Material.Specularity;
            } else if (intersection.Primitive.Material.Dielectric > 0) {
                // Dielectric
                float reflected = intersection.GetReflectivity();
                float refracted = 1 - reflected;
                Ray? refractedRay = intersection.GetRefractedRay();
                Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay, recursionDepth + 1) : Vector3.Zero;
                Vector3 incReflectedLight = Sample(intersection.GetReflectedRay(), recursionDepth + 1);
                radianceOut = incomingLight * (1f - intersection.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * intersection.Primitive.Material.Dielectric * intersection.Primitive.Material.Color;
            } else {
                // Diffuse
                radianceOut = incomingLight;
            }
            radianceOut += intersection.Primitive.GetEmmitance(ray);
            return radianceOut;
        }

        /// <summary> Cast shadow rays from an intersection to every light and calculate the color </summary>
        /// <param name="intersection">The intersection to cast the shadow rays from</param>
        /// <returns>The color at the intersection</returns>
        public Vector3 NextEventEstimation(Intersection intersection) {
            Vector3 radianceOut = Vector3.Zero;
            foreach (ShadowRay shadowRay in Guider.NextEventEstimation(intersection, Utils.Random)) {
                float NdotL = Vector3.Dot(intersection.Normal, shadowRay.Direction);
                if (NdotL < 0) {
                    continue;
                }
                Intersection? lightIntersection = shadowRay.Trace(Scene);
                if (lightIntersection == null) {
                    continue;
                }
                Vector3 radianceIn = shadowRay.Light.GetEmmitance(shadowRay);
                Vector3 irradiance = radianceIn * NdotL;
                //if (intersection.Primitive.Material.Glossyness > 0) {
                //    // Glossy Object: Phong-Shading
                //    Vector3 glossyDirection = shadowRay.Direction + 2 * Vector3.Dot(shadowRay.Direction, -intersection.Normal) * intersection.Normal;
                //    float dot = Vector3.Dot(glossyDirection, intersection.Ray.Direction);
                //    if (dot > 0) {
                //        float glossyness = (float)Math.Pow(dot, intersection.Primitive.Material.GlossSpecularity);
                //        irradiance = radianceIn * ((1 - intersection.Primitive.Material.Glossyness) * NdotL + intersection.Primitive.Material.Glossyness * glossyness);
                //    } else {
                //        irradiance = radianceIn * (1 - intersection.Primitive.Material.Glossyness) * NdotL;
                //    }
                //}
                // Absorption
                radianceOut += irradiance * intersection.Primitive.Material.Color;
            }
            return radianceOut;
        }
    }
}