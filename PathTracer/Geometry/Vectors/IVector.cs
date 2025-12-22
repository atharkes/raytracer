namespace PathTracer.Geometry.Vectors;

/// <summary> A dimensionless vector </summary>
public interface IVector {
    /// <summary> The length of the <see cref="IVector"/> </summary>
    float Length { get; }
    /// <summary> The squared length of the <see cref="IVector"/> </summary>
    float LengthSquared { get; }

    /// <summary> Convert the <see cref="IVector"/> to a <see cref="string"/> </summary>
    /// <returns>A <see cref="string"/> representing the <see cref="IVector"/></returns>
    string ToString();

    /// <summary>Convert the <see cref="IVector"/> to a <see cref="string"/> using a <paramref name="format"/> </summary>
    /// <param name="format">The format to use</param>
    /// <returns>A <see cref="string"/> representing the <see cref="IVector"/></returns>
    string ToString(string? format);
}
