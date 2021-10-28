using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions {
    public struct Direction3 : IDirection3 {
        /// <summary> The <see cref="Vector3"/> value used for this <see cref="IDirection3"/> </summary>
        public Vector3 Vector { get; }

        /// <summary> The X-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 X => Vector.X;
        /// <summary> The Y-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 Y => Vector.Y;
        /// <summary> The Z-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 Z => Vector.Z;
        /// <summary> The length of the <see cref="IDirection3"/> </summary>
        public Position1 Length => Vector.Length;
        /// <summary> The squared length of the <see cref="IDirection3"/> </summary>
        public Position1 LengthSquared => Vector.LengthSquared;

        public Direction3(Vector3 vector) {
            Vector = vector;
        }

        public Direction3(Direction1 xyz) {
            Vector = new Vector3(xyz, xyz, xyz);
        }

        public Direction3(Direction1 x, Direction1 y, Direction1 z) {
            Vector = new Vector3(x, y, z);
        }

        public static implicit operator Direction3(Vector3 vector) => new(vector);
        public static implicit operator Direction3((Vector1 X, Vector1 Y, Vector1 Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);
        public static implicit operator Direction3(Normal3 normal) => new(normal.Vector);

        public static explicit operator Direction3(Position3 position) => new(position.Vector);
        public static explicit operator Position3(Direction3 position) => new(position.Vector);

        public static Direction3 operator +(Direction3 left, Direction3 right) => new(left.Vector + right.Vector);
        public static Direction3 operator -(Direction3 direction) => new(-direction.Vector);
        public static Direction3 operator -(Direction3 left, Direction3 right) => new(left.Vector - right.Vector);
        public static Direction3 operator *(Direction3 direction, Position1 scale) => new(direction.Vector * scale.Vector);
        public static Direction3 operator *(Direction3 direction, Position3 scale) => new(direction.Vector * scale.Vector);
        public static Direction3 operator /(Direction3 direction, Position1 scale) => new(direction.Vector / scale.Vector);
        public static Direction3 operator /(Position1 scale, Direction3 direction) => scale.Vector / direction.Vector;
        public static Direction3 operator /(Direction3 direction, Position3 scale) => new(direction.Vector / scale.Vector);

        public static Direction1 Dot(IDirection3 left, IDirection3 right) => new(Vector3.Dot(left.Vector, right.Vector));
        public static Direction3 Lerp(IDirection3 a, IDirection3 b, Vector1 blend) => Vector3.Lerp(a.Vector, b.Vector, blend);

        public Normal3 Normalized() => new(Vector);
    }
}
