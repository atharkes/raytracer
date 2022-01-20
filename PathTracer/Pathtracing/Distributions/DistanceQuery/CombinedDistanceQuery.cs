using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class CombinedDistanceQuery : IDistanceQuery {
        /// <summary> The <see cref="IDistanceDistribution"/> of the <see cref="CombinedDistanceQuery"/> </summary>
        public IDistanceDistribution DistanceDistribution => new CombinedDistanceDistribution(queries.Select(q => q.DistanceDistribution).ToArray());

        readonly SortedSet<IDistanceQuery> queries;

        public CombinedDistanceQuery(params IDistanceQuery[] queries) {
            this.queries = new(queries, Comparer<IDistanceQuery>.Create((a, b) => a.DistanceDistribution.Domain.Entry.CompareTo(b.DistanceDistribution.Domain.Entry)));
        }

        public bool Contains(IPrimitive primitive) => queries.Any(q => q.Contains(primitive));

        public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) {
            Debug.Assert(DistanceDistribution.Contains(sample));
            List<(WeightedPMF<IPrimitive>, double)> pmfs = new();
            double cdf = DistanceDistribution.CumulativeProbability(sample);
            var contains = queries.Where(d => d.DistanceDistribution.Contains(sample));
            if (contains.Count() == 1) {
                IDistanceQuery query = contains.First();
                double queryCdf = query.DistanceDistribution.CumulativeProbability(sample);
                if (queryCdf == 1 || cdf == queryCdf) {
                    return query.GetPrimitives(sample);
                }
                double othersCdf = (queryCdf - cdf) / (queryCdf - 1);
                return new WeightedPMF<IPrimitive>((query.GetPrimitives(sample), othersCdf));
            } else {
                foreach (var query in contains) {
                    WeightedPMF<IPrimitive> pmf = query.GetPrimitives(sample);
                    double queryCdf = query.DistanceDistribution.CumulativeProbability(sample);
                    if (queryCdf == 1) {
                        return pmf;
                    }
                    double othersCdf = (queryCdf - cdf) / (queryCdf - 1);
                    double probabilityDensity = query.DistanceDistribution.Probability(sample) * (1 - othersCdf);
                    pmfs.Add((pmf, probabilityDensity));
                }
                return new WeightedPMF<IPrimitive>(pmfs.ToArray());
            }
        }
    }
}
