using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A weighted probability mass function (discreet PDF).
    /// Sampling can be improved using Vose's Alias Method to O(1). </summary>
    /// <typeparam name="T">The type of the <see cref="WeightedPMF{T}"/></typeparam>
    public class CombinedProbabilityDistribution<T> : IProbabilityDistribution<T> where T : notnull {
        public bool SingleSolution => items.Count == 1 && items.First().SingleSolution;
        public bool Discreet => items.Any(i => i.Discreet);
        public bool Continuous => items.Any(i => i.Continuous);
        public double DomainSize => throw new NotImplementedException();

        readonly double totalWeight;
        readonly HashSet<IProbabilityDistribution<T>> items = new();
        readonly Dictionary<IProbabilityDistribution<T>, double> weights = new();
        readonly Dictionary<IProbabilityDistribution<T>, double> probabilities = new();
        readonly Dictionary<IProbabilityDistribution<T>, double> cummulativeProbabilities = new();

        public CombinedProbabilityDistribution(params IProbabilityDistribution<T>[] distributions) {
            totalWeight = distributions.Length;
            foreach (IProbabilityDistribution<T> distribution in distributions) {
                if (items.Contains(distribution)) {
                    weights[distribution] += 1;
                    probabilities[distribution] = weights[distribution] / totalWeight;
                } else {
                    items.Add(distribution);
                    weights.Add(distribution, 1);
                    probabilities.Add(distribution, weights[distribution] / totalWeight);
                }
            }
            double cummulativeProbability = 0;
            foreach (IProbabilityDistribution<T> distribution in items) {
                cummulativeProbability += probabilities[distribution];
                cummulativeProbabilities.Add(distribution, cummulativeProbability);
            }
            Debug.Assert(cummulativeProbability >= 1);
        }

        public CombinedProbabilityDistribution(params (IProbabilityDistribution<T> ProbabilityDistribution, double Weight)[] pairs) {
            totalWeight = pairs.Sum(p => p.Weight);
            foreach ((IProbabilityDistribution<T> distribution, double weight) in pairs) {
                if (items.Contains(distribution)) {
                    weights[distribution] += weight;
                    probabilities[distribution] = weights[distribution] / totalWeight;
                } else {
                    items.Add(distribution);
                    weights.Add(distribution, weight);
                    probabilities.Add(distribution, weights[distribution] / totalWeight);
                }
            }
            double cummulativeProbability = 0;
            foreach (IProbabilityDistribution<T> distribution in items) {
                cummulativeProbability += probabilities[distribution];
                cummulativeProbabilities.Add(distribution, cummulativeProbability);
            }
            Debug.Assert(cummulativeProbability >= 1);
        }

        public bool Contains(T sample) => items.Any(d => d.Contains(sample));

        public double Probability(T sample) {
            double probability = 0;
            foreach (IProbabilityDistribution<T> distribution in items) {
                if (distribution.Contains(sample)) {
                    probability += distribution.Probability(sample) * probabilities[distribution];
                }
            }
            return probability;
        }

        public T Sample(Random random) {
            double sample = random.NextDouble();
            foreach ((IProbabilityDistribution<T> distribution, double cummulativeProbability) in cummulativeProbabilities) {
                if (sample < cummulativeProbability) {
                    return distribution.Sample(random);
                }
            }
            throw new InvalidOperationException("Probabilities don't add up to 1");
        }
    }
}
