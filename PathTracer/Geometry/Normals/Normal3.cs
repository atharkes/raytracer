using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Geometry.Normals {
    /// <summary> The unit vectors </summary>
    public enum Unit3 {
        X, Y, Z, MinX, MinY, MinZ
    }

    /// <summary> A 3-dimesional normalized direction vector </summary>
    public struct Normal3 : IDirection3, IEquatable<Normal3> {
        /// <summary> The unit vector in the X direction </summary>
        public static Normal3 UnitX => new(Vector3.UnitX);
        /// <summary> The unit vector in the Y direction </summary>
        public static Normal3 UnitY => new(Vector3.UnitY);
        /// <summary> The unit vector in the Z direction </summary>
        public static Normal3 UnitZ => new(Vector3.UnitZ);

        /// <summary> The <see cref="Vector3"/> </summary>
        public Vector3 Vector { get; }

        /// <summary> The X-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 X => Vector.X;
        /// <summary> The Y-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 Y => Vector.Y;
        /// <summary> The Z-coordinate of the <see cref="IDirection3"/> </summary>
        public Direction1 Z => Vector.Z;

        public Normal3(Unit3 unit) {
            Vector = unit switch {
                Unit3.X => Vector3.UnitX,
                Unit3.Y => Vector3.UnitY,
                Unit3.Z => Vector3.UnitZ,
                Unit3.MinX => -Vector3.UnitX,
                Unit3.MinY => -Vector3.UnitY,
                Unit3.MinZ => -Vector3.UnitZ,
                _ => throw new NotImplementedException("Not a valid unit vector"),
            };
        }

        public Normal3(Vector3 vector) {
            Debug.Assert(vector.Length != 0, "Normal vector cannot have a length of 0");
            Vector = vector.Normalized();
        }

        public Normal3(Direction3 direction) {
            Debug.Assert(direction.Length != 0, "Normal vector cannot have a length of 0");
            Vector = direction.Vector.Normalized();
        }

        public Normal3(Vector1 x, Vector1 y, Vector1 z) {
            Debug.Assert(x != 0 || y != 0 || z != 0, "Normal vector cannot have a length of 0");
            Vector = new Vector3(x, y, z).Normalized();
        }

        public static bool operator ==(Normal3 left, Normal3 right) => left.Equals(right);
        public static bool operator !=(Normal3 left, Normal3 right) => !left.Equals(right);

        public static Normal3 operator -(Normal3 normal) => new(-normal.Vector);
        public static Normal3 operator *(OpenTK.Mathematics.Quaternion left, Normal3 right) => new(left * right.Vector);

        public static Direction3 operator *(Normal3 normal, Position1 position) => (normal as IDirection3) * position;
        public static Direction3 operator /(Normal3 normal, Position1 position) => (normal as IDirection3) / position;

        public static Normal3 Cross(Normal3 left, Normal3 right) => new(Vector3.Cross(left.Vector, right.Vector));
        public static bool Similar(Normal3 left, Normal3 right) => IDirection3.Similar(left, right);

        public override bool Equals(object? obj) => Vector.Equals(obj);
        public bool Equals(Normal3 other) => Vector.Equals(other.Vector);
        public bool Equals(Normal3? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
    }
}
