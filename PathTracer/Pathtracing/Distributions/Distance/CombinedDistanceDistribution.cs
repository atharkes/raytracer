using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class CombinedDistanceDistribution : IDistanceDistribution, IEnumerable<IDistanceDistribution>, IEquatable<CombinedDistanceDistribution> {
        public IInterval Domain => new IntervalCollection(distributions.Select(x => x.Domain).ToArray());
        public bool ContainsDelta => distributions.Any(d => d.ContainsDelta);

        readonly SortedSet<IDistanceDistribution> distributions;

        public CombinedDistanceDistribution(params IDistanceDistribution[] distributions) {
            List<IDistanceDistribution> simpleDistributions = new();
            foreach(IDistanceDistribution distribution in distributions) {
                if (distribution is CombinedDistanceDistribution combinedDistribution) {
                    simpleDistributions.AddRange(combinedDistribution.distributions);
                } else {
                    simpleDistributions.Add(distribution);
                }
            }
            this.distributions = new(simpleDistributions, Comparer<IDistanceDistribution>.Create((a, b) => a.Domain.Entry.CompareTo(b.Domain.Entry)));
        }

        /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
        /// <param name="distance">The distance to get the material density at</param>
        /// <returns>The material density at the specified <paramref name="distance"/></returns>
        public double MaterialDensity(Position1 distance) => distributions.Sum(d => d.MaterialDensity(distance));

        public double ProbabilityDensity(Position1 sample) {
            double probabilityDensity = 0;
            double cdf = CumulativeProbability(sample);
            if (cdf == 1d) return 0f;
            var contains = distributions.TakeWhile(d => d.Domain.Entry <= sample);
            foreach (var current in contains) {
                double currentCdf = current.CumulativeProbability(sample);
                double othersCdf = (currentCdf - cdf) / (currentCdf - 1);
                probabilityDensity += current.Probability(sample) * (1 - othersCdf);
            }
            return probabilityDensity;
        }

        public double CumulativeProbability(Position1 sample) {
            var relevant = distributions.TakeWhile(d => d.Domain.Entry <= sample);
            double inverseCdf = 1d;
            foreach (var d in relevant) {
                inverseCdf *= 1d - d.CumulativeProbability(sample);
            }
            return 1d - inverseCdf;
        }

        public Position1 Sample(Random random) {
            Position1 smallestSample = Position1.PositiveInfinity;
            foreach (var d in distributions) {
                if (d.Domain.Entry > smallestSample) {
                    break;
                } else {
                    Position1 sample = d.Sample(random);
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
        public override string ToString() => $"Combined[{string.Join(',', distributions)}]";

        public IEnumerator<IDistanceDistribution> GetEnumerator() => distributions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)distributions).GetEnumerator();
    }
}
