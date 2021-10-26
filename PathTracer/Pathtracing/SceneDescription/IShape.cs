using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using System;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines a shape </summary>
    public interface IShape : IIntersectable, IBoundable, IDivisible<IShape> {
        /// <summary> Whether the <see cref="IShape"/> encompasses a volume </summary>
        bool Volumetric { get; }
        /// <summary> The volume of the <see cref="IShape"/> </summary>
        float Volume { get; }
        /// <summary> The surface area of the <see cref="IShape"/> </summary>
        float SurfaceArea { get; }

        /// <summary> Check whether a <paramref name="position"/> is inside the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is inside the <see cref="IShape"/></returns>
        bool Inside(Position3 position);

        /// <summary> Check whether a <paramref name="position"/> is outside the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is outside the <see cref="IShape"/></returns>
        bool Outside(Position3 position) => !Inside(position);

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="IShape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="IShape"/></returns>
        Position3 SurfacePosition(Random random);

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="IShape"/></returns>
        bool OnSurface(Position3 position, float epsilon = 0.001F);

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        Normal3 SurfaceNormal(Position3 position);
    }
}
