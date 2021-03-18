using OpenTK.Mathematics;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.Guiding;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneObjects.CameraParts;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;

namespace PathTracer {
    /// <summary> Main class of the raytracer </summary>
    public class Renderer {
        /// <summary> Whether next event estimation is used </summary>
        public bool NextEvenEstimation { get; set; } = false;

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
                Vector3 pixelColor = Sample(ray);
                if (ray is CameraRay cameraRay) {
                    cameraRay.Cavity.AddSample(pixelColor, cameraRay.BVHTraversals, cameraRay.Intersection);
                }
            }
        }

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth of tracing rays</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 Sample(Ray ray) {
            if (ray.RecursionDepth > Ray.MaxRecursionDepth) return Vector3.Zero;

            SurfacePoint? surfacePoint = ray.Trace(Scene);
            if (surfacePoint == null) return Vector3.Zero;
            
            Vector3 irradianceIn = Vector3.Zero;
            ICollection<Ray> samples = Guider.IndirectIllumination(ray, surfacePoint, Utils.Random);
            foreach (Ray sample in samples) {
                irradianceIn += Sample(sample) * Vector3.Dot(sample.Direction, surfacePoint.Normal);
            }

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
            return irradianceIn * surfacePoint.Primitive.Material.Color + surfacePoint.Primitive.GetEmmitance(ray);
        }
    }
}