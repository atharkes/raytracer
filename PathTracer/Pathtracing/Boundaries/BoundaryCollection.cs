using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Boundaries {
    public struct BoundaryCollection : IBoundaryCollection {
        public IEnumerable<IBoundaryInterval> BoundaryIntervals => throw new NotImplementedException();

        SortedSet<IBoundaryInterval> intervals;

        public BoundaryCollection() {
            intervals.
        }

        public IBoundaryPoint Entry(int index) {
            throw new NotImplementedException();
        }

        public IBoundaryPoint Exit(int index) {
            throw new NotImplementedException();
        }

        public IBoundaryPoint FirstEntry(double start, double end) {
            throw new NotImplementedException();
        }
    }
}
