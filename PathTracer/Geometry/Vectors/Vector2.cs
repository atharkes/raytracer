namespace PathTracer.Geometry.Vectors;

/// <summary> A 2-dimensional vector </summary>
public readonly struct Vector2 : IVector2, IEquatable<Vector2> {
    /// <summary> The smallest <see cref="Vector2"/> greater than 0 </summary>
    public static readonly Vector2 Epsilon = new(Vector1.Epsilon, Vector1.Epsilon);
    /// <summary> The maximum <see cref="Vector2"/> value </summary>
    public static readonly Vector2 MaxValue = new(Vector1.MaxValue, Vector1.MaxValue);
    /// <summary> The minimum <see cref="Vector2"/> value </summary>
    public static readonly Vector2 MinValue = new(Vector1.MinValue, Vector1.MinValue);
    /// <summary> The <see cref="Vector2"/> representation of NaN </summary>
    public static readonly Vector2 NaN = new(Vector1.NaN, Vector1.NaN);
    /// <summary> The <see cref="Vector2"/> representation of negative infinity </summary>
    public static readonly Vector2 NegativeInfinity = OpenTK.Mathematics.Vector2.NegativeInfinity;
    /// <summary> The <see cref="Vector2"/> representation of positive infinity </summary>
    public static readonly Vector2 PositiveInfinity = OpenTK.Mathematics.Vector2.PositiveInfinity;
    /// <summary> The <see cref="Vector2"/> with both components 0 </summary>
    public static readonly Vector2 Zero = OpenTK.Mathematics.Vector2.Zero;
    /// <summary> The <see cref="Vector2"/> with both components 1 </summary>
    public static readonly Vector2 One = OpenTK.Mathematics.Vector2.One;
    /// <summary> The unit <see cref="Vector2"/> in the X-direction </summary>
    public static readonly Vector2 UnitX = OpenTK.Mathematics.Vector2.UnitX;
    /// <summary> The unit <see cref="Vector2"/> in the Y-direction </summary>
    public static readonly Vector2 UnitY = OpenTK.Mathematics.Vector2.UnitY;

    /// <summary> The value of the <see cref="Vector2"/> </summary>
    public readonly OpenTK.Mathematics.Vector2 Value;

    /// <summary> The X-component of the <see cref="Vector2"/> </summary>
    public float X => Value.X;
    /// <summary> The Y-component of the <see cref="Vector2"/> </summary>
    public float Y => Value.Y;
    /// <summary> The length of the <see cref="Vector2"/> </summary>
    public float Length => Value.Length;
    /// <summary> The squared length of the <see cref="Vector2"/> </summary>
    public float LengthSquared => Value.LengthSquared;

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

    public override bool Equals(object? obj) => obj is Vector2 vector && Equals(vector);
    public bool Equals(Vector2 other) => Value.Equals(other.Value);
    public bool Equals(Vector2? other) => Value.Equals(other?.Value);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
    public string ToString(string? format) => $"({X.ToString(format)}, {Y.ToString(format)})";

    public Vector2 Normalized() => Value.Normalized();
}
