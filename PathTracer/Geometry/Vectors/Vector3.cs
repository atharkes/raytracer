using System;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 3-dimensional vector </summary>
    public struct Vector3 : IVector3, IEquatable<Vector3> {
        /// <summary> The smallest <see cref="Vector3"/> greater than 0 </summary>
        public static Vector3 Epsilon => new(Vector1.Epsilon, Vector1.Epsilon, Vector1.Epsilon);
        /// <summary> The maximum <see cref="Vector3"/> value </summary>
        public static Vector3 MaxValue => new(Vector1.MaxValue, Vector1.MaxValue, Vector1.MaxValue);
        /// <summary> The minimum <see cref="Vector3"/> value </summary>
        public static Vector3 MinValue => new(Vector1.MinValue, Vector1.MinValue, Vector1.MinValue);
        /// <summary> The <see cref="Vector3"/> representation of NaN </summary>
        public static Vector3 NaN => new(Vector1.NaN, Vector1.NaN, Vector1.NaN);
        /// <summary> The <see cref="Vector3"/> representation of negative infinity </summary>
        public static Vector3 NegativeInfinity => new(Vector1.NegativeInfinity, Vector1.NegativeInfinity, Vector1.NegativeInfinity);
        /// <summary> The <see cref="Vector3"/> representation of positive infinity </summary>
        public static Vector3 PositiveInfinity => new(Vector1.PositiveInfinity, Vector1.PositiveInfinity, Vector1.PositiveInfinity);
        /// <summary> The <see cref="Vector3"/> with both components 0 </summary>
        public static Vector3 Zero => new(Vector1.Zero, Vector1.Zero, Vector1.Zero);
        /// <summary> The <see cref="Vector3"/> with both components 1 </summary>
        public static Vector3 One => new(Vector1.One, Vector1.One, Vector1.One);
        /// <summary> The unit <see cref="Vector3"/> in the X-direction </summary>
        public static Vector3 UnitX => new(Vector1.One, Vector1.Zero, Vector1.Zero);
        /// <summary> The unit <see cref="Vector3"/> in the Y-direction </summary>
        public static Vector3 UnitY => new(Vector1.Zero, Vector1.One, Vector1.Zero);
        /// <summary> The unit <see cref="Vector3"/> in the Z-direction </summary>
        public static Vector3 UnitZ => new(Vector1.Zero, Vector1.Zero, Vector1.One);

        /// <summary> The value of the <see cref="Vector3"/> </summary>
        public OpenTK.Mathematics.Vector3 Value { get; }

        /// <summary> Get a value at the specified <paramref name="index"/> </summary>
        /// <param name="index">The index to get the value at</param>
        /// <returns>The value at the specified <paramref name="index"/></returns>
        public Vector1 this[int index] => Value[index];
        /// <summary> The X-component of the <see cref="Vector3"/> </summary>
        public Vector1 X => Value.X;
        /// <summary> The Y-component of the <see cref="Vector3"/> </summary>
        public Vector1 Y => Value.Y;
        /// <summary> The Z-component of the <see cref="Vector3"/> </summary>
        public Vector1 Z => Value.Z;
        /// <summary> The length of the <see cref="Vector3"/> </summary>
        public Vector1 Length => Value.Length;
        /// <summary> The squared length of the <see cref="Vector3"/> </summary>
        public Vector1 LengthSquared => Value.LengthSquared;

        public Vector3(float x, float y, float z) {
            Value = new OpenTK.Mathematics.Vector3(x, y, z);
        }

        public Vector3(Vector1 x, Vector1 y, Vector1 z) {
            Value = new OpenTK.Mathematics.Vector3(x.Value, y.Value, z.Value);
        }

        public Vector3(OpenTK.Mathematics.Vector3 value) {
            Value = value;
        }

        public static implicit operator Vector3((float X, float Y, float Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);
        public static implicit operator Vector3((Vector1 X, Vector1 Y, Vector1 Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);
        public static implicit operator Vector3(OpenTK.Mathematics.Vector3 value) => new(value);
        public static implicit operator OpenTK.Mathematics.Vector3(Vector3 value) => value.Value;

        public static bool IsFinite(Vector3 f) => Vector1.IsFinite(f.X) && Vector1.IsFinite(f.Y) && Vector1.IsFinite(f.Z);
        public static bool IsInfinity(Vector3 f) => Vector1.IsInfinity(f.X) || Vector1.IsInfinity(f.Y) || Vector1.IsInfinity(f.Z);
        public static bool IsNaN(Vector3 f) => Vector1.IsNaN(f.X) || Vector1.IsNaN(f.Y) || Vector1.IsNaN(f.Z);
        public static bool IsNegativeInfinity(Vector3 f) => Vector1.IsNegativeInfinity(f.X) || Vector1.IsNegativeInfinity(f.Y) || Vector1.IsNegativeInfinity(f.Z);
        public static bool IsPositiveInfinity(Vector3 f) => Vector1.IsPositiveInfinity(f.X) || Vector1.IsPositiveInfinity(f.Y) || Vector1.IsPositiveInfinity(f.Z);
        public static bool IsSubnormal(Vector3 f) => Vector1.IsSubnormal(f.X) || Vector1.IsSubnormal(f.Y) || Vector1.IsSubnormal(f.Z);

        public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);
        public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);

        public static Vector3 operator +(Vector3 left, Vector3 right) => left.Value + right.Value;
        public static Vector3 operator -(Vector3 vector) => -vector.Value;
        public static Vector3 operator -(Vector3 left, Vector3 right) => left.Value - right.Value;
        public static Vector3 operator *(Vector3 left, Vector3 right) => left.Value * right.Value;
        public static Vector3 operator *(Vector3 left, Vector1 right) => left.Value * right.Value;
        public static Vector3 operator *(Vector1 left, Vector3 right) => left.Value * right.Value;
        public static Vector3 operator *(OpenTK.Mathematics.Quaternion left, Vector3 right) => left * right.Value;
        public static Vector3 operator /(Vector3 left, Vector1 right) => left.Value / right.Value;
        public static Vector3 operator /(Vector1 left, Vector3 right) => new(left / right.X, left / right.Y, left / right.Z);
        public static Vector3 operator /(Vector3 left, Vector3 right) => OpenTK.Mathematics.Vector3.Divide(left, right);

        public static Vector1 Dot(Vector3 left, Vector3 right) => OpenTK.Mathematics.Vector3.Dot(left, right);
        public static Vector3 Cross(Vector3 left, Vector3 right) => OpenTK.Mathematics.Vector3.Cross(left, right);
        public static Vector3 ComponentMin(Vector3 left, Vector3 right) => OpenTK.Mathematics.Vector3.ComponentMin(left, right);
        public static Vector3 ComponentMax(Vector3 left, Vector3 right) => OpenTK.Mathematics.Vector3.ComponentMax(left, right);
        public static Vector3 Abs(Vector3 vector) => new(Vector1.Abs(vector.X), Vector1.Abs(vector.Y), Vector1.Abs(vector.Z));
        public static Vector3 Clamp(Vector3 vector, Vector3 min, Vector3 max) => OpenTK.Mathematics.Vector3.Clamp(vector, min, max);
        public static Vector3 Lerp(Vector3 a, Vector3 b, Vector1 blend) => OpenTK.Mathematics.Vector3.Lerp(a, b, blend);

        public override bool Equals(object? obj) => Value.Equals(obj);
        public bool Equals(Vector3 other) => Value.Equals(other.Value);
        public bool Equals(Vector3? other) => Value.Equals(other?.Value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();

        public Vector3 Normalized() => Value.Normalized();
    }
}
