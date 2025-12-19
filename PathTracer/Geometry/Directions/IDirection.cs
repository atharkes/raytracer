using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions;

/// <summary> A dimensionless direction <see cref="IVector"/> </summary>
/// <typeparam name="T">The <see cref="IVector"/> determining the dimension of the <see cref="IDirection{T}"/></typeparam>
public interface IDirection<T> where T : IVector {
    /// <summary> The <see cref="IVector"/> used for this <see cref="IDirection{T}"/> </summary>
    T Vector { get; }
    /// <summary> The length of the <see cref="IDirection{T}"/> </summary>
    Position1 Length => Vector.Length;
    /// <summary> The length squared of the <see cref="IDirection{T}"/> </summary>
    Position1 LengthSquared => Vector.LengthSquared;

    /// <summary> Convert the <see cref="IDirection{T}"/> to a <see cref="string"/> </summary>
    /// <returns>A <see cref="string"/> representing the <see cref="IDirection{T}"/></returns>
    string ToString() => Vector.ToString();

    /// <summary>Convert the <see cref="IDirection{T}"/> to a <see cref="string"/> using a <paramref name="format"/> </summary>
    /// <param name="format">The format to use</param>
    /// <returns>A <see cref="string"/> representing the <see cref="IDirection{T}"/></returns>
    string ToString(string? format) => Vector.ToString(format);
}
