using System.Diagnostics;

namespace PathTracer.Pathtracing.Points.Boundaries {
    /// <summary> A boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public struct BoundaryInterval : IBoundaryInterval {
        /// <summary> The entry-point of the <see cref="BoundaryInterval"/> </summary>
        public IBoundaryPoint Entry { get; }
        /// <summary> The exit-point of the <see cref="BoundaryInterval"/> </summary>
        public IBoundaryPoint Exit { get; }

        /// <summary> Create a new <see cref="BoundaryInterval"/> </summary>
        /// <param name="entry">The entry-point of the <see cref="BoundaryInterval"/></param>
        /// <param name="exit">The exit-point of the <see cref="BoundaryInterval"/></param>
        public BoundaryInterval(IBoundaryPoint entry, IBoundaryPoint exit) {
            Debug.Assert(entry.Distance > exit.Distance, "Provided interval is not valid");
            Entry = entry;
            Exit = exit;
        }
    }
}
