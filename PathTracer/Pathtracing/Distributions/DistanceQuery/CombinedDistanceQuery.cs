using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class CombinedDistanceQuery : IDistanceQuery {
        readonly SortedSet<IDistanceQuery> queries;
    }
}
