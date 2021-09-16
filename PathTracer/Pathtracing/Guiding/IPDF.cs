using MathNet.Numerics.Distributions;
using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.Guiding {
    public interface IPDF {
        bool SingleSolution { get; }
        double DomainStart { get; }
        double DomainEnd { get; }
        double DomainSize => DomainEnd - DomainStart;
    }

    public interface IPDF<T> : IPDF {
        (T, double) Sample(Random random);
        double Probability(T sample);
        double CumulativeDistribution(T sample);

        public static IPDF<double> operator* (IPDF<T> left, IPDF<double> right) {
            throw new NotImplementedException();
        }
    }

    public interface IDistancePDF : IPDF<double> {
        public static IDistancePDF operator +(IDistancePDF left, IDistancePDF right) {
            throw new NotImplementedException("Left and right have to be pattern matched. Also: sampling combined exponential pdfs is trouble");
        }
    }

    public class SumDistancePDF : IDistancePDF {
        public IDistancePDF Left { get; }
        public IDistancePDF Right { get; }

        public bool SingleSolution => Left.SingleSolution && Right.SingleSolution;
        public double DomainStart => Math.Min(Left.DomainStart, Right.DomainStart);
        public double DomainEnd => Math.Max(Left.DomainEnd, Right.DomainEnd);

        public SumDistancePDF(IDistancePDF left, IDistancePDF right) {
            Left = left;
            Right = right;
        }

        public (double, double) Sample(Random random) {
            throw new NotImplementedException();
        }

        public double Probability(double sample) {
            throw new NotImplementedException();
        }

        public double CumulativeDistribution(double sample) {
            throw new NotImplementedException();
        }
    }

    public class ProductDistancePDF : IDistancePDF { }

    public class SingleDistancePDF : IDistancePDF {
        public bool SingleSolution => true;
        public double DomainStart => Value;
        public double DomainEnd => Value;
        public double DomainSize => 0f;
        public double Value { get; }

        public SingleDistancePDF(double value) {
            Value = value;
        }

        public (double, double) Sample(Random random) {
            return (Value, 1);
        }

        public double Probability(double sample) {
            return sample == Value ? 1 : 0;
        }

        public double CumulativeDistribution(double sample) {
            return sample < Value ? 0 : 1;
        }
    }

    public class ExponentialDistancePDF : IDistancePDF {
        public bool SingleSolution => false;
        public Exponential Distribution { get; }
        public double DomainStart { get; }
        public double DomainEnd { get; }

        public ExponentialDistancePDF(double start, double end, double rate) {
            Distribution = new Exponential(rate);
            DomainStart = start;
            DomainEnd = end;
        }

        public (double, double) Sample(Random random) {
            double distance = DomainStart + Distribution.InverseCumulativeDistribution(random.NextDouble());
            if (distance > DomainEnd) {
                distance = double.PositiveInfinity;
            }
            return (distance, Probability(distance));
        }

        public double Probability(double sample) {
            if (DomainStart < sample && sample < DomainEnd) {
                return Distribution.Density(sample - DomainStart);
            } else if (double.IsPositiveInfinity(sample)) {
                return 1 - Distribution.CumulativeDistribution(DomainEnd - DomainStart);
            } else {
                return 0;
            }
        }

        public double CumulativeDistribution(double distance) {
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

    public interface IDirectionPDF : IPDF<Vector3> {
        public static IDirectionPDF operator *(IDirectionPDF left, IDirectionPDF right) {
            throw new NotImplementedException();
        }
    }

    public interface IPDF<In, Out> {
        Out Sample(In input, Random random);
        double Probability(In input, Out sample);
    }
}
