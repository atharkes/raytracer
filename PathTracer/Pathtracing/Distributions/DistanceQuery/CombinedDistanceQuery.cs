using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System.Collections;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery;

public class CombinedDistanceQuery : IDistanceQuery, ICollection<IDistanceQuery> {
    /// <summary> The <see cref="IDistanceDistribution"/> of the <see cref="CombinedDistanceQuery"/> </summary>
    public IDistanceDistribution DistanceDistribution => new CombinedDistanceDistribution(Queries.Select(q => q.DistanceDistribution).ToArray());

    public int Count => Queries.Count;
    public bool IsReadOnly => Queries.IsReadOnly;

    protected readonly ICollection<IDistanceQuery> Queries;

    public CombinedDistanceQuery(params IDistanceQuery[] queries) {
        List<IDistanceQuery> simpleQueries = new();
        foreach (var query in queries) {
            if (query is CombinedDistanceQuery combinedQuery) {
                simpleQueries.AddRange(combinedQuery.Queries);
            } else {
                simpleQueries.Add(query);
            }
        }
        IComparer<IDistanceQuery> comparer = Comparer<IDistanceQuery>.Create((a, b) => a.DistanceDistribution.Domain.Entry.CompareTo(b.DistanceDistribution.Domain.Entry));
        Queries = new SortedSet<IDistanceQuery>(simpleQueries, comparer);
    }

    public void Add(IDistanceQuery item) => Queries.Add(item);
    public void Clear() => Queries.Clear();
    public bool Contains(IDistanceQuery item) => Queries.Contains(item);
    public void CopyTo(IDistanceQuery[] array, int arrayIndex) => Queries.CopyTo(array, arrayIndex);
    public bool Remove(IDistanceQuery item) => Queries.Remove(item);
    public IEnumerator<IDistanceQuery> GetEnumerator() => Queries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Queries.GetEnumerator();

    public bool Contains(IPrimitive primitive) => Queries.Any(q => q.Contains(primitive));

    public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) {
        Debug.Assert(DistanceDistribution.Contains(sample));
        List<(WeightedPMF<IPrimitive>, double)> pmfs = new();
        var cdf = DistanceDistribution.CumulativeProbability(sample);
        var contains = Queries.Where(d => d.DistanceDistribution.Contains(sample));
        if (contains.Count() == 1) {
            var query = contains.First();
            var queryCdf = query.DistanceDistribution.CumulativeProbability(sample);
            if (queryCdf == 1 || cdf == queryCdf) {
                return query.GetPrimitives(sample);
            }
            var othersCdf = (queryCdf - cdf) / (queryCdf - 1);
            return new WeightedPMF<IPrimitive>((query.GetPrimitives(sample), othersCdf));
        } else {
            foreach (var query in contains) {
                var pmf = query.GetPrimitives(sample);
                var queryCdf = query.DistanceDistribution.CumulativeProbability(sample);
                if (queryCdf == 1) {
                    return pmf;
                }
                var othersCdf = (queryCdf - cdf) / (queryCdf - 1);
                var probabilityDensity = query.DistanceDistribution.Probability(sample) * (1 - othersCdf);
                if (probabilityDensity > 0) {
                    pmfs.Add((pmf, probabilityDensity));
                }
            }
            return new WeightedPMF<IPrimitive>(pmfs.ToArray());
        }
    }
}
