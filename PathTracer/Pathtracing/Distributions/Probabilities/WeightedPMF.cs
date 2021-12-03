using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A weighted probability mass function (discreet PDF).
    /// Sampling can be improved using Vose's Alias Method to O(1). </summary>
    /// <typeparam name="T">The type of the <see cref="WeightedPMF{T}"/></typeparam>
    public class WeightedPMF<T> : IPMF<T>, IEquatable<WeightedPMF<T>> where T : notnull {
        public bool SingleSolution => items.Count == 1;
        public int DomainSize => items.Count;

        readonly double totalWeight;
        readonly HashSet<T> items = new();
        readonly Dictionary<T, double> weights = new();
        readonly Dictionary<T, double> probabilities = new();
        readonly Dictionary<T, double> cummulativeProbabilities = new();

        public WeightedPMF(params (T Item, double Weight)[] pairs) {
            totalWeight = pairs.Sum(i => i.Weight);
            Debug.Assert(double.IsNormal(totalWeight));
            for (int i = 0; i< pairs.Length; i++) {
                (T item, double weight) = pairs[i];
                if (items.Contains(item)) {
                    weights[item] += weight;
                    probabilities[item] = weights[item] / totalWeight;
                } else {
                    items.Add(item);
                    weights.Add(item, weight);
                    probabilities.Add(item, weights[item] / totalWeight);
                }
            }
            double cummulativeProbability = 0;
            foreach (T item in items) {
                cummulativeProbability += probabilities[item];
                cummulativeProbabilities.Add(item, cummulativeProbability);
            }
            Debug.Assert(cummulativeProbability >= 1);
        }

        public WeightedPMF(params (WeightedPMF<T> PMF, double Weight)[] weightedPmfs) {
            totalWeight = weightedPmfs.Sum(p => p.PMF.totalWeight * p.Weight);
            Debug.Assert(double.IsNormal(totalWeight));
            foreach ((WeightedPMF<T> pmf, double pmfWeight) in weightedPmfs) {
                foreach ((T item, double weight) in pmf.weights) {
                    if (items.Contains(item)) {
                        weights[item] += weight * pmfWeight;
                        probabilities[item] = weights[item] / totalWeight;
                    } else {
                        items.Add(item);
                        weights.Add(item, weight * pmfWeight);
                        probabilities.Add(item, weights[item] / totalWeight);
                    }
                }
            }
            double cummulativeProbability = 0;
            foreach (T item in items) {
                cummulativeProbability += probabilities[item];
                cummulativeProbabilities.Add(item, cummulativeProbability);
            }
        }

        public bool Contains(T sample) =>  items.Contains(sample);

        public double ProbabilityMass(T sample) => probabilities[sample];

        public T Sample(Random random) {
            double sample = random.NextDouble();
            foreach ((T item, double cummulativeProbability) in cummulativeProbabilities) {
                if (sample < cummulativeProbability) {
                    return item;
                }
            }
            throw new InvalidOperationException("Probabilities don't add up to 1");
        }

        public override bool Equals(object? obj) => obj is WeightedPMF<T> wpmf && Equals(wpmf);
        public bool Equals(IProbabilityDistribution<T>? other) => other is WeightedPMF<T> wpmf && Equals(wpmf);
        public bool Equals(WeightedPMF<T>? other) => other is not null && weights.Equals(other.weights);
        public override int GetHashCode() => HashCode.Combine(571118491, weights);

        public static bool operator ==(WeightedPMF<T> left, WeightedPMF<T> right) => left.Equals(right);
        public static bool operator !=(WeightedPMF<T> left, WeightedPMF<T> right) => !(left == right);
    }
}
