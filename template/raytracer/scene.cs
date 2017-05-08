using OpenTK;
using System.Collections.Generic;

namespace template {
    class Scene {
        public List<Primitive> primitives;
        public List<Lightsource> lights;

        public Scene() {
            lights = new List<Lightsource>();
            lights.Add(new Lightsource(new Vector3(0, -10, 3), new Vector3(150, 150, 100)));
            primitives = new List<Primitive>();
            primitives.Add(new Sphere(new Vector3(-3, 0, 5), 1, new Vector3(0.5f, 0, 0)));
            primitives.Add(new Sphere(new Vector3(3, 0, 5), 1, new Vector3(0, 1, 0), 0, 0.5f, 10f));
            primitives.Add(new Sphere(new Vector3(0, 0, 5), 1, new Vector3(1f, 1f, 1f), 1f));
            //primitives.Add(new plane(new Vector3(0, -1, 0), -2, new Vector3(1, 1, 1), 0));
            primitives.Add(new Triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 0), new Vector3(5, 1, 10), new Vector3(1, 0.8f, 1), 0.4f, 0.7f, 50f));
            primitives.Add(new Triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 10), new Vector3(-5, 1, 10), new Vector3(1, 1, 0.8f), 0, 0.7f, 50f));
        }
    }
}