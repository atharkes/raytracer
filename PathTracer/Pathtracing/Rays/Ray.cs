using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Pathtracing.Rays {
    /// <summary> A <see cref="Ray"/> used for tracing through the <see cref="IScene"/> </summary>
    public class Ray : IRay {
        /// <summary> The origin of the <see cref="Ray"/> </summary>
        public Position3 Origin { get; }
        /// <summary> The normalized direction of the <see cref="Ray"/> </summary>
        public Normal3 Direction { get; }
        /// <summary> The distance the <see cref="Ray"/> is travelling </summary>
        public Position1 Length { get => length; set => length = value; }

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public Vector3 InvDirection { get; }
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public OpenTK.Mathematics.Vector3i Sign { get; }

        volatile float length;

        /// <summary> Create a <see cref="Ray"/> at a specified <paramref name="origin"/> going in a specified <paramref name="direction"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="direction">The direction of the <see cref="Ray"/></param>
        /// <param name="length">The length the <see cref="Ray"/> will try to travel</param>
        /// <param name="recursionDepth">The recursion depth of the <see cref="Ray"/></param>
        public Ray(Position3 origin, Normal3 direction, float length = float.PositiveInfinity) {
            Origin = origin;
            Direction = direction;
            Length = length;
            InvDirection = new Vector3(1 / (float)Direction.X, 1f / (float)Direction.Y, 1f / (float)Direction.Z);
            Sign = new OpenTK.Mathematics.Vector3i(InvDirection.X < 0 ? 1 : 0, InvDirection.Y < 0 ? 1 : 0, InvDirection.Z < 0 ? 1 : 0);
        }
    }
}