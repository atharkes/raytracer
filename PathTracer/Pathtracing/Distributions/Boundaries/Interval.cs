using PathTracer.Geometry.Positions;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
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
            Entry = entry;
            Exit = exit;
        }
    }
}
