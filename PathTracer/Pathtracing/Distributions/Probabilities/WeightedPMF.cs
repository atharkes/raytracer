using System.Diagnostics;

namespace PathTracer.Pathtracing.Distributions.Probabilities;

/// <summary> A weighted probability mass function (discreet PDF).
/// Sampling can be improved using Vose's Alias Method to O(1). </summary>
/// <typeparam name="T">The type of the <see cref="WeightedPMF{T}"/></typeparam>
public class WeightedPMF<T> : IPMF<T>, IEquatable<WeightedPMF<T>> where T : notnull {
    public bool SingleSolution => count == 1;
    public int DomainSize => count;

    private readonly double totalWeight;
    private readonly Entry[] entries;
    private int count;

    public WeightedPMF(params ReadOnlySpan<(T Item, double Weight)> pairs) {
        foreach (var pair in pairs) totalWeight += pair.Weight;
        Debug.Assert(totalWeight > 0 && double.IsFinite(totalWeight));
        entries = new Entry[pairs.Length];
        foreach ((var item, var weight) in pairs) {
            Add(item, weight);
        }
        var cummulativeProbability = Accumulate();
        Debug.Assert(cummulativeProbability >= 1);
    }

    public WeightedPMF(params ReadOnlySpan<(WeightedPMF<T> PMF, double Weight)> weightedPmfs) {
        var length = 0;
        foreach ((var pmf, var pmfWeight) in weightedPmfs) {
            totalWeight += pmf.totalWeight * pmfWeight;
            length += pmf.count;
        }
        Debug.Assert(totalWeight > 0 && double.IsFinite(totalWeight));
        entries = new Entry[length];
        foreach ((var pmf, var pmfWeight) in weightedPmfs) {
            for (var i = 0; i < pmf.count; i++) {
                Add(pmf.entries[i].Item, pmf.entries[i].Weight * pmfWeight);
            }
        }
        Accumulate();
    }

    private void Add(T item, double weight) {
        var index = IndexOf(item);
        if (index < 0) {
            entries[count++] = new Entry(item, weight);
        } else {
            entries[index].Weight += weight;
        }
    }

    private double Accumulate() {
        double cummulativeProbability = 0;
        for (var i = 0; i < count; i++) {
            cummulativeProbability += entries[i].Weight / totalWeight;
            entries[i].CummulativeProbability = cummulativeProbability;
        }
        return cummulativeProbability;
    }

    private int IndexOf(T item) {
        for (var i = 0; i < count; i++) {
            if (EqualityComparer<T>.Default.Equals(entries[i].Item, item)) return i;
        }
        return -1;
    }

    public bool Contains(T sample) => IndexOf(sample) >= 0;

    public double ProbabilityMass(T sample) {
        var index = IndexOf(sample);
        return index >= 0 ? entries[index].Weight / totalWeight : throw new KeyNotFoundException($"{sample} is not in the {nameof(WeightedPMF<T>)}");
    }

    public T Sample(Random random) {
        var sample = random.NextDouble();
        for (var i = 0; i < count; i++) {
            if (sample < entries[i].CummulativeProbability) {
                return entries[i].Item;
            }
        }
        throw new InvalidOperationException("Probabilities don't add up to 1");
    }

    public override bool Equals(object? obj) => obj is WeightedPMF<T> wpmf && Equals(wpmf);
    public bool Equals(IProbabilityDistribution<T>? other) => other is WeightedPMF<T> wpmf && Equals(wpmf);
    public bool Equals(WeightedPMF<T>? other) => other is not null && entries.Equals(other.entries);
    public override int GetHashCode() => HashCode.Combine(571118491, entries);

    public static bool operator ==(WeightedPMF<T> left, WeightedPMF<T> right) => left.Equals(right);
    public static bool operator !=(WeightedPMF<T> left, WeightedPMF<T> right) => !(left == right);

    private struct Entry(T item, double weight) {
        public readonly T Item = item;
        public double Weight = weight;
        public double CummulativeProbability;
    }
}
