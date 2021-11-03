using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Geometry.Normals {
    /// <summary> A 2-dimesional normalized direction vector </summary>
    public struct Normal2 : IDirection2, IEquatable<Normal2> {
        /// <summary> The unit vector in the X direction </summary>
        public static Normal2 UnitX => new(Vector2.UnitX);
        /// <summary> The unit vector in the Y direction </summary>
        public static Normal2 UnitY => new(Vector2.UnitY);

        /// <summary> The <see cref="Vector2"/> </summary>
        public Vector2 Vector { get; }

        public Normal2(Vector2 vector) {
            Debug.Assert(vector.Length != 0, "Normal vector cannot have a length of 0");
            Vector = vector.Normalized();
        }

        public Normal2(Vector1 x, Vector1 y) {
            Debug.Assert(x != 0 || y != 0, "Normal vector cannot have a length of 0");
            Vector = new Vector2(x, y).Normalized();
        }

        public static bool operator ==(Normal2 left, Normal2 right) => left.Equals(right);
        public static bool operator !=(Normal2 left, Normal2 right) => !left.Equals(right);

        public static Normal2 operator -(Normal2 normal) => new(-normal.Vector);

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Normal2 other) => Vector.Equals(other.Vector);
        public bool Equals(Normal2? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
    }
}
