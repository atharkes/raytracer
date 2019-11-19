using OpenTK;

namespace Raytracer.Objects {
    class Lightsource {
        public Vector3 Position;
        public Vector3 Color;

        public Lightsource(Vector3 position, Vector3 color) {
            Position = position;
            Color = color;
        }
    }
}