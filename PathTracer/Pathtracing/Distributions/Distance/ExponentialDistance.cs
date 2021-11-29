using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class ExponentialDistance : IDistanceDistribution {
        public Exponential Distribution { get; }
        public Position1 ExponentialStart { get; }
        public Position1 ExponentialEnd { get; }
        public Direction1 ExponentialSize => ExponentialEnd - ExponentialStart;
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }

        public Position1 Minimum => ExponentialStart;
        public Position1 Maximum => Position1.PositiveInfinity;
        public double DomainSize => ExponentialSize;
        public bool ContainsDelta => true;

        public ExponentialDistance(Position1 start, Position1 end, double rate, IMaterial material, IShapeInterval interval) {
            Distribution = new Exponential(rate);
            ExponentialStart = start;
            ExponentialEnd = end;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            Position1 distance = ExponentialStart + (float)Distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= ExponentialEnd ? distance : Position1.PositiveInfinity;
        }

        public double ProbabilityDensity(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return 1 - Distribution.CumulativeDistribution(ExponentialSize);
            } else if (ExponentialStart <= sample && sample <= ExponentialEnd) {
                return Distribution.Density(sample - ExponentialStart);
            } else {
                return 0;
            }
        }

        double IProbabilityDistribution<Position1>.RelativeProbability(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return ProbabilityDensity(sample) * DomainSize;
            } else if (ExponentialStart <= sample && sample <= ExponentialEnd) {
                return ProbabilityDensity(sample) * DomainSize * ExponentialSize;
            } else {
                return 0;
            }
        }

        public double CumulativeProbabilityDensity(Position1 distance) {
            if (distance < Minimum) {
                return 0;
            } else if (distance < ExponentialEnd) {
                return Distribution.CumulativeDistribution(distance - ExponentialStart);
            } else if (distance < double.PositiveInfinity) {
                return Distribution.CumulativeDistribution(ExponentialEnd - ExponentialStart);
            } else {
                return 1;
            }
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return (this as IPDF<Position1>).Contains(sample) ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return (this as IPDF<Position1>).Contains(sample) && material.Equals(Material) ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }
}
