using System.Diagnostics;

namespace PathTracer.Pathtracing.Points.Boundaries {
    /// <summary> A boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public struct BoundaryInterval : IBoundaryInterval {
        /// <summary> The entry-point of the <see cref="BoundaryInterval"/> </summary>
        public IShapePoint1 Entry { get; }
        /// <summary> The exit-point of the <see cref="BoundaryInterval"/> </summary>
        public IShapePoint1 Exit { get; }

        /// <summary> Create a new <see cref="BoundaryInterval"/> </summary>
        /// <param name="entry">The entry-point of the <see cref="BoundaryInterval"/></param>
        /// <param name="exit">The exit-point of the <see cref="BoundaryInterval"/></param>
        public BoundaryInterval(IShapePoint1 entry, IShapePoint1 exit) {
            Debug.Assert(entry.Position <= exit.Position, "Provided interval is not valid");
            Entry = entry;
            Exit = exit;
        }
    }
}
