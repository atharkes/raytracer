using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery;

public class CombinedDistanceQuery : IDistanceQuery, ICollection<IDistanceQuery> {
    /// <summary> The <see cref="IDistanceDistribution"/> of the <see cref="CombinedDistanceQuery"/> </summary>
    public IDistanceDistribution DistanceDistribution => distanceDistribution ??= CombineDistributions();

    public int Count => queries.Count;
    public bool IsReadOnly => false;

    private readonly List<IDistanceQuery> queries;
    private IDistanceDistribution? distanceDistribution;

    public CombinedDistanceQuery(params ReadOnlySpan<IDistanceQuery> queries) {
        this.queries = new(queries.Length);
        foreach (var query in queries) {
            if (query is CombinedDistanceQuery combinedQuery) {
                foreach (var inner in combinedQuery.queries) Insert(inner);
            } else {
                Insert(query);
            }
        }
    }

    private bool Insert(IDistanceQuery query) {
        var entry = query.DistanceDistribution.Domain.Entry;
        var index = queries.Count;
        while (index > 0) {
            var comparison = queries[index - 1].DistanceDistribution.Domain.Entry.CompareTo(entry);
            if (comparison == 0) return false;
            if (comparison < 0) break;
            index--;
        }
        queries.Insert(index, query);
        distanceDistribution = null;
        return true;
    }

    private CombinedDistanceDistribution CombineDistributions() {
        var distributions = new IDistanceDistribution[queries.Count];
        for (var i = 0; i < queries.Count; i++) {
            distributions[i] = queries[i].DistanceDistribution;
        }
        return new CombinedDistanceDistribution(distributions);
    }

    public void Add(IDistanceQuery item) => Insert(item);
    public void Clear() {
        queries.Clear();
        distanceDistribution = null;
    }
    public bool Contains(IDistanceQuery item) => queries.Contains(item);
    public void CopyTo(IDistanceQuery[] array, int arrayIndex) => queries.CopyTo(array, arrayIndex);
    public bool Remove(IDistanceQuery item) {
        if (!queries.Remove(item)) return false;
        distanceDistribution = null;
        return true;
    }
    public IEnumerator<IDistanceQuery> GetEnumerator() => queries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => queries.GetEnumerator();

    public bool Contains(IPrimitive primitive) {
        foreach (var query in queries) {
            if (query.Contains(primitive)) return true;
        }
        return false;
    }

    public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) {
        Debug.Assert(DistanceDistribution.Contains(sample));
        var cdf = DistanceDistribution.CumulativeProbability(sample);
        IDistanceQuery? first = null;
        var containing = 0;
        foreach (var query in queries) {
            if (query.DistanceDistribution.Contains(sample)) {
                first ??= query;
                containing++;
            }
        }
        if (containing == 1) {
            var queryCdf = first!.DistanceDistribution.CumulativeProbability(sample);
            if (queryCdf == 1 || cdf == queryCdf) {
                return first.GetPrimitives(sample);
            }
            var othersCdf = (queryCdf - cdf) / (queryCdf - 1);
            return new WeightedPMF<IPrimitive>((first.GetPrimitives(sample), othersCdf));
        } else {
            List<(WeightedPMF<IPrimitive>, double)> pmfs = new(containing);
            foreach (var query in queries) {
                if (!query.DistanceDistribution.Contains(sample)) continue;
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
            return new WeightedPMF<IPrimitive>(CollectionsMarshal.AsSpan(pmfs));
        }
    }
}
