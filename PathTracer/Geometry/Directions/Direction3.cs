using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions;

public readonly struct Direction3 : IDirection3, IEquatable<Direction3> {
    /// <summary> The direction vector reprsenting no direction </summary>
    public static readonly Direction3 Zero = Vector3.Zero;
    /// <summary> The direction vector with 1 in all directions </summary>
    public static readonly Direction3 One = Vector3.One;

    /// <summary> The <see cref="Vector3"/> value used for this <see cref="IDirection3"/> </summary>
    public Vector3 Vector { get; }

    /// <summary> The X-coordinate of the <see cref="IDirection3"/> </summary>
    public Direction1 X => Vector.X;
    /// <summary> The Y-coordinate of the <see cref="IDirection3"/> </summary>
    public Direction1 Y => Vector.Y;
    /// <summary> The Z-coordinate of the <see cref="IDirection3"/> </summary>
    public Direction1 Z => Vector.Z;
    /// <summary> The length of the <see cref="IDirection3"/> </summary>
    public Vector1 Length => Vector.Length;
    /// <summary> The squared length of the <see cref="IDirection3"/> </summary>
    public Vector1 LengthSquared => Vector.LengthSquared;

    public Direction3(Vector3 vector) {
        Vector = vector;
    }

    public Direction3(Direction1 xyz) {
        Vector = new Vector3(xyz, xyz, xyz);
    }

    public Direction3(Direction1 x, Direction1 y, Direction1 z) {
        Vector = new Vector3(x, y, z);
    }

    public static implicit operator Direction3(Vector3 vector) => new(vector);
    public static implicit operator Direction3((Vector1 X, Vector1 Y, Vector1 Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);
    public static implicit operator Direction3(Normal3 normal) => new(normal.Vector);

    public static explicit operator Direction3(Position3 position) => new(position.Vector);
    public static explicit operator Position3(Direction3 position) => new(position.Vector);

    public static bool operator ==(Direction3 left, Direction3 right) => left.Equals(right);
    public static bool operator !=(Direction3 left, Direction3 right) => !(left == right);

    public static Direction3 operator +(Direction3 left, Direction3 right) => left.Vector + right.Vector;
    public static Direction3 operator -(Direction3 direction) => -direction.Vector;
    public static Direction3 operator -(Direction3 left, Direction3 right) => left.Vector - right.Vector;
    public static Direction3 operator *(Direction3 direction, float scale) => direction.Vector * scale;
    public static Direction3 operator *(Direction3 direction, Position1 scale) => direction.Vector * scale.Vector;
    public static Direction3 operator *(Direction3 direction, Position3 scale) => direction.Vector * scale.Vector;
    public static Direction3 operator /(Direction3 direction, Position1 scale) => direction.Vector / scale.Vector;
    public static Direction3 operator /(Position1 scale, Direction3 direction) => scale.Vector / direction.Vector;
    public static Direction3 operator /(Direction3 direction, Position3 scale) => direction.Vector / scale.Vector;

    public static Direction1 Dot(IDirection3 left, IDirection3 right) => new(Vector3.Dot(left.Vector, right.Vector));
    public static Direction3 Lerp(IDirection3 a, IDirection3 b, Vector1 blend) => Vector3.Lerp(a.Vector, b.Vector, blend);

    public override bool Equals(object? obj) => obj is Direction3 direction && Equals(direction);
    public bool Equals(Direction3 other) => Vector.Equals(other.Vector);
    public bool Equals(Direction3? other) => Vector.Equals(other?.Vector);
    public override int GetHashCode() => Vector.GetHashCode();
    public override string ToString() => Vector.ToString();
    public string ToString(string? format) => Vector.ToString(format);
    public Normal3 Normalized() => new(Vector);
}
