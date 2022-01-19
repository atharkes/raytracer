using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
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
                if (distribution is CombinedDistanceDistribution cdd) {
                    simpleDistributions.AddRange(cdd.distributions);
                } else {
                    simpleDistributions.Add(distribution);
                }
            }
            this.distributions = new(simpleDistributions, Comparer<IDistanceDistribution>.Create((a, b) => a.Domain.Entry.CompareTo(b.Domain.Entry)));
        }

        public double ProbabilityDensity(Position1 sample) {
            double probabilityDensity = 0;
            double cdf = CumulativeProbabilityDensity(sample);
            var contains = distributions.TakeWhile(d => !d.Before(sample));
            foreach (var d in contains) {
                double localCdf = d.CumulativeProbabilityDensity(sample);
                double beforeCdf = (localCdf - cdf) / (localCdf - 1);
                probabilityDensity += d.Probability(sample) * beforeCdf;
            }
            return probabilityDensity;
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            var relevant = distributions.TakeWhile(d => !d.Before(sample));
            double inverseCdf = 1;
            foreach (var d in relevant) {
                inverseCdf *= (1 - d.CumulativeProbabilityDensity(sample));
            }
            return 1 - inverseCdf;
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

        public bool Contains(Position1 sample) => distributions.Any(d => d.Contains(sample));

        public override bool Equals(object? obj) => obj is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(CombinedDistanceDistribution? other) => other is not null && distributions.Equals(other.distributions);
        public override int GetHashCode() => HashCode.Combine(664311359, distributions);

        public IEnumerator<IDistanceDistribution> GetEnumerator() => distributions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)distributions).GetEnumerator();
    }
}
