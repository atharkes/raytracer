using MathNet.Numerics.Distributions;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistanceMaterialPDF : IDistancePDF, IPDF<double, IMaterial> {
        public static IDistanceMaterialPDF operator +(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            return new SumDistanceMaterialPDF(left, right);
        }

        public static IDistanceMaterialPDF operator *(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            return new ProductDistanceMaterialPDF(left, right);
        }
    }

    public abstract class DistanceMaterialPDF : IDistanceMaterialPDF {
        public bool SingleSolution { get; }
        public abstract double DomainStart { get; }
        public abstract double DomainEnd { get; }
        public double DomainSize => DomainEnd - DomainStart;
        public abstract bool Leaf { get; }

        public bool Contains(double sample) => !(IsBefore(sample) || IsAfter(sample));
        public bool IsBefore(double sample) => DomainEnd < sample;
        public bool IsAfter(double sample) => DomainStart > sample;

        public abstract double SampleSingle(Random random);
        public abstract double Probability(double sample);
        public abstract double CumulativeDistribution(double sample);

        public abstract bool Contains((double, IMaterial) sample);
        public abstract IPDF<IMaterial> ExtractPDF(double sample);
        public abstract (double, IMaterial) SampleDouble(Random random);
        public abstract double Probability((double, IMaterial) sample);
        public abstract double CumulativeDistribution((double, IMaterial) sample);
    }

    public class SumDistanceMaterialPDF : DistanceMaterialPDF, IRecursivePDF<double>, IRecursivePDF<(double, IMaterial)> {
        public override bool Leaf => false;
        public IDistanceMaterialPDF Left { get; }
        public IDistanceMaterialPDF Right { get; }

        IPDF<double> IRecursivePDF<double>.Left => Left;
        IPDF<double> IRecursivePDF<double>.Right => Right;
        IPDF<(double, IMaterial)> IRecursivePDF<(double, IMaterial)>.Left => Left;
        IPDF<(double, IMaterial)> IRecursivePDF<(double, IMaterial)>.Right => Right;

        public override double DomainStart => Math.Min(Left.DomainStart, Right.DomainStart);

        public override double DomainEnd => Math.Max(Left.DomainEnd, Right.DomainEnd);

        public SumDistanceMaterialPDF(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            Left = left;
            Right = right;
        }

        public override bool Contains((double, IMaterial) sample) {
            return Left.Contains(sample) || Right.Contains(sample);
        }

        public override double SampleSingle(Random random) {
            return Math.Min((Left as IPDF<double>).Sample(random), (Right as IPDF<double>).Sample(random));
        }

        public override double Probability(double sample) {
            return (this as IRecursivePDF<double>).Probability(sample);
        }

        public override double CumulativeDistribution(double sample) {
            return IsAfter(sample) ? 0 : (this as IRecursivePDF<double>).CumulativeDistribution(sample);
        }

        public override IPDF<IMaterial> ExtractPDF(double sample) {
            throw new NotImplementedException("Requires an implementation of a MaterialPDF");
        }

        public override (double, IMaterial) SampleDouble(Random random) {
            (double Distance, IMaterial Material) left = Left.SampleDouble(random);
            (double Distance, IMaterial Material) right = Right.SampleDouble(random);
            return left.Distance < right.Distance ? left : right;
        }

        public override double Probability((double, IMaterial) sample) {
            return (this as IRecursivePDF<(double, IMaterial)>).Probability(sample);
        }

        public override double CumulativeDistribution((double, IMaterial) sample) {
            return IsAfter(sample.Item1) ? 0 : (this as IRecursivePDF<(double, IMaterial)>).CumulativeDistribution(sample);
        }
    }

    public class ProductDistanceMaterialPDF : DistanceMaterialPDF {
        public override bool Leaf => false;
        public IDistanceMaterialPDF Left { get; }
        public IDistanceMaterialPDF Right { get; }

        public ProductDistanceMaterialPDF(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            Left = left;
            Right = right;
        }
    }

    public class DistributionDistanceMaterialPDf : DistanceMaterialPDF {
        public override bool Leaf => true;
        public IContinuousDistribution? Distribution { get; }

        public DistributionDistanceMaterialPDf(IContinuousDistribution distribution) {
            Distribution = distribution;
        }
    }
}
