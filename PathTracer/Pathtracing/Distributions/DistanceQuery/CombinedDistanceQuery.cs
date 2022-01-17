using System.Collections.Generic;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class CombinedDistanceQuery : IDistanceQuery {
        readonly SortedSet<IDistanceQuery> queries;
    }
}
