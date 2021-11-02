using System;
using System.Diagnostics;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 1-dimensional vector </summary>
    public struct Vector1 : IVector1, IEquatable<Vector1>, IComparable<Vector1> {
        /// <summary> The smallest <see cref="Vector1"/> greater than 0 </summary>
        public static Vector1 Epsilon => float.Epsilon;
        /// <summary> The maximum <see cref="Vector1"/> value </summary>
        public static Vector1 MaxValue => float.MaxValue;
        /// <summary> The minimum <see cref="Vector1"/> value </summary>
        public static Vector1 MinValue => float.MinValue;
        /// <summary> The <see cref="Vector1"/> representation of NaN </summary>
        public static Vector1 NaN => float.NaN;
        /// <summary> The <see cref="Vector1"/> representation of negative infinity </summary>
        public static Vector1 NegativeInfinity => float.NegativeInfinity;
        /// <summary> The <see cref="Vector1"/> representation of positive infinity </summary>
        public static Vector1 PositiveInfinity => float.PositiveInfinity;
        /// <summary> The <see cref="Vector1"/> representing 0 </summary>
        public static Vector1 Zero => 0;
        /// <summary> The <see cref="Vector1"/> representing 1 </summary>
        public static Vector1 One => 1;

        /// <summary> The <see cref="float"/> value of the <see cref="Vector1"/> </summary>
        public float Value { get; }

        /// <summary> The X-component of the <see cref="Vector1"/> </summary>
        public Vector1 X => Value;
        /// <summary> The length of the <see cref="Vector1"/> </summary>
        public Vector1 Length => Value;
        /// <summary> The squared length of the <see cref="Vector1"/> </summary>
        public Vector1 LengthSquared => Value * Value;

        /// <summary> Create a <see cref="Vector1"/></summary>
        /// <param name="value">The floating point value of the <see cref="Vector1"/></param>
        public Vector1(float value) {
            Value = value;
        }

        /// <summary> Convert an <see cref="int"/> to a <see cref="Vector1"/> </summary>
        /// <param name="value">The <see cref="int"/> to use for the conversion</param>
        public static implicit operator Vector1(int value) => new(value);
        /// <summary> Convert a <see cref="float"/> to a <see cref="Vector1"/> </summary>
        /// <param name="value">The <see cref="float"/> to use for the conversion</param>
        public static implicit operator Vector1(float value) => new(value);
        /// <summary> Convert a <see cref="Vector1"/> to a <see cref="float"/> </summary>
        /// <param name="value">The <see cref="Vector1"/> to use for the conversion</param>
        public static implicit operator float(Vector1 value) => value.Value;

        public static bool operator ==(Vector1 left, Vector1 right) => left.Equals(right);
        public static bool operator !=(Vector1 left, Vector1 right) => !(left == right);
        public static bool operator <=(Vector1 left, Vector1 right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Vector1 left, Vector1 right) => left.CompareTo(right) >= 0;
        public static bool operator <(Vector1 left, Vector1 right) => left.CompareTo(right) < 0;
        public static bool operator >(Vector1 left, Vector1 right) => left.CompareTo(right) > 0;

        public static Vector1 operator +(Vector1 left, Vector1 right) => new(left.Value + right.Value);
        public static Vector1 operator -(Vector1 vector) => new(-vector.Value);
        public static Vector1 operator -(Vector1 left, Vector1 right) => new(left.Value - right.Value);
        public static Vector1 operator *(Vector1 left, Vector1 right) => new(left.Value * right.Value);
        public static Vector1 operator /(Vector1 left, Vector1 right) => new(left.Value / right.Value);

        public static Vector1 Dot(Vector1 left, Vector1 right) => left * right;
        public static Vector1 ComponentMin(Vector1 left, Vector1 right) => Math.Min(left.Value, right.Value);
        public static Vector1 ComponentMax(Vector1 left, Vector1 right) => Math.Max(left.Value, right.Value);
        public static Vector1 Abs(Vector1 vector) => Math.Abs(vector.Value);

        
        public override bool Equals(object? obj) => Value.Equals(obj);
        
        public bool Equals(Vector1? other) => Value.Equals(other?.Value);
        public bool Equals(Vector1 other) => Value.Equals(other.Value);
        public bool Equals(Vector1 other, Vector1 epsilon) {
            Vector1 absA = Abs(this);
            Vector1 absB = Abs(other);
            Vector1 diff = Abs(this - other);
            if (Equals(other)) { // shortcut, handles infinities
                return true;
            } else if (Equals(Zero) || other.Equals(Zero) || absA + absB < Epsilon) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * Epsilon);
            } else { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(Vector1? other) => Value.CompareTo(other?.Value);
        public int CompareTo(Vector1 other) => Value.CompareTo(other.Value);

        public Vector1 Normalized() {
            Debug.Assert(Length != 0);
            return Value < 0 ? -1 : 1;
        }
    }
}
