using OpenTK;

namespace template {
    class Intersection {
        public Vector3 position;
        public Primitive primitive;

        public Intersection(Vector3 position, Primitive primitive) {
            this.position = position;
            this.primitive = primitive;
        }
    }
}