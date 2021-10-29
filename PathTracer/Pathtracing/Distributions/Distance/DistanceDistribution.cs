using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public abstract class DistanceDistribution : IDistanceDistribution {
        public abstract Position1 Minimum { get; }
        public abstract Position1 Maximum { get; }
        public double DomainSize => Maximum - Minimum;

        public bool Contains(Position1 sample) => (this as ICDF<Position1>).Contains(sample);
        public bool Before(Position1 sample) => (this as ICDF<Position1>).Before(sample);
        public bool After(Position1 sample) => (this as ICDF<Position1>).After(sample);

        public abstract Position1 Sample(Random random);
        public abstract double Probability(Position1 sample);
        public abstract double CumulativeDistribution(Position1 sample);

        public abstract WeightedPMF<IMaterial>? GetMaterials(Position1 sample);
        public abstract WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material);
    }

    public class RecursiveDistanceDistribution : DistanceDistribution, IRecursiveCDF<Position1> {
        public override Position1 Minimum => (this as IRecursiveCDF<Position1>).Minimum;
        public override Position1 Maximum => (this as IRecursiveCDF<Position1>).Maximum;
        public IDistanceDistribution Left { get; }
        public IDistanceDistribution Right { get; }

        ICDF<Position1> IRecursiveCDF<Position1>.Left => Left;
        ICDF<Position1> IRecursiveCDF<Position1>.Right => Right;

        public RecursiveDistanceDistribution(IDistanceDistribution left, IDistanceDistribution right) {
            Left = left;
            Right = right;
        }

        public override Position1 Sample(Random random) => (this as IRecursiveCDF<Position1>).Sample(random);
        public override double Probability(Position1 sample) => (this as IRecursiveCDF<Position1>).Probability(sample);
        public override double CumulativeDistribution(Position1 sample) => (this as IRecursiveCDF<Position1>).CumulativeDistribution(sample);

        public override WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
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

        public override WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
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

    public class SingleDistanceDistribution : DistanceDistribution {
        public override Position1 Minimum => Distance;
        public override Position1 Maximum => Distance;
        public Position1 Distance { get; }
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }

        public SingleDistanceDistribution(Position1 distance, IMaterial material, IShapeInterval interval) {
            Distance = distance;
            Material = material;
            Interval = interval;
        }

        public override Position1 Sample(Random random) {
            return Distance;
        }

        public override double Probability(Position1 sample) {
            return sample == Distance ? 1 : 0;
        }

        public override double CumulativeDistribution(Position1 sample) {
            return sample < Distance ? 0 : 1;
        }

        public override WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return sample == Distance ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public override WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return sample == Distance && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }

    public class ExponentialDistanceDistribution : DistanceDistribution {
        public override Position1 Minimum { get; }
        public override Position1 Maximum { get; }
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

        public override Position1 Sample(Random random) {
            Position1 distance = Minimum + (float)Distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= Maximum ? distance : Position1.PositiveInfinity;

        }

        public override double Probability(Position1 sample) {
            if (Minimum <= sample && sample <= Maximum) {
                return Distribution.Density(sample - Minimum);
            } else if (sample == Position1.PositiveInfinity) {
                return 1 - Distribution.CumulativeDistribution(Maximum - Minimum);
            } else {
                return 0;
            }
        }

        public override double CumulativeDistribution(Position1 distance) {
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

        public override WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return Contains(sample) ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public override WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return Contains(sample) && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }
}
