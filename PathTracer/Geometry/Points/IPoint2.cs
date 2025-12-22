using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

/// <summary> A point in 2-dimensional space </summary>
public interface IPoint2 {
    /// <summary> The position of the <see cref="IPoint2"/> </summary>
    Position2 Position { get; }
    /// <summary> The normal of the <see cref="IPoint2"/> </summary>
    Normal2 Normal { get; }

    /// <summary> Get an <see cref="IPoint2"/> with the normal flipped </summary>
    /// <returns>A <see cref="IPoint2"/> with the normal flipped</returns>
    IPoint2 NormalFlipped();

    /// <summary> Check whether a <paramref name="direction"/> goes into the <see cref="IPoint2"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes into the <see cref="IPoint2"/></returns>
    bool IsTowards(IDirection2 direction) => IDirection2.Dot(Normal, direction) < 0;

    /// <summary> Check whether a <paramref name="direction"/> goes away from the <see cref="IPoint2"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes away from the <see cref="IPoint2"/></returns>
    bool IsFrom(IDirection2 direction) => IDirection2.Dot(Normal, direction) > 0;
}
