using OpenTK;
using System.Collections.Generic;

namespace template
{
    class scene
    {
        public List<primitive> primitives;
        public List<lightsource> lights;

        public scene()
        {
            lights = new List<lightsource>();
            lights.Add(new lightsource(new Vector3(0, -10, 3), new Vector3(150, 150, 100)));
            primitives = new List<primitive>();
            primitives.Add(new sphere(new Vector3(-3, 0, 5), 1, new Vector3(0.5f, 0, 0)));
            primitives.Add(new sphere(new Vector3(3, 0, 5), 1, new Vector3(0, 1, 0), 0, 0.5f, 10f));
            primitives.Add(new sphere(new Vector3(0, 0, 5), 1, new Vector3(1f, 1f, 1f), 1f));
            //primitives.Add(new plane(new Vector3(0, -1, 0), -2, new Vector3(1, 1, 1), 0));
            primitives.Add(new triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 0), new Vector3(5, 1, 10), new Vector3(1, 0.8f, 1), 0.4f, 0.7f, 50f));
            primitives.Add(new triangle(new Vector3(-5, 1, 0), new Vector3(5, 1, 10), new Vector3(-5, 1, 10), new Vector3(1, 1, 0.8f), 0, 0.7f, 50f));
        }
    }
}