using OpenTK;

namespace raytracer {
    class Intersection {
        public Vector3 Position;
        public Primitive Primitive;
        public Vector3 Normal;

        readonly float epsilon = 0.00001f;

        public Intersection(Vector3 position, Primitive primitive) {
            Position = position;
            Primitive = primitive;
            Normal = primitive?.GetNormal(position) ?? Vector3.Zero;
            // Raise intersection to not get artifacts by rounding errors
            Position += Normal * epsilon;
        }
    }
}