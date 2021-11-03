using System;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 2-dimensional vector </summary>
    public struct Vector2 : IVector2, IEquatable<Vector2> {
        /// <summary> The smallest <see cref="Vector2"/> greater than 0 </summary>
        public static Vector2 Epsilon => new(Vector1.Epsilon, Vector1.Epsilon);
        /// <summary> The maximum <see cref="Vector2"/> value </summary>
        public static Vector2 MaxValue => new(Vector1.MaxValue, Vector1.MaxValue);
        /// <summary> The minimum <see cref="Vector2"/> value </summary>
        public static Vector2 MinValue => new(Vector1.MinValue, Vector1.MinValue);
        /// <summary> The <see cref="Vector2"/> representation of NaN </summary>
        public static Vector2 NaN => new(Vector1.NaN, Vector1.NaN);
        /// <summary> The <see cref="Vector2"/> representation of negative infinity </summary>
        public static Vector2 NegativeInfinity => new(Vector1.NegativeInfinity, Vector1.NegativeInfinity);
        /// <summary> The <see cref="Vector2"/> representation of positive infinity </summary>
        public static Vector2 PositiveInfinity => new(Vector1.PositiveInfinity, Vector1.PositiveInfinity);
        /// <summary> The <see cref="Vector2"/> with both components 0 </summary>
        public static Vector2 Zero => new(Vector1.Zero, Vector1.Zero);
        /// <summary> The <see cref="Vector2"/> with both components 1 </summary>
        public static Vector2 One => new(Vector1.One, Vector1.One);
        /// <summary> The unit <see cref="Vector2"/> in the X-direction </summary>
        public static Vector2 UnitX => new(Vector1.One, Vector1.Zero);
        /// <summary> The unit <see cref="Vector2"/> in the Y-direction </summary>
        public static Vector2 UnitY => new(Vector1.Zero, Vector1.One);

        /// <summary> The value of the <see cref="Vector2"/> </summary>
        public OpenTK.Mathematics.Vector2 Value { get; }

        /// <summary> The X-component of the <see cref="Vector2"/> </summary>
        public Vector1 X => Value.X;
        /// <summary> The Y-component of the <see cref="Vector2"/> </summary>
        public Vector1 Y => Value.Y;
        /// <summary> The length of the <see cref="Vector2"/> </summary>
        public Vector1 Length => Value.Length;
        /// <summary> The squared length of the <see cref="Vector2"/> </summary>
        public Vector1 LengthSquared => Value.LengthSquared;

        /// <summary> Create a <see cref="Vector2"/> using an <paramref name="x"/> and <paramref name="y"/> </summary>
        /// <param name="x">The X-component of the <see cref="Vector2"/></param>
        /// <param name="y">The Y-component of the <see cref="Vector2"/></param>
        public Vector2(float x, float y) {
            Value = new OpenTK.Mathematics.Vector2(x, y);
        }

        public Vector2(Vector1 xy) {
            Value = new OpenTK.Mathematics.Vector2(xy.Value, xy.Value);
        }

        /// <summary> Create a <see cref="Vector2"/> using two <see cref="Vector1"/>s </summary>
        /// <param name="x">The X-component of the <see cref="Vector2"/></param>
        /// <param name="y">The Y-component of the <see cref="Vector2"/></param>
        public Vector2(Vector1 x, Vector1 y) {
            Value = new OpenTK.Mathematics.Vector2(x.Value, y.Value);
        }

        /// <summary> Create a <see cref="Vector2"/> using an <see cref="OpenTK.Mathematics.Vector2"/> </summary>
        /// <param name="value">The <see cref="OpenTK.Mathematics.Vector2"/></param>
        public Vector2(OpenTK.Mathematics.Vector2 value) {
            Value = value;
        }

        public static implicit operator Vector2((float X, float Y) tuple) => new(tuple.X, tuple.Y);
        public static implicit operator Vector2((Vector1 X, Vector1 Y) tuple) => new(tuple.X, tuple.Y);
        public static implicit operator Vector2(OpenTK.Mathematics.Vector2 value) => new(value);

        public static bool IsFinite(Vector2 f) => Vector1.IsFinite(f.X) && Vector1.IsFinite(f.Y);
        public static bool IsInfinity(Vector2 f) => Vector1.IsInfinity(f.X) || Vector1.IsInfinity(f.Y);
        public static bool IsNaN(Vector2 f) => Vector1.IsNaN(f.X) || Vector1.IsNaN(f.Y);
        public static bool IsNegativeInfinity(Vector2 f) => Vector1.IsNegativeInfinity(f.X) || Vector1.IsNegativeInfinity(f.Y);
        public static bool IsPositiveInfinity(Vector2 f) => Vector1.IsPositiveInfinity(f.X) || Vector1.IsPositiveInfinity(f.Y);
        public static bool IsSubnormal(Vector2 f) => Vector1.IsSubnormal(f.X) || Vector1.IsSubnormal(f.Y);

        public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
        public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);

        public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.Value + right.Value);
        public static Vector2 operator -(Vector2 vector) => new(-vector.Value);
        public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.Value - right.Value);
        public static Vector2 operator *(Vector2 left, Vector2 right) => new(left.Value * right.Value);
        public static Vector2 operator *(Vector2 left, Vector1 right) => new(left.Value * right.Value);
        public static Vector2 operator *(Vector1 left, Vector2 right) => new(left.Value * right.Value);
        public static Vector2 operator /(Vector2 left, Vector1 right) => new(left.Value / right.Value);
        public static Vector2 operator /(Vector1 left, Vector2 right) => new Vector2(left, left) / right;
        public static Vector2 operator /(Vector2 left, Vector2 right) => OpenTK.Mathematics.Vector2.Divide(left.Value, right.Value);

        public static Vector1 Dot(Vector2 left, Vector2 right) => OpenTK.Mathematics.Vector2.Dot(left.Value, right.Value);
        public static Vector2 ComponentMin(Vector2 left, Vector2 right) => OpenTK.Mathematics.Vector2.ComponentMin(left.Value, right.Value);
        public static Vector2 ComponentMax(Vector2 left, Vector2 right) => OpenTK.Mathematics.Vector2.ComponentMax(left.Value, right.Value);
        public static Vector2 Abs(Vector2 vector) => new(Vector1.Abs(vector.X), Vector1.Abs(vector.Y));

        public override bool Equals(object? obj) => Value.Equals(obj);
        public bool Equals(Vector2 other) => Value.Equals(other.Value);
        public bool Equals(Vector2? other) => Value.Equals(other?.Value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();

        public Vector2 Normalized() => Value.Normalized();
    }
}
