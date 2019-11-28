using OpenTK;
using System;
using System.Collections.Generic;
using WhittedStyleRaytracer.Raytracing.SceneObjects;
using WhittedStyleRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedStyleRaytracer.Raytracing {
    class Scene {
        public List<Primitive> Primitives;
        public List<Lightsource> Lights;

        readonly Random r = new Random();

        public Scene() {
            Lights = new List<Lightsource>
            {
                new Lightsource(new Vector3(0, -8, 3), new Vector3(300, 300, 100))
            };
            Primitives = new List<Primitive>
            {
                new Sphere(new Vector3(-3, -1, 5), 1, new Vector3(0.5f, 0, 0)),
                new Sphere(new Vector3(3, -1, 5), 1, new Vector3(0, 1, 0), 0, 0.5f, 10f),
                new Sphere(new Vector3(0, -1, 5), 1, new Vector3(1f, 1f, 1f), 1f),
                new Plane(new Vector3(0, -1, 0), -1, new Vector3(1, 1, 1), 0),
                new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), new Vector3(1, 0.8f, 1), 0.4f, 0.7f, 50f),
                new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), new Vector3(1, 1, 0.8f), 0, 0.7f, 50f)
            };
            AddRandomSpeheres(1000);
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