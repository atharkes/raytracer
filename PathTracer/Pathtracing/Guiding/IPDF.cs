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
        T Sample(Random random);
        double Probability(T sample);
        double CumulativeDistribution(T sample);
    }

    public interface IDistancePDF : IPDF<double> {
        public bool IsAfter(double sample);
        public bool IsBefore(double sample);
        public bool Contains(double sample);

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

        public bool IsAfter(double sample) => sample < DomainStart;
        public bool IsBefore(double sample) => sample > DomainEnd;
        public bool Contains(double sample) => DomainStart <= sample && sample <= DomainEnd;

        public abstract double CumulativeDistribution(double sample);
        public abstract double Probability(double sample);
        public abstract double Sample(Random random);
    }

    public class SumDistancePDF : DistancePDF {
        public IDistancePDF Left { get; }
        public IDistancePDF Right { get; }

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
            if (!Contains(sample)) {
                return 0;
            } else {
                double pLeft = Left.Probability(sample);
                double pRight = Right.Probability(sample);
                // If there would be an early out for multiplication with 0 (without having to evaluate the recursive calls), than only the final else statement would be sufficient
                if (pLeft <= 0) {
                    return (1 - Left.CumulativeDistribution(sample)) * pRight;
                } else if (pRight <= 0) {
                    return (1 - Right.CumulativeDistribution(sample)) * pLeft;
                } else {
                    return (1 - Left.CumulativeDistribution(sample)) * pRight + (1 - Right.CumulativeDistribution(sample)) * pLeft;
                }
            }
        }

        public override double CumulativeDistribution(double sample) {
            if (IsAfter(sample)) {
                return 0;
            } else {
                double l = Left.CumulativeDistribution(sample);
                double r = Right.CumulativeDistribution(sample);
                return l + r - l * r;
            }
            
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
