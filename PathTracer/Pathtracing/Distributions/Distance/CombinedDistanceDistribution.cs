using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class CombinedDistanceDistribution : IDistanceDistribution, IEnumerable<IDistanceDistribution>, IEquatable<CombinedDistanceDistribution> {
        public Position1 Minimum => distributions.First().Minimum;
        public Position1 Maximum => distributions.Last().Maximum;
        public bool ContainsDelta => distributions.Any(d => d.ContainsDelta);
        public double DomainSize => throw new NotImplementedException("Union of intervals is not implemented");

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
            this.distributions = new(simpleDistributions, Comparer<IDistanceDistribution>.Create((a, b) => a.Minimum.CompareTo(b.Minimum)));
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

        public bool Contains(Position1 sample) => distributions.Any(d => d.Contains(sample));

        public bool Contains(IMaterial material) => distributions.Any(d => d.Contains(material));

        public bool Contains(IShapeInterval interval) => distributions.Any(d => d.Contains(interval));

        public WeightedPMF<IMaterial> GetMaterials(Position1 sample) {
            Debug.Assert(Contains(sample));
            List<(WeightedPMF<IMaterial>, double)> pmfs = new();
            double cdf = CumulativeProbabilityDensity(sample);
            var contains = distributions.Where(d => d.Contains(sample));
            if (contains.Count() == 1) {
                return contains.First().GetMaterials(sample);
            } else {
                foreach (var distribution in contains) {
                    WeightedPMF<IMaterial> pmf = distribution.GetMaterials(sample);
                    double localCdf = distribution.CumulativeProbabilityDensity(sample);
                    if (localCdf == 1) return pmf;
                    double beforeCdf = (localCdf - cdf) / (localCdf - 1);
                    double probabilityDensity = distribution.Probability(sample) * (1 - beforeCdf);
                    pmfs.Add((pmf, probabilityDensity));
                }
                return new WeightedPMF<IMaterial>(pmfs.ToArray());
            }
        }

        public WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material) {
            Debug.Assert(Contains(sample));
            List<(WeightedPMF<IShapeInterval>, double)> pmfs = new();
            double cdf = CumulativeProbabilityDensity(sample);
            var contains = distributions.Where(d => d.Contains(sample));
            if (contains.Count() == 1) {
                return contains.First().GetShapeIntervals(sample, material);
            } else {
                foreach (var d in contains) {
                    WeightedPMF<IShapeInterval> pmf = d.GetShapeIntervals(sample, material);
                    double localCdf = d.CumulativeProbabilityDensity(sample);
                    if (localCdf == 1) return pmf;
                    double beforeCdf = (localCdf - cdf) / (localCdf - 1);
                    double probabilityDensity = d.Probability(sample) * (1 - beforeCdf);
                    pmfs.Add((pmf, probabilityDensity));
                }
                return new WeightedPMF<IShapeInterval>(pmfs.ToArray());
            }
        }

        public override bool Equals(object? obj) => obj is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is CombinedDistanceDistribution cdd && Equals(cdd);
        public bool Equals(CombinedDistanceDistribution? other) => other is not null && distributions.Equals(other.distributions);
        public override int GetHashCode() => HashCode.Combine(664311359, distributions);

        public IEnumerator<IDistanceDistribution> GetEnumerator() => distributions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)distributions).GetEnumerator();
    }
}
