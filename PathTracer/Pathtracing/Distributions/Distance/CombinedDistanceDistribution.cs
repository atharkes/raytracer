using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class CombinedDistanceDistribution : IDistanceDistribution, IEnumerable<IDistanceDistribution>, IEquatable<CombinedDistanceDistribution> {
        IInterval IDistanceDistribution.Domain => new IntervalCollection(distributions.Select(x => x.Domain).ToArray());
        bool IPDF.ContainsDelta => distributions.Any(d => d.ContainsDelta);

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

        public double ProbabilityDensity(Position1 sample) {
            double probabilityDensity = 0;
            double cdf = CumulativeProbability(sample);
            var contains = distributions.TakeWhile(d => d.Domain.Entry <= sample);
            if (contains.Count() == 1) return contains.First().ProbabilityDensity(sample);
            foreach (var current in contains) {
                double currentCdf = current.CumulativeProbability(sample);
                double othersCdf = (currentCdf - cdf) / (currentCdf - 1);
                if (currentCdf == 1 || currentCdf == othersCdf) return current.ProbabilityDensity(sample);
                probabilityDensity += current.Probability(sample) * othersCdf;
            }
            return probabilityDensity;
        }

        public double CumulativeProbability(Position1 sample) {
            var relevant = distributions.TakeWhile(d => d.Domain.Entry <= sample);
            double inverseCdf = 1d;
            foreach (var d in relevant) {
                inverseCdf *= (1d - d.CumulativeProbability(sample));
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

        public IEnumerator<IDistanceDistribution> GetEnumerator() => distributions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)distributions).GetEnumerator();
    }
}
