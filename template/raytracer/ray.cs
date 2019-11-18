using OpenTK;

namespace raytracer {
    class Ray {
        public Vector3 Origin;
        public Vector3 Direction;
        public Vector3 DirectionInverted;
        public float Length;
        public float BeginLength = float.MaxValue;

        public Ray(Vector3 origin, Vector3 direction) {
            Origin = origin;
            Direction = direction.Normalized();
            DirectionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            Length = BeginLength;
        }
    }
}