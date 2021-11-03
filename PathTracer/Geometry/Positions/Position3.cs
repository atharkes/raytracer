using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Positions {
    public struct Position3 : IPosition<Vector3>, IEquatable<Position3> {
        /// <summary> The origin of the 3-dimensional coordinate system </summary>
        public static Position3 Origin => new(Vector3.Zero);
        /// <summary> The maximum position </summary>
        public static Position3 MaxValue => new(Vector3.MaxValue);
        /// <summary> The minimum position </summary>
        public static Position3 MinValue => new(Vector3.MinValue);
        /// <summary> The position representing negative infinity </summary>
        public static Position3 NegativeInfinity => new(Vector3.NegativeInfinity);
        /// <summary> The position representing positive infinity </summary>
        public static Position3 PositiveInfinity => new(Vector3.PositiveInfinity);

        public Vector3 Vector { get; }

        public Position1 X => Vector.X;
        public Position1 Y => Vector.Y;
        public Position1 Z => Vector.Z;

        public Position3(Vector3 vector) {
            Vector = vector;
        }

        public Position3(Position1 xyz) {
            Vector = new Vector3(xyz, xyz, xyz);
        }

        public Position3(Position1 x, Position1 y, Position1 z) {
            Vector = new Vector3(x, y, z);
        }

        public static implicit operator Position3(Vector3 vector) => new(vector);

        public static bool operator ==(Position3 left, Position3 right) => left.Equals(right);
        public static bool operator !=(Position3 left, Position3 right) => !(left == right);

        public static Direction3 operator -(Position3 left, Position3 right) => new(left.Vector - right.Vector);

        public static Position3 operator +(Position3 position, IDirection3 direction) => new(position.Vector + direction.Vector);
        public static Position3 operator -(Position3 position, IDirection3 direction) => new(position.Vector - direction.Vector);

        public static Position1 Dot(Position3 position, IDirection3 direction) => new(Vector3.Dot(position.Vector, direction.Vector));
        public static Position3 ComponentMin(Position3 left, Position3 right) => new(Vector3.ComponentMin(left.Vector, right.Vector));
        public static Position3 ComponentMax(Position3 left, Position3 right) => new(Vector3.ComponentMax(left.Vector, right.Vector));

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Position3 other) => Vector.Equals(other.Vector);
        public bool Equals(Position3? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
    }
}
