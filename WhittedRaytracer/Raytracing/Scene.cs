using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public readonly ICollection<Primitive> Primitives;
        /// <summary> The lightsources in the scene </summary>
        public readonly ICollection<PointLight> Lights;

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="primitives">The primitives in the scene</param>
        /// <param name="lights">The lights in the scene</param>
        public Scene(IScreen screen, List<Primitive> primitives, ICollection<PointLight> lights) { 
            Camera = new Camera(screen);
            Primitives = primitives;
            Lights = lights;
            Stopwatch timer = Stopwatch.StartNew();
            AccelerationStructure = new BVHNode(primitives);
            Console.WriteLine(timer.ElapsedMilliseconds + "\t| BVH Building");
        }

        /// <summary> Create an empty scene </summary>
        /// <param name="screen">The screen to draw the raytracing</param>
        /// <returns>An empty scene</returns>
        public static Scene Empty(IScreen screen) {
            return new Scene(screen, new List<Primitive>(), new List<PointLight>());
        }
        
        /// <summary> Create the default scene </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <returns>A default scene</returns>
        public static Scene Default(IScreen screen) {
            return new Scene(screen, DefaultPrimitives, DefaultLights);
        }

        /// <summary> Create the default scene with random spheres </summary>
        /// <param name="screen">Te screen to draw the raytracing to</param>
        /// <param name="randomSpheres">The amount of random spheres in the scene</param>
        /// <returns>A default scene with random spheres</returns>
        public static Scene DefaultWithRandomSpheres(IScreen screen, int randomSpheres) {
            List<Primitive> defaultPrimitives = DefaultPrimitives;
            List<Primitive> primitives = new List<Primitive>(defaultPrimitives);
            for (int i = 0; i < randomSpheres; i++) {
                Vector3 spheresCenter = new Vector3(0f, -50f, 0f);
                Vector3 spheresBox = new Vector3(60f, 30f, 60f);
                Vector3 pos = Utils.RandomVector * spheresBox - 0.5f * spheresBox + spheresCenter;
                float radius = (float)Utils.Random.NextDouble();
                primitives.Add(new Sphere(pos, radius));
            }
            return new Scene(screen, primitives, DefaultLights);
        }

        /// <summary> The lights in the default scene </summary>
        public static List<PointLight> DefaultLights => new List<PointLight>() { new PointLight(new Vector3(0, -8, 3), new Vector3(200, 200, 150)) };

        /// <summary> The primitives in the default scene </summary>
        public static List<Primitive> DefaultPrimitives => new List<Primitive>() {
            new Sphere(new Vector3(-3, -1, 5), 1, Material.DiffuseGreen),
            new Sphere(new Vector3(3, -1, 5), 1, Material.GlossyRed),
            new Sphere(new Vector3(0, -1, 5), 1, Material.Mirror),
            new Sphere(new Vector3(-1, -1, 2), 0.5f, Material.Glass),
            //new Plane(new Vector3(0, -1, 0), -1, Material.DiffuseYellow),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), null, Material.GlossyPurpleMirror),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), null, Material.GlossyGreen)
        };

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth if this is a secondary ray</param>
        /// <param name="debugRay">Whether to draw this ray in debug</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 CastRay(Ray ray, int recursionDepth = 0, bool debugRay = false) {
            // Intersect with Scene
            Intersection intersection = AccelerationStructure.Intersect(ray);
            if (intersection == null) return Vector3.Zero;
            Vector3 directIllumination = intersection.Primitive.Material.Specularity < 1 ? CastShadowRays(intersection, debugRay) : Vector3.Zero;
            Vector3 radianceOut;
            
            if (intersection.Primitive.Material.Specularity > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Specular
                Vector3 reflectedIn = CastRay(intersection.GetReflectedRay(), recursionDepth + 1, debugRay);
                Vector3 reflectedOut = reflectedIn * intersection.Primitive.Material.Color;
                radianceOut = directIllumination * (1 - intersection.Primitive.Material.Specularity) + reflectedOut * intersection.Primitive.Material.Specularity;
            } else if (intersection.Primitive.Material.Dielectric > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Dielectric
                float reflected = intersection.GetReflectivity();
                float refracted = 1 - reflected;
                Ray refractedRay = intersection.GetRefractedRay();
                Vector3 incRefractedLight = refractedRay != null ? CastRay(refractedRay, recursionDepth + 1, debugRay) : Vector3.Zero;
                Vector3 incReflectedLight = CastRay(intersection.GetReflectedRay(), recursionDepth + 1, debugRay);
                radianceOut = directIllumination * (1f - intersection.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * intersection.Primitive.Material.Dielectric * intersection.Primitive.Material.Color;
            } else {
                // Diffuse
                radianceOut = directIllumination;
            }

            // Debug: Primary Rays
            if (debugRay) {
                Camera.ScreenPlane.DrawRay(ray);
            }
            return radianceOut;
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