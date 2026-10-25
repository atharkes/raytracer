using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System.Collections;

namespace PathTracer.Pathtracing.Distributions.Distance;

public class CombinedDistanceDistribution : IDistanceDistribution, IEnumerable<IDistanceDistribution>, IEquatable<CombinedDistanceDistribution> {
    public IInterval Domain => domain ??= new IntervalCollection(Domains());
    public bool ContainsDelta {
        get {
            foreach (var distribution in distributions) {
                if (distribution.ContainsDelta) return true;
            }
            return false;
        }
    }

    private readonly IDistanceDistribution[] distributions;
    private IInterval? domain;

    public CombinedDistanceDistribution(params ReadOnlySpan<IDistanceDistribution> distributions) {
        var length = 0;
        foreach (var distribution in distributions) {
            length += distribution is CombinedDistanceDistribution combinedDistribution ? combinedDistribution.distributions.Length : 1;
        }
        this.distributions = new IDistanceDistribution[length];
        var count = 0;
        foreach (var distribution in distributions) {
            if (distribution is CombinedDistanceDistribution combinedDistribution) {
                foreach (var inner in combinedDistribution.distributions) {
                    Insert(inner, count++);
                }
            } else {
                Insert(distribution, count++);
            }
        }
    }

    private void Insert(IDistanceDistribution distribution, int count) {
        var entry = distribution.Domain.Entry;
        var index = count;
        while (index > 0 && distributions[index - 1].Domain.Entry.CompareTo(entry) > 0) {
            distributions[index] = distributions[index - 1];
            index--;
        }
        distributions[index] = distribution;
    }

    private IInterval[] Domains() {
        var domains = new IInterval[distributions.Length];
        for (var i = 0; i < distributions.Length; i++) {
            domains[i] = distributions[i].Domain;
        }
        return domains;
    }

    /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
    /// <param name="distance">The distance to get the material density at</param>
    /// <returns>The material density at the specified <paramref name="distance"/></returns>
    public double MaterialDensity(Position1 distance) {
        double materialDensity = 0;
        foreach (var distribution in distributions) {
            materialDensity += distribution.MaterialDensity(distance);
        }
        return materialDensity;
    }

    public bool Contains(Position1 sample) {
        foreach (var distribution in distributions) {
            if (distribution.Domain.Includes(sample)) return true;
        }
        return false;
    }

    public double ProbabilityDensity(Position1 sample) {
        double probabilityDensity = 0;
        var cdf = CumulativeProbability(sample);
        if (cdf == 1d) return 0f;
        foreach (var current in distributions) {
            if (current.Domain.Entry > sample) break;
            var currentCdf = current.CumulativeProbability(sample);
            var othersCdf = (currentCdf - cdf) / (currentCdf - 1);
            probabilityDensity += current.Probability(sample) * (1 - othersCdf);
        }
        return probabilityDensity;
    }

    public double CumulativeProbability(Position1 sample) {
        var inverseCdf = 1d;
        foreach (var d in distributions) {
            if (d.Domain.Entry > sample) break;
            inverseCdf *= 1d - d.CumulativeProbability(sample);
        }
        return 1d - inverseCdf;
    }

    public Position1 Sample(Random random) {
        var smallestSample = Position1.PositiveInfinity;
        foreach (var d in distributions) {
            if (d.Domain.Entry > smallestSample) {
                break;
            } else {
                var sample = d.Sample(random);
                if (smallestSample > sample) {
                    smallestSample = sample;
                }
            }
        }
        return smallestSample;
    }

    public override bool Equals(object? obj) => obj is CombinedDistanceDistribution cdd && Equals(cdd);
    public bool Equals(IProbabilityDistribution<Position1>? other) => other is CombinedDistanceDistribution cdd && Equals(cdd);
    public bool Equals(CombinedDistanceDistribution? other) => other is not null && distributions.Equals(other.distributions);
    public override int GetHashCode() => HashCode.Combine(664311359, distributions);
    public override string ToString() => $"Combined[{string.Join(',', (IEnumerable<IDistanceDistribution>)distributions)}]";

    public IEnumerator<IDistanceDistribution> GetEnumerator() => ((IEnumerable<IDistanceDistribution>)distributions).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => distributions.GetEnumerator();
}
