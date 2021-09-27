using System.Collections.Generic;

namespace PathTracer.Pathtracing.Boundaries {
    /// <summary> A collection of <see cref="IBoundaryInterval"/>s </summary>
    public struct BoundaryCollection : IBoundaryCollection {
        /// <summary> Return an <see cref="IEnumerable{T}"/> of <see cref="IBoundaryInterval"/>s </summary>
        public IEnumerable<IBoundaryInterval> BoundaryIntervals => intervals;

        readonly SortedSet<IBoundaryInterval> intervals;

        /// <summary> Create a new <see cref="BoundaryCollection"/> </summary>
        /// <param name="interval">The <see cref="IBoundaryInterval"/> to add to the <see cref="BoundaryCollection"/></param>
        public BoundaryCollection(IBoundaryInterval interval) {
            intervals = new SortedSet<IBoundaryInterval>() { interval };
        }

        /// <summary> Create a new <see cref="BoundaryCollection"/> </summary>
        /// <param name="intervals">The <see cref="IBoundaryInterval"/>s to add to the <see cref="BoundaryCollection"/></param>
        public BoundaryCollection(IEnumerable<IBoundaryInterval> intervals) {
            this.intervals = new SortedSet<IBoundaryInterval>(intervals);
        }

        /// <summary> Add another <see cref="IBoundaryCollection"/> to this <see cref="IBoundaryCollection"/> </summary>
        /// <param name="other">The other <see cref="IBoundaryCollection"/></param>
        public void AddRange(IBoundaryCollection other) {
            intervals.UnionWith(other.BoundaryIntervals);
        }
    }
}
