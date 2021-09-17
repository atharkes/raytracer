using MathNet.Numerics.Distributions;
using System;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistancePDF : IPDF<double> {
        double DomainStart { get; }
        double DomainEnd { get; }

        bool IsBefore(double sample);
        bool IsAfter(double sample);

        public static IDistancePDF operator +(IDistancePDF left, IDistancePDF right) {
            return new SumDistancePDF(left, right);
        }

        public static IDistancePDF operator *(IDistancePDF left, IDistancePDF right) {
            return new ProductDistancePDF(left, right);
        }
    }

    public abstract class DistancePDF : IDistancePDF {
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

    public class SumDistancePDF : DistancePDF, IRecursivePDF<double> {
        public IDistancePDF Left { get; }
        public IDistancePDF Right { get; }

        IPDF<double> IRecursivePDF<double>.Left => Left;
        IPDF<double> IRecursivePDF<double>.Right => Right;

        public override bool SingleSolution => Left.SingleSolution && Right.SingleSolution;
        public override double DomainStart => Math.Min(Left.DomainStart, Right.DomainStart);
        public override double DomainEnd => Math.Max(Left.DomainEnd, Right.DomainEnd);

        public SumDistancePDF(IDistancePDF left, IDistancePDF right) {
            Left = left;
            Right = right;
        }

        public override double Sample(Random random) {
            // Can be optimized by sampling the lower domain first
            return Math.Min(Left.Sample(random), Right.Sample(random));
        }

        public override double Probability(double sample) {
            return (this as IRecursivePDF<double>).Probability(sample);
        }

        public override double CumulativeDistribution(double sample) {
            return IsAfter(sample) ? 0 : (this as IRecursivePDF<double>).CumulativeDistribution(sample);
        }
    }

    public class ProductDistancePDF : DistancePDF {
        public IDistancePDF Left { get; }
        public IDistancePDF Right { get; }

        public override bool SingleSolution => throw new NotImplementedException();
        public override double DomainStart => throw new NotImplementedException();
        public override double DomainEnd => throw new NotImplementedException();

        public ProductDistancePDF(IDistancePDF left, IDistancePDF right) {
            Left = left;
            Right = right;
        }

        public override double Sample(Random random) {
            throw new NotImplementedException();
        }

        public override double Probability(double sample) {
            throw new NotImplementedException();
        }

        public override double CumulativeDistribution(double sample) {
            throw new NotImplementedException();
        }
    }

    public class SingleDistancePDF : DistancePDF {
        public override bool SingleSolution => true;
        public override double DomainStart => Value;
        public override double DomainEnd => Value;
        public double Value { get; }

        public SingleDistancePDF(double value) {
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

    public class ExponentialDistancePDF : DistancePDF {
        public override bool SingleSolution => false;
        public override double DomainStart { get; }
        public override double DomainEnd { get; }
        public Exponential Distribution { get; }

        public ExponentialDistancePDF(double start, double end, double rate) {
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
