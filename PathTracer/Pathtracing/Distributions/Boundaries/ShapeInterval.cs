using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> A boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public struct ShapeInterval : IShapeInterval {
        /// <summary> The <see cref="IShape"/> that produced the <see cref="ShapeInterval"/> </summary>
        public IShape Shape { get; }
        /// <summary> The entry-point of the <see cref="ShapeInterval"/> </summary>
        public Position1 Entry { get; }
        /// <summary> The exit-point of the <see cref="ShapeInterval"/> </summary>
        public Position1 Exit { get; }

        /// <summary> Create a new <see cref="ShapeInterval"/> </summary>
        /// <param name="entry">The entry-point of the <see cref="ShapeInterval"/></param>
        /// <param name="exit">The exit-point of the <see cref="ShapeInterval"/></param>
        public ShapeInterval(IShape shape, Position1 entry, Position1 exit) {
            Debug.Assert(entry <= exit, "Provided interval is not valid");
            Shape = shape;
            Entry = entry;
            Exit = exit;
        }
    }
}
