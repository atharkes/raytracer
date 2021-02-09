using OpenTK.Mathematics;

namespace PathTracer.Raytracing {
    /// <summary> A datastructure to store a ray </summary>
    public class Ray {
        /// <summary> Constant that defines the maximum recursion for secondary rays </summary>
        public const int MaxRecursionDepth = 3;

        /// <summary> The origin of the ray </summary>
        public readonly Vector3 Origin;
        /// <summary> The direction of the ray. This should always be normalized </summary>
        public readonly Vector3 Direction;
        /// <summary> How many bounces this ray made so far </summary>
        public readonly int RecursionDepth;

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public readonly Vector3 DirectionInverted;
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public readonly int[] Sign;

        /// <summary> The length that the ray is travelling </summary>
        public float Length { get; set; }
        /// <summary> The distance attenuation of a point light when this is a shadow ray </summary>
        public float DistanceAttenuation => 1f / (Length * Length);

        /// <summary> Create a new ray using an origin and a direction </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray (it will be normalized)</param>
        /// <param name="length">The length of the ray</param>
        /// <param name="recursionDepth">How manny bounces this ray made so far</param>
        public Ray(Vector3 origin, Vector3 direction, float length = float.MaxValue, int recursionDepth = 0) {
            Origin = origin;
            Direction = direction.Normalized();
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            Length = length;
            Sign = new int[] {
                DirectionInverted.X < 0 ? 1 : 0,
                DirectionInverted.Y < 0 ? 1 : 0,
                DirectionInverted.Z < 0 ? 1 : 0
            };
        }
    }
}