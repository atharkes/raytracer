using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Boundaries {
    /// <summary> An interface for a boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public interface IBoundaryInterval {
        /// <summary> The entry-point of the <see cref="IBoundaryInterval"/> </summary>
        IBoundaryPoint Entry { get; }
        /// <summary> The exit-point of the <see cref="IBoundaryInterval"/> </summary>
        IBoundaryPoint Exit { get; }

        /// <summary> Whether the <see cref="IBoundaryInterval"/> is a valid interval </summary>
        bool Valid => Entry.Distance <= Exit.Distance;
        /// <summary> Whether the <see cref="IBoundaryInterval"/> is volumetric </summary>
        bool Volumetric => Entry.Distance < Exit.Distance;
        /// <summary> Whether the <see cref="IBoundaryInterval"/> is planar </summary>
        bool Planar => Entry.Distance == Exit.Distance;

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryInterval"/></returns>
        bool Inside(double distance) => Entry.Distance <= distance && distance <= Exit.Distance;

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryInterval"/></returns>
        bool Outside(double distance) => distance < Entry.Distance || Exit.Distance < distance;
    }
}
