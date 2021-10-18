using MathNet.Numerics.Distributions;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public abstract class DistanceCDF : IDistanceCDF {
        public abstract bool SingleSolution { get; }
        public abstract double DomainStart { get; }
        public abstract double DomainEnd { get; }
        public double DomainSize => DomainEnd - DomainStart;

        public bool Contains(double sample) => !(IsBefore(sample) || IsAfter(sample));
        public bool IsBefore(double sample) => DomainEnd < sample;
        public bool IsAfter(double sample) => DomainStart > sample;

        public abstract double Sample(Random random);
        public abstract double Probability(double sample);
        public abstract double CumulativeDistribution(double sample);
    }

    public class SumDistanceCDF : DistanceCDF, ISumDistanceCDF<double> {
        public IDistanceCDF Left { get; }
        public IDistanceCDF Right { get; }

        ICDF<double> IRecursiveCDF<double>.Left => Left;
        ICDF<double> IRecursiveCDF<double>.Right => Right;

        public override bool SingleSolution => Left.SingleSolution && Right.SingleSolution;
        public override double DomainStart => Math.Min(Left.DomainStart, Right.DomainStart);
        public override double DomainEnd => Math.Max(Left.DomainEnd, Right.DomainEnd);

        public SumDistanceCDF(IDistanceCDF left, IDistanceCDF right) {
            Left = left;
            Right = right;
        }

        public override double Sample(Random random) {
            // Can be optimized by sampling the lower domain first
            return Math.Min(Left.Sample(random), Right.Sample(random));
        }

        public override double Probability(double sample) {
            return (this as IRecursiveCDF<double>).Probability(sample);
        }

        public override double CumulativeDistribution(double sample) {
            return IsAfter(sample) ? 0 : (this as IRecursiveCDF<double>).CumulativeDistribution(sample);
        }
    }

    public class SingleDistanceCDF : DistanceCDF {
        public override bool SingleSolution => true;
        public override double DomainStart => Value;
        public override double DomainEnd => Value;
        public double Value { get; }

        public SingleDistanceCDF(double value) {
            Value = value;
        }

        public override double Sample(Random random) {
            return Value;
        }

        public override double Probability(double sample) {
            return sample == Value ? 1 : 0;
        }

        public override double CumulativeDistribution(double sample) {
            return sample < Value ? 0 : 1;
        }
    }

    public class ExponentialDistanceCDF : DistanceCDF {
        public override bool SingleSolution => false;
        public override double DomainStart { get; }
        public override double DomainEnd { get; }
        public Exponential Distribution { get; }

        public ExponentialDistanceCDF(double start, double end, double rate) {
            Distribution = new Exponential(rate);
            DomainStart = start;
            DomainEnd = end;
        }

        public override double Sample(Random random) {
            double distance = DomainStart + Distribution.InverseCumulativeDistribution(random.NextDouble());
            if (distance > DomainEnd) {
                distance = double.PositiveInfinity;
            }
            return distance;
        }

        public override double Probability(double sample) {
            if (DomainStart < sample && sample < DomainEnd) {
                return Distribution.Density(sample - DomainStart);
            } else if (double.IsPositiveInfinity(sample)) {
                return 1 - Distribution.CumulativeDistribution(DomainEnd - DomainStart);
            } else {
                return 0;
            }
        }

        public override double CumulativeDistribution(double distance) {
            if (distance < DomainStart) {
                return 0;
            } else if (distance < DomainEnd) {
                return Distribution.CumulativeDistribution(distance - DomainStart);
            } else if (distance < double.PositiveInfinity) {
                return Distribution.CumulativeDistribution(DomainEnd - DomainStart);
            } else {
                return 1;
            }
        }
    }
}
