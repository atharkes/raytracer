using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Positions;

public readonly struct Position3 : IPosition<Vector3>, IEquatable<Position3> {
    /// <summary> The origin of the 3-dimensional coordinate system </summary>
    public static readonly Position3 Origin = Vector3.Zero;
    /// <summary> The maximum position </summary>
    public static readonly Position3 MaxValue = Vector3.MaxValue;
    /// <summary> The minimum position </summary>
    public static readonly Position3 MinValue = Vector3.MinValue;
    /// <summary> The position representing negative infinity </summary>
    public static readonly Position3 NegativeInfinity = Vector3.NegativeInfinity;
    /// <summary> The position representing positive infinity </summary>
    public static readonly Position3 PositiveInfinity = Vector3.PositiveInfinity;

    public Vector3 Vector { get; }

    public readonly Position1 X => Vector.X;
    public readonly Position1 Y => Vector.Y;
    public readonly Position1 Z => Vector.Z;

    public Position3(Vector3 vector) {
        Vector = vector;
    }

    public Position3(Position1 xyz) {
        Vector = new Vector3(xyz, xyz, xyz);
    }

    public Position3(Position1 x, Position1 y, Position1 z) {
        Vector = new Vector3(x, y, z);
    }

    public static implicit operator Position3(Vector3 vector) => new(vector);

    public static bool operator ==(Position3 left, Position3 right) => left.Equals(right);
    public static bool operator !=(Position3 left, Position3 right) => !(left == right);

    public static Direction3 operator -(Position3 left, Position3 right) => new(left.Vector - right.Vector);

    public static Position3 operator +(Position3 position, IDirection3 direction) => new(position.Vector + direction.Vector);
    public static Position3 operator -(Position3 position, IDirection3 direction) => new(position.Vector - direction.Vector);

    public static Position3 Lerp(Position3 left, Position3 right, Vector1 blend) => Vector3.Lerp(left.Vector, right.Vector, blend);
    public static Position1 Dot(Position3 position, IDirection3 direction) => Vector3.Dot(position.Vector, direction.Vector);
    public static Position3 ComponentMin(Position3 left, Position3 right) => Vector3.ComponentMin(left.Vector, right.Vector);
    public static Position3 ComponentMax(Position3 left, Position3 right) => Vector3.ComponentMax(left.Vector, right.Vector);

    public override bool Equals(object? obj) => obj is Position3 position && Equals(position);
    public bool Equals(Position3 other) => Vector.Equals(other.Vector);
    public bool Equals(Position3? other) => Vector.Equals(other?.Vector);
    public override int GetHashCode() => Vector.GetHashCode();
    public override string ToString() => Vector.ToString();
    public string ToString(string? format) => Vector.ToString(format);
}
