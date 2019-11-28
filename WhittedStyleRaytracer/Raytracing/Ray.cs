using OpenTK;

namespace WhittedStyleRaytracer.Raytracing {
    /// <summary> A datastructure to store a ray </summary>
    class Ray {
        /// <summary> The origin of the ray </summary>
        public Vector3 Origin { get; }
        /// <summary> The direction of the ray. This should always be normalized </summary>
        public Vector3 Direction { get; }

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public Vector3 DirectionInverted { get; }
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public readonly int[] Sign;

        /// <summary> The length that the ray is travelling </summary>
        public float Length { get; set; }

        /// <summary> Create a new ray using an origin and a direction </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray (it will be normalized)</param>
        /// <param name="length">The length of the ray</param>
        public Ray(Vector3 origin, Vector3 direction, float length = float.MaxValue) {
            Origin = origin;
            Direction = direction.Normalized();
            DirectionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            Length = length;
            Sign = new int[] {
                DirectionInverted.X < 0 ? 1 : 0,
                DirectionInverted.Y < 0 ? 1 : 0,
                DirectionInverted.Z < 0 ? 1 : 0
            };
        }

        /// <summary> Create a new ray using origin and destination </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="destination">The destination of the ray</param>
        /// <returns>A ray with the length from the origin to the destination</returns>
        public static Ray CreateShadowRay(Vector3 origin, Vector3 destination) {
            Vector3 direction = (destination - origin).Normalized();
            float length = (destination - origin).Length;
            return new Ray(origin, direction, length);
        }
    }
}