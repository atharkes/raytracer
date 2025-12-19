using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Positions;

/// <summary> A dimensionless position <see cref="IVector"/> </summary>
/// <typeparam name="T">The <see cref="IVector"/> determining the dimension of the <see cref="IPosition{T}"/></typeparam>
public interface IPosition<T> where T : IVector {
    /// <summary> The <see cref="IVector"/> used for this <see cref="IPosition{T}"/> </summary>
    T Vector { get; }

    /// <summary> Convert the <see cref="IPosition{T}"/> to a <see cref="string"/> </summary>
    /// <returns>A <see cref="string"/> representing the <see cref="IPosition{T}"/></returns>
    string ToString() => Vector.ToString();

    /// <summary>Convert the <see cref="IPosition{T}"/> to a <see cref="string"/> using a <paramref name="format"/> </summary>
    /// <param name="format">The format to use</param>
    /// <returns>A <see cref="string"/> representing the <see cref="IPosition{T}"/></returns>
    string ToString(string? format) => Vector.ToString(format);
}
