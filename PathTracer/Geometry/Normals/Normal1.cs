using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Geometry.Normals {
    /// <summary> A 1-dimesional normalized direction vector </summary>
    public struct Normal1 : IDirection1, IEquatable<Normal1> {
        /// <summary> The one vector </summary>
        public static readonly Normal1 One = new(Vector1.One);
        /// <summary> The default right direction when no rotation is applied </summary>
        public static readonly Normal1 DefaultRight = One;
        /// <summary> The default left direction when no rotation is applied </summary>
        public static readonly Normal1 DefaultLeft = -DefaultRight;

        /// <summary> The <see cref="Vector1"/> </summary>
        public Vector1 Vector { get; }

        public Normal1(Vector1 value) {
            Debug.Assert(value != 0, "Normal vector cannot have a length of 0");
            Vector = value > 0 ? 1 : -1;
        }

        public static bool operator ==(Normal1 left, Normal1 right) => left.Equals(right);
        public static bool operator !=(Normal1 left, Normal1 right) => !left.Equals(right);

        public static Normal1 operator -(Normal1 normal) => new(-normal.Vector);

        public override bool Equals(object? obj) => obj is Normal2 normal && Equals(normal);
        public bool Equals(Normal1 other) => Vector.Equals(other.Vector);
        public bool Equals(Normal1? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
    }
}
