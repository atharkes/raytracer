using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

/// <summary> A point in 1-dimensional space </summary>
public interface IPoint1 {
    /// <summary> The position of the <see cref="IPoint1"/> </summary>
    Position1 Position { get; }
    /// <summary> The normal of the <see cref="IPoint1"/> </summary>
    Normal1 Normal { get; }

    /// <summary> Get an <see cref="IPoint1"/> with the normal flipped </summary>
    /// <returns>A <see cref="IPoint1"/> with the normal flipped</returns>
    IPoint1 NormalFlipped();

    /// <summary> Check whether a <paramref name="direction"/> goes into the <see cref="IPoint1"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes into the <see cref="IPoint1"/></returns>
    bool IsTowards(IDirection1 direction) => IDirection1.Dot(Normal, direction) < 0;

    /// <summary> Check whether a <paramref name="direction"/> goes away from the <see cref="IPoint1"/> </summary>
    /// <param name="direction">The direction</param>
    /// <returns>Whether the <paramref name="direction"/> goes away from the <see cref="IPoint1"/></returns>
    bool IsFrom(IDirection1 direction) => IDirection1.Dot(Normal, direction) > 0;
}
