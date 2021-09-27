using PathTracer.Pathtracing.SceneDescription;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Boundaries {
    /// <summary> The boundary of a <see cref="IShape"/>-<see cref="IRay"/>-intersection </summary>
    public interface IBoundaryCollection {
        /// <summary> The <see cref="IBoundaryInterval"/>s making up the <see cref="IBoundaryCollection"/> </summary>
        IEnumerable<IBoundaryInterval> BoundaryIntervals { get; }

        /// <summary> Whether the <see cref="IBoundaryCollection"/> is volumetric </summary>
        bool Volumetric => BoundaryIntervals.Any(i => i.Volumetric);
        /// <summary> Whether the <see cref="IBoundaryCollection"/> is planar </summary>
        bool Planar => BoundaryIntervals.All(i => i.Planar);
        /// <summary> The amount of times the <see cref="IRay"/> entered the <see cref="IBoundaryCollection"/> </summary>
        int Passthroughs => BoundaryIntervals.Count();

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryCollection"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryCollection"/></returns>
        bool Inside(double distance) => BoundaryIntervals.Any(i => i.Inside(distance));

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryCollection"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryCollection"/></returns>
        bool Outside(double distance) => !Inside(distance);

        IBoundaryPoint FirstEntry(double start, double end);

        IBoundaryPoint Entry(int index);

        IBoundaryPoint Exit(int index);
    }
}
