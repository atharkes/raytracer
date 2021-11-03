﻿using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Positions {
    public struct Position2 : IPosition<Vector2>, IEquatable<Position2> {
        /// <summary> The origin of the 2-dimensional coordinate system </summary>
        public static Position2 Origin => new(Vector2.Zero);
        /// <summary> The maximum position </summary>
        public static Position2 MaxValue => new(Vector2.MaxValue);
        /// <summary> The minimum position </summary>
        public static Position2 MinValue => new(Vector2.MinValue);
        /// <summary> The position representing negative infinity </summary>
        public static Position2 NegativeInfinity => new(Vector2.NegativeInfinity);
        /// <summary> The position representing positive infinity </summary>
        public static Position2 PositiveInfinity => new(Vector2.PositiveInfinity);

        public Vector2 Vector { get; }

        public Position1 X => Vector.X;
        public Position1 Y => Vector.Y;

        public Position2(Vector2 value) {
            Vector = value;
        }

        public Position2(Position1 xy) {
            Vector = new Vector2(xy, xy);
        }

        public Position2(Position1 x, Position1 y) {
            Vector = new Vector2(x, y);
        }

        public static implicit operator Position2(Vector2 vector) => new(vector);

        public static bool operator ==(Position2 left, Position2 right) => left.Equals(right);
        public static bool operator !=(Position2 left, Position2 right) => !(left == right);

        public static Direction2 operator -(Position2 left, Position2 right) => new(left.Vector - right.Vector);
        public static Position2 operator +(Position2 position, Direction2 direction) => new(position.Vector + direction.Vector);

        public static Position1 Dot(Position2 position, IDirection2 direction) => new(Vector2.Dot(position.Vector, direction.Vector));
        public static Position2 ComponentMin(Position2 left, Position2 right) => new(Vector2.ComponentMin(left.Vector, right.Vector));
        public static Position2 ComponentMax(Position2 left, Position2 right) => new(Vector2.ComponentMax(left.Vector, right.Vector));

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Position2 other) => Vector.Equals(other.Vector);
        public bool Equals(Position2? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
    }
}
