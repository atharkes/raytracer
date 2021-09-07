using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    public class Scene : Aggregate, IScene {
        /// <summary> The camera in the scene </summary>
        public Camera Camera { get; }
        /// <summary> The lightsources in the <see cref="Scene"/> </summary>
        public IEnumerable<ISceneObject> Lights { get; }

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="primitives">The primitives in the scene</param>
        public Scene(IScreen screen, List<ISceneObject> primitives) { 
            Camera = new Camera(screen);
            Lights = primitives.FindAll(p => p.Material.Emitting);
            Stopwatch timer = Stopwatch.StartNew();
            Items.Add(new SBVH(primitives));
            Console.WriteLine(timer.ElapsedMilliseconds + "\t| (S)BVH Building ms");
        }

        /// <summary> Create an empty scene </summary>
        /// <param name="screen">The screen to draw the raytracing</param>
        /// <returns>An empty scene</returns>
        public static Scene Empty(IScreen screen) {
            return new Scene(screen, new List<ISceneObject>());
        }
        
        /// <summary> Create the default scene </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <returns>A default scene</returns>
        public static Scene Default(IScreen screen) {
            return new Scene(screen, DefaultPrimitives);
        }

        /// <summary> The primitives in the default scene </summary>
        public static List<ISceneObject> DefaultPrimitives => new() {
            new Primitive(new AxisAlignedBox(new Vector3(-10, -5, -10), new Vector3(10, 10, 10)) { InwardNormals = true }, Material.WhiteLight),
            new Primitive(new Sphere(new Vector3(-3, 1, 5), 1), Material.DiffuseGreen),
            new Primitive(new Sphere(new Vector3(3, 1, 5), 1), Material.GlossyRed),
            new Primitive(new Sphere(new Vector3(0, 1, 5), 1), Material.Mirror),
            new Primitive(new Sphere(new Vector3(-1, 1, 2), 0.5f), Material.Glass),
            new Primitive(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), null), Material.GlossyPurpleMirror),
            new Primitive(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), null), Material.DiffuseYellow),
        };
    }
}