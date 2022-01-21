using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    public class Scene : Aggregate, IScene {
        /// <summary> The camera in the scene </summary>
        public ICamera Camera { get; }
        /// <summary> The lightsources in the <see cref="Scene"/> </summary>
        public IEnumerable<ISceneObject> Lights { get; }

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="primitives">The primitives in the scene</param>
        public Scene(ICamera camera, List<ISceneObject> primitives) { 
            Camera = camera;
            Lights = primitives.FindAll(s => s is IPrimitive p && p.Material.EmittanceProfile.IsEmitting);
            Stopwatch timer = Stopwatch.StartNew();
            Items.Add(new SpatialBVH(primitives));
            Console.WriteLine(timer.ElapsedMilliseconds + "\t| (S)BVH Building ms");
        }
    }
}