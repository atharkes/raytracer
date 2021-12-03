using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class CombinedDistanceDistribution : IDistanceDistribution, IEnumerable<IDistanceDistribution>, IEquatable<CombinedDistanceDistribution> {
        public Position1 Minimum => distributions.First().Minimum;
        public Position1 Maximum => distributions.Last().Maximum;
        public bool ContainsDelta => distributions.Any(d => d.ContainsDelta);
        public double DomainSize => throw new NotImplementedException("Union of intervals is not implemented");

        SortedSet<IDistanceDistribution> distributions;

        public CombinedDistanceDistribution(params IDistanceDistribution[] distributions) {
            this.distributions = new(distributions, Comparer<IDistanceDistribution>.Create((a, b) => a.Minimum.CompareTo(b.Minimum)));
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
            Position1 smallestSample = Position1.MaxValue;
            foreach (var d in distributions) {
                if (d.Minimum > smallestSample) {
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

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            List<(WeightedPMF<IMaterial>, double)> pmfs = new();
            double cdf = CumulativeProbabilityDensity(sample);
            var contains = distributions.Where(d => d.Contains(sample));
            foreach (var d in contains) {
                WeightedPMF<IMaterial>? pmf = d.GetMaterials(sample);
                double localCdf = d.CumulativeProbabilityDensity(sample);
                double beforeCdf = (localCdf - cdf) / (localCdf - 1);
                double probabilityDensity = d.Probability(sample) * beforeCdf;
                pmfs.Add()
            }
            var left = Left.GetMaterials(sample);
            var right = Right.GetMaterials(sample);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IMaterial>((left, probabilityLeft), (right, probabilityRight));
            }
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => obj is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(CombinedDistanceDistribution? other) => other is not null && distributions.Equals(other.distributions);
        public override int GetHashCode() => HashCode.Combine(664311359, distributions);

        public IEnumerator<IDistanceDistribution> GetEnumerator() => distributions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)distributions).GetEnumerator();
    }
}
