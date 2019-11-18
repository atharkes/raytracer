using OpenTK;

namespace template {
    class Intersection {
        public Vector3 Position;
        public Primitive Primitive;

        public Intersection(Vector3 position, Primitive primitive) {
            Position = position;
            Primitive = primitive;
        }
    }
}