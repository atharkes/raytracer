using OpenTK;
using System;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructures;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedRaytracer.Raytracing {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    class Scene {
        /// <summary> The camera in the scene </summary>
        public readonly Camera Camera;
        /// <summary> The acceleration structure used to find intersections </summary>
        public readonly IAccelerationStructure AccelerationStructure;
        /// <summary> The primitives in the scene </summary>
        public readonly List<Primitive> Primitives = new List<Primitive>();
        /// <summary> The lightsources in the scene </summary>
        public readonly List<PointLight> Lights = new List<PointLight>();

        readonly Random r = new Random();

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        public Scene(IScreen screen) { 
            Camera = new Camera(screen);
            AddDefaultLights();
            AddDefaultPrimitives();
            AddRandomSpeheres(100);
            AccelerationStructure = new BVHNode(Primitives);
        }

        void AddDefaultLights() {
            Lights.Add(new PointLight(new Vector3(0, -8, 3), new Vector3(200, 200, 150)));
        }

        void AddDefaultPrimitives() {
            Primitives.Add(Sphere.DiffuseGreen(new Vector3(-3, -1, 5)));
            Primitives.Add(Sphere.GlossyRed(new Vector3(3, -1, 5)));
            Primitives.Add(Sphere.Mirror(new Vector3(0, -1, 5)));
            Primitives.Add(Sphere.Glass(new Vector3(-1, -1, 2)));
            Primitives.Add(new Plane(new Vector3(0, -1, 0), -1, new Vector3(1, 1, 0.5f)));
            Primitives.Add(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), new Vector3(1, 0.8f, 1), 0.4f, 0, 1, 0.7f, 50f));
            Primitives.Add(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), new Vector3(1, 1, 0.8f), 0, 0, 1, 0.7f, 50f));
        }

        void AddRandomSpeheres(int amount) {
            for (int i = 0; i < amount; i++) {
                Vector3 pos = new Vector3((float)r.NextDouble() * 60f - 30f, (float)r.NextDouble() * 30f - 40f, (float)r.NextDouble() * 60f - 30f);
                float radius = (float)r.NextDouble() * 2f;
                Vector3 color = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                float specularity = r.NextDouble() > 0.5f ? (float)r.NextDouble() : 0;
                float dielectric = r.NextDouble() > 0.5f ? (float)r.NextDouble() : 0;
                float refractionIndex = (float)r.NextDouble() * 2f + 1f;
                float glossyness = r.NextDouble() > 0.5f ? (float)r.NextDouble() : 0;
                float glossSpecularity = (float)r.NextDouble() * 10f;
                Primitives.Add(new Sphere(pos, radius, color, specularity, dielectric, refractionIndex, glossyness, glossSpecularity));
            }
        }

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth if this is a secondary ray</param>
        /// <param name="debugRay">Whether to draw this ray in debug</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 CastRay(Ray ray, int recursionDepth = 0, bool debugRay = false) {
            // Intersect with Scene
            Intersection intersection = AccelerationStructure.Intersect(ray);
            if (intersection == null) return Vector3.Zero;
            ray.Length = intersection.Distance; // Set ray length because BHV doesn't handle it correctly atm
            Vector3 directIllumination = intersection.Primitive.Specularity < 1 ? CastShadowRays(intersection, debugRay) : Vector3.Zero;
            Vector3 outgoingLight;
            
            if (intersection.Primitive.Specularity > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Specular
                Vector3 outReflectedLight = CastRay(intersection.GetReflectedRay(), recursionDepth + 1, debugRay) * intersection.Primitive.Color;
                outgoingLight = directIllumination * (1 - intersection.Primitive.Specularity) + outReflectedLight * intersection.Primitive.Specularity;
            } else if (intersection.Primitive.Dielectric > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Dielectric
                float reflected = intersection.GetReflectivity();
                float refracted = 1 - reflected;
                Ray refractedRay = intersection.GetRefractedRay();
                Vector3 incRefractedLight = refractedRay != null ? CastRay(refractedRay, recursionDepth + 1, debugRay) : Vector3.Zero;
                Vector3 incReflectedLight = CastRay(intersection.GetReflectedRay(), recursionDepth + 1, debugRay);
                outgoingLight = directIllumination * (1f - intersection.Primitive.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * intersection.Primitive.Dielectric * intersection.Primitive.Color;
            } else {
                // Diffuse
                outgoingLight = directIllumination;
            }

            // Debug: Primary Rays
            if (debugRay) {
                Camera.ScreenPlane.DrawRay(ray);
            }
            return outgoingLight;
        }

        /// <summary> Cast shadow rays from an intersection to every light and calculate the color </summary>
        /// <param name="intersection">The intersection to cast the shadow rays from</param>
        /// <param name="debugRay">Whether to draw this ray in debug</param>
        /// <returns>The color at the intersection</returns>
        public Vector3 CastShadowRays(Intersection intersection, bool debugRay = false) {
            Vector3 radianceOut = Vector3.Zero;
            foreach (PointLight light in Lights) {
                Ray shadowRay = intersection.GetShadowRay(light);
                if (AccelerationStructure.IntersectBool(shadowRay)) continue;
                Vector3 radianceIn = light.Color * shadowRay.DistanceAttenuation;
                Vector3 irradiance;
                // N dot L
                float NdotL = Vector3.Dot(intersection.Normal, shadowRay.Direction);
                if (intersection.Primitive.Glossyness > 0) {
                    // Glossy Object: Phong-Shading
                    Vector3 glossyDirection = -shadowRay.Direction - 2 * Vector3.Dot(-shadowRay.Direction, intersection.Normal) * intersection.Normal;
                    float dot = Vector3.Dot(glossyDirection, -intersection.Ray.Direction);
                    if (dot > 0) {
                        float glossyness = (float)Math.Pow(dot, intersection.Primitive.GlossSpecularity);
                        irradiance = radianceIn * ((1 - intersection.Primitive.Glossyness) * NdotL + intersection.Primitive.Glossyness * glossyness);
                    } else {
                        irradiance = radianceIn * (1 - intersection.Primitive.Glossyness) * NdotL;
                    }
                } else {
                    // Diffuse
                    irradiance = radianceIn * NdotL;
                }
                // Absorption
                radianceOut += irradiance * intersection.Primitive.Color;
                if (debugRay) {
                    Camera.ScreenPlane.DrawRay(shadowRay, light.Color.ToIntColor());
                }

            }
            // Triangle Texture
            if (intersection.Primitive is Triangle) {
                if (Math.Abs(intersection.Position.X % 2) < 1) radianceOut = radianceOut * 0.5f;
                if (Math.Abs(intersection.Position.Z % 2) > 1) radianceOut = radianceOut * 0.5f;
            }

            return radianceOut;
        }
    }
}