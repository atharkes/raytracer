using OpenTK.Mathematics;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.SceneObjects;
using PathTracer.Pathtracing.SceneObjects.CameraParts;
using System;

namespace PathTracer {
    /// <summary> Main class of the raytracer </summary>
    public class Renderer {
        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public Scene Scene { get; }

        /// <summary> Create a raytracing application </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        public Renderer(IScreen screen) {
            Scene = Scene.Default(screen);
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
            CameraRay[] rays = Scene.Camera.GetRandomCameraRays(to - from);
            for (int i = 0; i < rays.Length; i++) {
                int x = i % Scene.Camera.ScreenPlane.Screen.Width;
                int y = i / Scene.Camera.ScreenPlane.Screen.Width;
                Vector3 pixelColor = Sample(rays[i], 0);
                rays[i].Cavity.AddPhoton(pixelColor, rays[i].BVHTraversals);
            }
        }

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth of tracing rays</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 Sample(Ray ray, int recursionDepth = 0) {
            // Intersect with Scene
            Intersection? intersection = Scene.Intersect(ray);
            if (intersection == null) return Vector3.Zero;
            Vector3 directIllumination = intersection.Primitive.Material.Specularity < 1 ? NextEventEstimation(intersection) : Vector3.Zero;
            Vector3 radianceOut;

            if (intersection.Primitive.Material.Specularity > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Specular
                Vector3 reflectedIn = Sample(intersection.GetReflectedRay(), recursionDepth + 1);
                Vector3 reflectedOut = reflectedIn * intersection.Primitive.Material.Color;
                radianceOut = directIllumination * (1 - intersection.Primitive.Material.Specularity) + reflectedOut * intersection.Primitive.Material.Specularity;
            } else if (intersection.Primitive.Material.Dielectric > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Dielectric
                float reflected = intersection.GetReflectivity();
                float refracted = 1 - reflected;
                Ray? refractedRay = intersection.GetRefractedRay();
                Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay, recursionDepth + 1) : Vector3.Zero;
                Vector3 incReflectedLight = Sample(intersection.GetReflectedRay(), recursionDepth + 1);
                radianceOut = directIllumination * (1f - intersection.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * intersection.Primitive.Material.Dielectric * intersection.Primitive.Material.Color;
            } else {
                // Diffuse
                radianceOut = directIllumination;
            }

            if (intersection.Primitive.Material.Emitting) {
                radianceOut += intersection.Primitive.Material.EmittingLight / ray.DistanceAttenuation;
            }
            return radianceOut;
        }

        /// <summary> Cast shadow rays from an intersection to every light and calculate the color </summary>
        /// <param name="intersection">The intersection to cast the shadow rays from</param>
        /// <returns>The color at the intersection</returns>
        public Vector3 NextEventEstimation(Intersection intersection) {
            Vector3 radianceOut = Vector3.Zero;
            foreach (Primitive light in Scene.Lights) {
                Ray shadowRay = intersection.GetShadowRay(light);
                if (Vector3.Dot(intersection.Normal, shadowRay.Direction) < 0) continue;
                if (Scene.IntersectBool(shadowRay)) continue;
                Vector3 radianceIn = light.Material.EmittingLight * shadowRay.DistanceAttenuation;
                Vector3 irradiance;
                // N dot L
                float NdotL = Vector3.Dot(intersection.Normal, shadowRay.Direction);
                if (intersection.Primitive.Material.Glossyness > 0) {
                    // Glossy Object: Phong-Shading
                    Vector3 glossyDirection = -shadowRay.Direction - 2 * Vector3.Dot(-shadowRay.Direction, intersection.Normal) * intersection.Normal;
                    float dot = Vector3.Dot(glossyDirection, -intersection.Ray.Direction);
                    if (dot > 0) {
                        float glossyness = (float)Math.Pow(dot, intersection.Primitive.Material.GlossSpecularity);
                        irradiance = radianceIn * ((1 - intersection.Primitive.Material.Glossyness) * NdotL + intersection.Primitive.Material.Glossyness * glossyness);
                    } else {
                        irradiance = radianceIn * (1 - intersection.Primitive.Material.Glossyness) * NdotL;
                    }
                } else {
                    // Diffuse
                    irradiance = radianceIn * NdotL;
                }
                // Absorption
                radianceOut += irradiance * intersection.Primitive.Material.Color;
            }
            return radianceOut;
        }
    }
}