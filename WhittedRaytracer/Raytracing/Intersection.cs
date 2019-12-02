using OpenTK;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing {
    /// <summary> A data structure to store an intersection between a ray and a primitive </summary>
    class Intersection {
        /// <summary> The ray that intersects the primitive </summary>
        public readonly Ray Ray;
        /// <summary> The primitive that is intersected by the ray </summary>
        public readonly Primitive Primitive;
        /// <summary> The distance from the origin of the ray to the intersection </summary>
        public readonly float Distance;

        /// <summary> The position of the intersection </summary>
        public readonly Vector3 Position;
        /// <summary> The normal of the primitive at the intersection </summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// Epsilon used to raise the intersection away from the primitive.
        /// Used to avoid the intersection falling behind the primitive by rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.00001f;

        /// <summary> Create a new intersection between a ray and a primitive </summary>
        /// <param name="ray">The ray that intersects the primitive</param>
        /// <param name="primitive">The primitive that is intersected by the ray</param>
        /// <param name="position">The position of the intersection</param>
        public Intersection(Ray ray, Primitive primitive, float distance) {
            Ray = ray;
            Ray.Length = distance;
            Primitive = primitive;
            Distance = distance;
            Position = Ray.Origin + Ray.Direction * distance;
            Normal = primitive?.GetNormal(Position) ?? Vector3.Zero;
            Position += Normal * RaiseEpsilon;
        }
    }
}