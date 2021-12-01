using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Directions {
    /// <summary> A 1-dimensional direction vector </summary>
    public struct Direction1 : IDirection1, IEquatable<Direction1> {
        /// <summary> The direction vector reprsenting no direction </summary>
        public static readonly Direction1 Zero = Vector1.Zero;
        /// <summary> The direction vector with 1 in every direction </summary>
        public static readonly Direction1 One = Vector1.One;

        public Vector1 Vector { get; }

        /// <summary> The X-coordinate of the <see cref="IDirection1"/> </summary>
        public Direction1 X => Vector.X;

        public Direction1(Vector1 value) {
            Vector = value;
        }

        public static implicit operator Direction1(float value) => new(value);
        public static implicit operator float(Direction1 value) => value.Vector.Value;
        public static implicit operator Direction1(Vector1 value) => new(value);
        public static implicit operator Direction1(Normal1 normal) => new(normal.Vector);

        public static bool operator ==(Direction1 left, Direction1 right) => left.Equals(right);
        public static bool operator !=(Direction1 left, Direction1 right) => !(left == right);

        public static bool operator <=(Direction1 left, Direction1 right) => (left as IDirection1).CompareTo(right) <= 0;
        public static bool operator >=(Direction1 left, Direction1 right) => (left as IDirection1).CompareTo(right) >= 0;
        public static bool operator <(Direction1 left, Direction1 right) => (left as IDirection1).CompareTo(right) < 0;
        public static bool operator >(Direction1 left, Direction1 right) => (left as IDirection1).CompareTo(right) > 0;

        public static Direction1 operator +(Direction1 left, Direction1 right) => left.Vector + right.Vector;
        public static Direction1 operator -(Direction1 direction) => -direction.Vector;
        public static Direction1 operator -(Direction1 left, Direction1 right) => left.Vector - right.Vector;
        public static Direction1 operator *(Direction1 direction, Position1 scale) => direction.Vector * scale.Vector;
        public static Direction1 operator /(Direction1 direction, Position1 scale) => direction.Vector / scale.Vector;
        public static Direction1 operator /(Position1 scale, Direction1 direction) => scale.Vector / direction.Vector;

        public static Position1 operator *(Direction1 left, Direction1 right) => left.Vector * right.Vector;
        public static Position1 operator /(Direction1 left, Direction1 right) => left.Vector / right.Vector;

        public override bool Equals(object? obj) => obj is Direction1 direction && Equals(direction);
        public bool Equals(Direction1 other) => Vector.Equals(other.Vector);
        public bool Equals(Direction1? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
        public string ToString(string? format) => Vector.ToString(format);
        public Normal1 Normalized() => new(Vector);
    }
}
