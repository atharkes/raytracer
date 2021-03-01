using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing {
    /// <summary> A datastructure to store a ray </summary>
    public class Ray {
        /// <summary> Constant that defines the maximum recursion for secondary rays </summary>
        public const int MaxRecursionDepth = 8;

        /// <summary> The origin of the ray </summary>
        public Vector3 Origin { get; }
        /// <summary> The direction of the ray. This should always be normalized </summary>
        public Vector3 Direction { get; }
        /// <summary> How many bounces this ray made so far </summary>
        public int RecursionDepth { get; }

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public Vector3 DirectionInverted { get; }
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public Vector3i Sign { get; }

        /// <summary> The length that the ray is travelling </summary>
        public float Length { get; set; }

        /// <summary> Create a new ray using an origin and a direction </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray (it will be normalized)</param>
        /// <param name="length">The length of the ray</param>
        /// <param name="recursionDepth">How manny bounces this ray made so far</param>
        public Ray(Vector3 origin, Vector3 direction, float length = float.MaxValue, int recursionDepth = 0) {
            Origin = origin;
            Direction = direction.Normalized();
            Length = length;
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            Sign = new Vector3i(DirectionInverted.X < 0 ? 1 : 0, DirectionInverted.Y < 0 ? 1 : 0, DirectionInverted.Z < 0 ? 1 : 0);
        }
    }
}