using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Positions {
    public struct Position1 : IPosition<Vector1>, IComparable<Position1>, IEquatable<Position1> {
        /// <summary> The origin of the 3-dimensional coordinate system </summary>
        public static Position1 Origin => new(Vector1.Zero);
        /// <summary> The position representing negative infinity </summary>
        public static Position1 NegativeInfinity => new(Vector1.NegativeInfinity);
        /// <summary> The position representing positive infinity </summary>
        public static Position1 PositiveInfinity => new(Vector1.PositiveInfinity);

        public Vector1 Vector { get; }

        public Position1 X => Vector;

        public Position1(Vector1 value) {
            Vector = value;
        }

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Position1 other) => Vector.Equals(other.Vector);
        public bool Equals(Position1? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public int CompareTo(Position1 other) => Vector.CompareTo(other.Vector);

        public static implicit operator Position1(float value) => new(value);
        public static implicit operator float(Position1 value) => value.Vector.Value;
        public static implicit operator Position1(Vector1 vector) => new(vector);

        public static explicit operator Position1(Direction1 direction) => new(direction.Vector);
        public static explicit operator Position1(Normal1 normal) => new(normal.Vector);

        public static bool operator <(Position1 left, Position1 right) => left.CompareTo(right) < 0;
        public static bool operator >(Position1 left, Position1 right) => left.CompareTo(right) > 0;
        public static bool operator <=(Position1 left, Position1 right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Position1 left, Position1 right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Position1 left, Position1 right) => left.Equals(right);
        public static bool operator !=(Position1 left, Position1 right) => !left.Equals(right);

        public static Direction1 operator -(Position1 left, Position1 right) => new(left.Vector - right.Vector);
        public static Position1 operator +(Position1 position, Direction1 direction) => new(position.Vector + direction.Vector);

        public static Position1 Dot(Position1 position, IDirection1 direction) => new(Vector1.Dot(position.Vector, direction.Vector));
        public static Position1 ComponentMin(Position1 left, Position1 right) => new(Vector1.ComponentMin(left.Vector, right.Vector));
        public static Position1 ComponentMax(Position1 left, Position1 right) => new(Vector1.ComponentMax(left.Vector, right.Vector));
    }
}
