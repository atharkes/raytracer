using OpenTK.Mathematics;
using PathTracer.Pathtracing.AccelerationStructures;
using PathTracer.Pathtracing.SceneObjects;
using PathTracer.Pathtracing.SceneObjects.CameraParts;
using PathTracer.Pathtracing.SceneObjects.Primitives;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathTracer.Pathtracing {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    public class Scene {
        /// <summary> The camera in the scene </summary>
        public Camera Camera { get; }
        /// <summary> The acceleration structure used to find intersections </summary>
        public IAccelerationStructure AccelerationStructure { get; }
        /// <summary> The primitives in the scene </summary>
        public ICollection<Primitive> Primitives { get; }
        /// <summary> The lightsources in the scene </summary>
        public ICollection<Primitive> Lights { get; }

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="primitives">The primitives in the scene</param>
        public Scene(IScreen screen, List<Primitive> primitives) { 
            Camera = new Camera(screen);
            Primitives = primitives;
            Lights = primitives.FindAll(p => p.Material.Emitting);
            Stopwatch timer = Stopwatch.StartNew();
            AccelerationStructure = new SBVHTree(primitives);
            Console.WriteLine(timer.ElapsedMilliseconds + "\t| (S)BVH Building ms");
        }

        /// <summary> Create an empty scene </summary>
        /// <param name="screen">The screen to draw the raytracing</param>
        /// <returns>An empty scene</returns>
        public static Scene Empty(IScreen screen) {
            return new Scene(screen, new List<Primitive>());
        }
        
        /// <summary> Create the default scene </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <returns>A default scene</returns>
        public static Scene Default(IScreen screen) {
            return new Scene(screen, DefaultPrimitives);
        }

        /// <summary> Create the default scene with random spheres </summary>
        /// <param name="screen">Te screen to draw the raytracing to</param>
        /// <param name="randomSpheres">The amount of random spheres in the scene</param>
        /// <returns>A default scene with random spheres</returns>
        public static Scene DefaultWithRandomSpheres(IScreen screen, int randomSpheres) {
            List<Primitive> defaultPrimitives = DefaultPrimitives;
            List<Primitive> primitives = new List<Primitive>(defaultPrimitives);
            for (int i = 0; i < randomSpheres; i++) {
                Vector3 spheresCenter = new Vector3(0f, -30f, 0f);
                Vector3 spheresBox = new Vector3(60f, 30f, 60f);
                Vector3 pos = Utils.DetRandom.Vector() * spheresBox - 0.5f * spheresBox + spheresCenter;
                float radius = (float)Utils.DetRandom.NextDouble();
                primitives.Add(new Sphere(pos, radius));
            }
            return new Scene(screen, primitives);
        }

        /// <summary> Create the default scene with random spheres </summary>
        /// <param name="screen">Te screen to draw the raytracing to</param>
        /// <param name="randomTriangles">The amount of random triangles in the scene</param>
        /// <returns>A default scene with random spheres</returns>
        public static Scene DefaultWithRandomTriangles(IScreen screen, int randomTriangles) {
            List<Primitive> defaultPrimitives = DefaultPrimitives;
            List<Primitive> primitives = new(defaultPrimitives);
            for (int i = 0; i < randomTriangles; i++) {
                Vector3 trianglesCenter = new(0f, -30f, 0f);
                Vector3 trianglesBox = new(60f, 30f, 60f);
                Vector3 p1 = Utils.DetRandom.Vector() * trianglesBox - 0.5f * trianglesBox + trianglesCenter;
                Vector3 p2 = p1 + Utils.DetRandom.Vector(4f);
                Vector3 p3 = p1 - Utils.DetRandom.Vector(4f);
                primitives.Add(new Triangle(p1, p2, p3));
            }
            return new Scene(screen, primitives);
        }

        /// <summary> The primitives in the default scene </summary>
        public static List<Primitive> DefaultPrimitives => new() {
            new AxisAlignedBox(new Vector3(-10, -5, -10), new Vector3(10, -1, 10)),
            new Sphere(new Vector3(-3, 1, 5), 1, Material.DiffuseGreen),
            new Sphere(new Vector3(3, 1, 5), 1, Material.GlossyRed),
            new Sphere(new Vector3(0, 1, 5), 1, Material.Mirror),
            new Sphere(new Vector3(-1, 1, 2), 0.5f, Material.Glass),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), null, Material.GlossyPurpleMirror),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), null, Material.GlossyGreen),
            new PointLight(new Vector3(0, 8, 3), new Vector3(1, 1, 0.75f), 200),
            //new Sphere(new Vector3(0, -8, 3), 0.5f, new Material(50, new Vector3(1, 1, 0.75f)))
        };

        /// <summary> Intersect the <see cref="Scene"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Scene"/> with</param>
        /// <returns>The <see cref="Intersection"/> if there is any</returns>
        public Intersection? Intersect(Ray ray) {
            return AccelerationStructure.Intersect(ray);
        }

        /// <summary> Intersect the <see cref="Scene"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Scene"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects something in the <see cref="Scene"/></returns>
        public bool IntersectBool(Ray ray) {
            return AccelerationStructure.IntersectBool(ray);
        }
    }
}