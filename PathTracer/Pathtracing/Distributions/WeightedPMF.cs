using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A weighted probability mass function (discreet PDF).
    /// Sampling can be improved using Vose's Alias Method to O(1). </summary>
    /// <typeparam name="T">The type of the <see cref="WeightedPMF{T}"/></typeparam>
    public class WeightedPMF<T> : IPDF<T> where T : notnull {
        public bool SingleSolution => items.Length == 1;
        public double DomainSize => items.Length;

        T[] items;
        Dictionary<T, double> weights;
        Dictionary<T, double> probabilities;
        double[] cummulativeProbabilities;

        public WeightedPMF(params (T Item, double Weight)[] items) {
            this.items = new T[items.Length];
            weights = new Dictionary<T, double>(items.Length);
            probabilities = new Dictionary<T, double>(items.Length);
            cummulativeProbabilities = new double[items.Length];
            double totalWeight = items.Sum(i => i.Weight);
            double cummulativeProbability = 0;
            for (int i = 0; i< items.Length; i++) {
                (T item, double weight) = items[i];
                this.items[i] = item;
                weights.Add(item, weight);
                probabilities.Add(item, weight / totalWeight);
                cummulativeProbability += probabilities[item];
                cummulativeProbabilities[i] = cummulativeProbability;
            }
            Debug.Assert(cummulativeProbability >= 1);
        }

        public WeightedPMF(params (WeightedPMF<T> PMF, double Probability)[] items) {
            // Have to use a set to prevent duplicate items (materials) having different probabilities instead of accumulating.
        }

        public bool Contains(T sample) =>  items.Contains(sample);

        public double Probability(T sample) => probabilities[sample];

        public T Sample(Random random) {
            double sample = random.NextDouble();
            for (int i = 0; i < items.Length; i++) {
                if (sample < cummulativeProbabilities[i]) {
                    return items[i];
                }
            }
            throw new InvalidOperationException("Probabilities don't add up to 1");
        }
    }
}
