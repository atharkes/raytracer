using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class RecursiveDistanceDistribution : IDistanceDistribution, IRecursiveCDF<Position1> {
        public IDistanceDistribution Left { get; }
        public IDistanceDistribution Right { get; }

        ICDF<Position1> IRecursiveCDF<Position1>.Left => Left;
        ICDF<Position1> IRecursiveCDF<Position1>.Right => Right;

        public RecursiveDistanceDistribution(IDistanceDistribution left, IDistanceDistribution right) {
            Left = left;
            Right = right;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
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
            var left = Left.GetShapeIntervals(sample, material);
            var right = Right.GetShapeIntervals(sample, material);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IShapeInterval>((left, probabilityLeft), (right, probabilityRight));
            }
        }
    }

    public class DeltaDistanceDistribution : IDistanceDistribution {
        public Position1 Distance { get; }
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }
        public Position1 Minimum => Distance;
        public Position1 Maximum => Distance;
        public double DomainSize => 0;
        public bool ContainsDelta => true;

        public DeltaDistanceDistribution(Position1 distance, IMaterial material, IShapeInterval interval) {
            Distance = distance;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            return Distance;
        }

        public double ProbabilityDensity(Position1 sample) {
            return sample == Distance ? double.PositiveInfinity : 0;
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            return sample >= Distance ? 1 : 0;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return sample == Distance ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return sample == Distance && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }

    public class ExponentialDistanceDistribution : IDistanceDistribution {
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

        public ExponentialDistanceDistribution(Position1 start, Position1 end, double rate, IMaterial material, IShapeInterval interval) {
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
            return (this as IPDF<Position1>).Contains(sample) && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }
}
