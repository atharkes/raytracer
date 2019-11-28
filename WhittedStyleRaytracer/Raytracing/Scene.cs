using OpenTK;
using System;
using System.Collections.Generic;
using WhittedStyleRaytracer.Raytracing.SceneObjects;
using WhittedStyleRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedStyleRaytracer.Raytracing {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    class Scene {
        /// <summary> The camera in the scene </summary>
        public readonly Camera Camera;
        /// <summary> The acceleration structure used to find intersections </summary>
        public readonly BVHNode AccelerationStructure;
        /// <summary> The primitives in the scene </summary>
        public readonly List<Primitive> Primitives = new List<Primitive>();
        /// <summary> The lightsources in the scene </summary>
        public readonly List<Lightsource> Lights = new List<Lightsource>();

        readonly Random r = new Random();

        /// <summary> Create a new scene with some default objects </summary>
        public Scene(IScreen screen) {
            Camera = new Camera(screen);
            AddDefaultLights();
            AddDefaultPrimitives();
            AddRandomSpeheres(1000);
            AccelerationStructure = new BVHNode(Primitives);
        }

        void AddDefaultLights() {
            Lights.Add(new Lightsource(new Vector3(0, -8, 3), new Vector3(300, 300, 100)));
        }

        void AddDefaultPrimitives() {
            Primitives.Add(new Sphere(new Vector3(-3, -1, 5), 1, new Vector3(0.5f, 0, 0)));
            Primitives.Add(new Sphere(new Vector3(3, -1, 5), 1, new Vector3(0, 1, 0), 0, 0.5f, 10f));
            Primitives.Add(new Sphere(new Vector3(0, -1, 5), 1, new Vector3(1f, 1f, 1f), 1f));
            Primitives.Add(new Plane(new Vector3(0, -1, 0), -1, new Vector3(1, 1, 1), 0));
            Primitives.Add(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), new Vector3(1, 0.8f, 1), 0.4f, 0.7f, 50f));
            Primitives.Add(new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), new Vector3(1, 1, 0.8f), 0, 0.7f, 50f));
        }

        void AddRandomSpeheres(int amount) {
            for (int i = 0; i < amount; i++) {
                Vector3 pos = new Vector3((float)r.NextDouble() * 60 - 30, (float)r.NextDouble() * 30 - 40, (float)r.NextDouble() * 60 - 30);
                Vector3 color = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                Primitives.Add(new Sphere(pos, (float)r.NextDouble(), color, (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()));
            }
        }
    }
}