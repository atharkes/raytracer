using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Directions {
    public struct Direction2 : IDirection2, IEquatable<Direction2> {
        /// <summary> The direction vector reprsenting no direction </summary>
        public static readonly Direction2 Zero = Vector2.Zero;
        /// <summary> The direction vector 1 in all directions </summary>
        public static readonly Direction2 One = Vector2.One;

        public Vector2 Vector { get; }

        /// <summary> The X-coordinate of the <see cref="IDirection2"/> </summary>
        public Direction1 X => Vector.X;
        /// <summary> The Y-coordinate of the <see cref="IDirection2"/> </summary>
        public Direction1 Y => Vector.Y;

        public Direction2(Vector2 value) {
            Vector = value;
        }

        public Direction2(Direction1 xy) {
            Vector = new Vector2(xy, xy);
        }

        public Direction2(Direction1 x, Direction1 y) {
            Vector = new Vector2(x, y);
        }

        public static implicit operator Direction2(Vector2 value) => new(value);
        public static implicit operator Direction2((Vector1 X, Vector1 Y) tuple) => new(tuple.X, tuple.Y);
        public static implicit operator Direction2(Normal2 normal) => new(normal.Vector);

        public static bool operator ==(Direction2 left, Direction2 right) => left.Equals(right);
        public static bool operator !=(Direction2 left, Direction2 right) => !(left == right);

        public static Direction2 operator +(Direction2 left, Direction2 right) => left.Vector + right.Vector;
        public static Direction2 operator -(Direction2 direction) => -direction.Vector;
        public static Direction2 operator -(Direction2 left, Direction2 right) => left.Vector - right.Vector;
        public static Direction2 operator *(Direction2 direction, Position1 scale) => direction.Vector * scale.Vector;
        public static Direction2 operator *(Direction2 direction, Position2 scale) => direction.Vector * scale.Vector;
        public static Direction2 operator /(Direction2 direction, Position1 scale) => direction.Vector / scale.Vector;
        public static Direction2 operator /(Position1 scale, Direction2 direction) => scale.Vector / direction.Vector;
        public static Direction2 operator /(Direction2 direction, Position2 scale) => direction.Vector / scale.Vector;

        public override bool Equals(object? obj) => obj is Direction2 direction && Equals(direction);
        public bool Equals(Direction2 other) => Vector.Equals(other.Vector);
        public bool Equals(Direction2? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
        public Normal2 Normalized() => new(Vector);
    }
}
