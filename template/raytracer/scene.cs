using OpenTK;
using System;
using System.Collections.Generic;

namespace raytracer {
    class Scene {
        public List<Primitive> Primitives;
        public List<Lightsource> Lights;

        readonly Random r = new Random();

        public Scene() {
            Lights = new List<Lightsource>
            {
                new Lightsource(new Vector3(0, -10, 3), new Vector3(300, 300, 100))
            };
            Primitives = new List<Primitive>
            {
                new Sphere(new Vector3(-3, 0, 5), 1, new Vector3(0.5f, 0, 0)),
                new Sphere(new Vector3(3, 0, 5), 1, new Vector3(0, 1, 0), 0, 0.5f, 10f),
                new Sphere(new Vector3(0, 0, 5), 1, new Vector3(1f, 1f, 1f), 1f),
                //new Plane(new Vector3(0, -1, 0), -2, new Vector3(1, 1, 1), 0),
                new Triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 0), new Vector3(5, 1, 10), new Vector3(1, 0.8f, 1), 0.4f, 0.7f, 50f),
                new Triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 10), new Vector3(-5, 1, 10), new Vector3(1, 1, 0.8f), 0, 0.7f, 50f)
            };
            AddRandomSpeheres(1000);
        }

        void AddRandomSpeheres(int amount) {
            for (int i = 0; i < amount; i++) {
                Vector3 pos = new Vector3(r.Next(-30, 30), r.Next(-30, -12), r.Next(-30, 30));
                Vector3 color = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                Primitives.Add(new Sphere(pos, (float)r.NextDouble(), color, (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()));
            }
        }
    }
}