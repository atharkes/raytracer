using OpenTK.Mathematics;

namespace PathTracer.Pathtracing {
    /// <summary> A <see cref="Ray"/> used for tracing through the <see cref="Scene"/> </summary>
    public class Ray {
        /// <summary> The maximum recursion depth of traced paths </summary>
        public static int MaxRecursionDepth { get; set; } = 3;

        /// <summary> The origin of the <see cref="Ray"/> </summary>
        public Vector3 Origin { get; }
        /// <summary> The normalized direction of the <see cref="Ray"/> </summary>
        public Vector3 Direction { get; }
        /// <summary> The length the <see cref="Ray"/> is travelling </summary>
        public float Length { get; set; }
        /// <summary> The destination of the <see cref="Ray"/> </summary>
        public Vector3 Destination => Direction * Length;
        /// <summary> The recursion depth of the traced path so far </summary>
        public int RecursionDepth { get; }

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public Vector3 DirectionInverted { get; }
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public Vector3i Sign { get; }

        /// <summary> Create a new <see cref="Ray"/> using an <paramref name="origin"/> and <paramref name="direction"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="direction">The direction of the <see cref="Ray"/></param>
        /// <param name="length">The length of the <see cref="Ray"/></param>
        /// <param name="recursionDepth">The recursion depth of the path so farr</param>
        public Ray(Vector3 origin, Vector3 direction, float length = float.MaxValue, int recursionDepth = 0) {
            Origin = origin;
            Direction = direction.Normalized();
            Length = length;
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / Direction.X, 1 / Direction.Y, 1 / Direction.Z);
            Sign = new Vector3i(DirectionInverted.X < 0 ? 1 : 0, DirectionInverted.Y < 0 ? 1 : 0, DirectionInverted.Z < 0 ? 1 : 0);
        }

        /// <summary> Create a new <see cref="Ray"/> using a <paramref name="origin"/> and <paramref name="destination"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="destination">The destination of the <see cref="Ray"/></param>
        /// <param name="recursionDepth">The recursion depth of the path so far</param>
        public Ray(Vector3 origin, Vector3 destination, int recursionDepth = 0) {
            Origin = origin;
            Vector3 direction = destination - origin;
            Direction = direction.Normalized();
            Length = direction.Length;
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            Sign = new Vector3i(DirectionInverted.X < 0 ? 1 : 0, DirectionInverted.Y < 0 ? 1 : 0, DirectionInverted.Z < 0 ? 1 : 0);
        }

        /// <summary> Trace the <see cref="Ray"/> through a <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to trace through</param>
        /// <returns>An <see cref="Intersection"/> if there is one</returns>
        public virtual Intersection? Trace(Scene scene) {
            Intersection? intersection = scene.Intersect(this);
            if (intersection != null) {
                Length = intersection.Distance;
                return new Intersection(this, intersection.Distance, intersection.Primitive);
            } else {
                return null;
            }
        }

        /// <summary> Check whether the specified <paramref name="distance"/> is within the ray interval </summary>
        /// <param name="distance">The specified distance to check</param>
        /// <returns>Whether the <paramref name="distance"/> falls in the ray interval</returns>
        public bool WithinBounds(float distance) {
            return 0 < distance && distance < Length;
        }
    }
}