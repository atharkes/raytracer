using System.Collections.Generic;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> A collection of <see cref="IShapeInterval"/>s </summary>
    public struct BoundaryCollection : IBoundaryCollection {
        /// <summary> Return an <see cref="IEnumerable{T}"/> of <see cref="IShapeInterval"/>s </summary>
        public IEnumerable<IShapeInterval> BoundaryIntervals => intervals;

        readonly SortedSet<IShapeInterval> intervals;

        /// <summary> Create a new <see cref="BoundaryCollection"/> </summary>
        /// <param name="interval">The <see cref="IShapeInterval"/> to add to the <see cref="BoundaryCollection"/></param>
        public BoundaryCollection(IShapeInterval interval) {
            intervals = new SortedSet<IShapeInterval>() { interval };
        }

        /// <summary> Create a new <see cref="BoundaryCollection"/> </summary>
        /// <param name="intervals">The <see cref="IShapeInterval"/>s to add to the <see cref="BoundaryCollection"/></param>
        public BoundaryCollection(IEnumerable<IShapeInterval> intervals) {
            this.intervals = new SortedSet<IShapeInterval>(intervals);
        }

        /// <summary> Add another <see cref="IBoundaryCollection"/> to this <see cref="IBoundaryCollection"/> </summary>
        /// <param name="other">The other <see cref="IBoundaryCollection"/></param>
        public void AddRange(IBoundaryCollection other) {
            intervals.UnionWith(other.BoundaryIntervals);
        }
    }
}
