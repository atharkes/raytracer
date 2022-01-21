using PathTracer.Geometry.Positions;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Distributions.Intervals {
    /// <summary> An interval </summary>
    public struct Interval : IInterval {
        /// <summary> The entry point of the <see cref="Interval"/> </summary>
        public Position1 Entry { get; }
        /// <summary> The exit pointof the <see cref="Interval"/> </summary>
        public Position1 Exit { get; }

        /// <summary> Create a new <see cref="Interval"/> </summary>
        /// <param name="entry">The entry point of the <see cref="Interval"/></param>
        /// <param name="exit">The exit point of the <see cref="Interval"/></param>
        public Interval(Position1 entry, Position1 exit) {
            Debug.Assert(!float.IsNaN(entry) && !float.IsNaN(exit));
            Entry = entry;
            Exit = exit;
        }
    }
}
