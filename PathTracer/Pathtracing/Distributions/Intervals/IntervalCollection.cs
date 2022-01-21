using System.Collections.Generic;

namespace PathTracer.Pathtracing.Distributions.Intervals {
    /// <summary> A collection of <see cref="IInterval"/>s </summary>
    public class IntervalCollection : IIntervalCollection {
        /// <summary> The <see cref="ICollection{T}"/> of <see cref="IInterval"/>s </summary>
        public ICollection<IInterval> Intervals => intervals;
        
        readonly SortedSet<IInterval> intervals;

        /// <summary> Create a new <see cref="IntervalCollection"/> </summary>
        /// <param name="intervals">The <see cref="IInterval"/>s to include in the <see cref="IntervalCollection"/></param>
        public IntervalCollection(params IInterval[] intervals) {
            this.intervals = new SortedSet<IInterval>(intervals);
        }

        /// <summary> Add another <see cref="IIntervalCollection"/> to this <see cref="IIntervalCollection"/> </summary>
        /// <param name="other">The other <see cref="IIntervalCollection"/></param>
        void IIntervalCollection.Add(IIntervalCollection other) => intervals.UnionWith(other);
    }
}
