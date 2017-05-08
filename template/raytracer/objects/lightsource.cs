using OpenTK;

namespace template {
    class Lightsource {
        public Vector3 position;
        public Vector3 color;

        public Lightsource(Vector3 position, Vector3 color) {
            this.position = position;
            this.color = color;
        }
    }
}