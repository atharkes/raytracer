﻿using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions {
    /// <summary> A 1-dimensional direction vector </summary>
    public struct Direction1 : IDirection1 {
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

        public Normal1 Normalized() => new(Vector);
    }
}