using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

/// <summary> A point in 3-dimensional space </summary>
public interface IPoint3 {
    /// <summary> The position of the <see cref="IPoint3"/> </summary>
    Position3 Position { get; }
    /// <summary> The normal of the <see cref="IPoint3"/> </summary>
    Normal3 Normal { get; }

    /// <summary> Get an <see cref="IPoint3"/> with the normal flipped </summary>
    /// <returns>A <see cref="IPoint3"/> with the normal flipped</returns>
    IPoint3 NormalFlipped();

    /// <summary> Check whether a <paramref name="direction"/> goes into the <see cref="IPoint3"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes into the <see cref="IPoint3"/></returns>
    bool IsTowards(IDirection3 direction) => IDirection3.Dot(Normal, direction) < 0;

    /// <summary> Check whether a <paramref name="direction"/> goes away from the <see cref="IPoint3"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes away from the <see cref="IPoint3"/></returns>
    bool IsFrom(IDirection3 direction) => IDirection3.Dot(Normal, direction) > 0;
}
