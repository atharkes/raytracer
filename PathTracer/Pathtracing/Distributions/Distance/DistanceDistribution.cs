using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
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
                double probabilityLeft = (1 - Right.CumulativeDistribution(sample)) * Left.Probability(sample);
                double probabilityRight = (1 - Left.CumulativeDistribution(sample)) * Right.Probability(sample);
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
                double probabilityLeft = (1 - Right.CumulativeDistribution(sample)) * Left.Probability(sample);
                double probabilityRight = (1 - Left.CumulativeDistribution(sample)) * Right.Probability(sample);
                return new WeightedPMF<IShapeInterval>((left, probabilityLeft), (right, probabilityRight));
            }
        }
    }

    public class SingleDistanceDistribution : IDistanceDistribution {
        public Position1 Minimum => Distance;
        public Position1 Maximum => Distance;
        public Position1 Distance { get; }
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }

        public SingleDistanceDistribution(Position1 distance, IMaterial material, IShapeInterval interval) {
            Distance = distance;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            return Distance;
        }

        public double Probability(Position1 sample) {
            return sample == Distance ? 1 : 0;
        }

        public double CumulativeDistribution(Position1 sample) {
            return sample < Distance ? 0 : 1;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return sample == Distance ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return sample == Distance && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }

    public class ExponentialDistanceDistribution : IDistanceDistribution {
        public Position1 Minimum { get; }
        public Position1 Maximum { get; }
        public Exponential Distribution { get; }
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }

        public ExponentialDistanceDistribution(Position1 start, Position1 end, double rate, IMaterial material, IShapeInterval interval) {
            Distribution = new Exponential(rate);
            Minimum = start;
            Maximum = end;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            Position1 distance = Minimum + (float)Distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= Maximum ? distance : Position1.PositiveInfinity;

        }

        public double Probability(Position1 sample) {
            if (Minimum <= sample && sample <= Maximum) {
                return Distribution.Density(sample - Minimum);
            } else if (sample == Position1.PositiveInfinity) {
                return 1 - Distribution.CumulativeDistribution(Maximum - Minimum);
            } else {
                return 0;
            }
        }

        public double CumulativeDistribution(Position1 distance) {
            if (distance < Minimum) {
                return 0;
            } else if (distance < Maximum) {
                return Distribution.CumulativeDistribution(distance - Minimum);
            } else if (distance < double.PositiveInfinity) {
                return Distribution.CumulativeDistribution(Maximum - Minimum);
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
