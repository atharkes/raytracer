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
        public static readonly Normal3 UnitX = new(Unit3.X);
        /// <summary> The unit vector in the Y direction </summary>
        public static readonly Normal3 UnitY = new(Unit3.Y);
        /// <summary> The unit vector in the Z direction </summary>
        public static readonly Normal3 UnitZ = new(Unit3.Z);
        /// <summary> The default right direction when no rotation is applied </summary>
        public static readonly Normal3 DefaultRight = UnitX;
        /// <summary> The default left direction when no rotation is applied </summary>
        public static readonly Normal3 DefaultLeft = -DefaultRight;
        /// <summary> The default up vector when no rotation is applied </summary>
        public static readonly Normal3 DefaultUp = UnitY;
        /// <summary> The default down vector when no rotation is applied </summary>
        public static readonly Normal3 DefaultDown = -DefaultUp;
        /// <summary> The default front vector when no rotation is applied </summary>
        public static readonly Normal3 DefaultFront = UnitZ;
        /// <summary> The default back vector when no rotation is applied </summary>
        public static readonly Normal3 DefaultBack = -DefaultFront;

        Vector3 IDirection<Vector3>.Vector => Vector;
        /// <summary> The <see cref="Vector3"/> of the <see cref="Normal3"/> </summary>
        public readonly Vector3 Vector;

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

        public static Normal3 AnyPerpendicular(Normal3 normal) {
            if (normal.Equals(UnitX)) {
                return new Normal3(Vector3.Cross(normal.Vector, UnitY.Vector));
            } else {
                return new Normal3(Vector3.Cross(normal.Vector, UnitX.Vector));
            }
        }

        public static Normal3 Perpendicular(Normal3 first, Normal3 second) => new(Vector3.Cross(first.Vector, second.Vector));
        public static Normal3 Reflect(Normal3 direction, Normal3 normal) => new(-direction + normal * 2f * Similarity(direction, normal));
        public static float Similarity(Normal3 left, Normal3 right) => Vector3.Dot(left.Vector, right.Vector);
        public static bool Similar(Normal3 left, Normal3 right) => Similarity(left, right) > 0f;

        public override bool Equals(object? obj) => obj is Normal3 normal && Equals(normal);
        public bool Equals(Normal3 other) => Vector.Equals(other.Vector);
        public bool Equals(Normal3? other) => Vector.Equals(other?.Vector);
        public override int GetHashCode() => Vector.GetHashCode();
        public override string ToString() => Vector.ToString();
        public string ToString(string? format) => Vector.ToString(format);
    }
}
