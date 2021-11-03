using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Geometry.Normals {
    /// <summary> A 1-dimesional normalized direction vector </summary>
    public struct Normal1 : IDirection1, IEquatable<Normal1> {
        /// <summary> The one vector </summary>
        public static Normal1 One => new(Vector1.One);

        /// <summary> The <see cref="Vector1"/> </summary>
        public Vector1 Vector { get; }

        public Normal1(Vector1 value) {
            Debug.Assert(value != 0, "Normal vector cannot have a length of 0");
            Vector = value > 0 ? 1 : -1;
        }

        public static bool operator ==(Normal1 left, Normal1 right) => left.Equals(right);
        public static bool operator !=(Normal1 left, Normal1 right) => !left.Equals(right);

        public static Normal1 operator -(Normal1 normal) => new(-normal.Vector);

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Normal1 other) => Vector.Equals(other.Vector);
        public bool Equals(Normal1? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
    }
}
