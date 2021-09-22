using MathNet.Numerics.Distributions;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public struct DistanceMaterial : IDistanceMaterial {
        public double Distance { get; }
        public IMaterial Material { get; }

        public DistanceMaterial(double distance, IMaterial material) {
            Distance = distance;
            Material = material;
        }
    }

    public abstract class DistanceMaterialPDF : IDistanceMaterialPDF {
        public abstract bool SingleSolution { get; }
        public abstract double DomainStart { get; }
        public abstract double DomainEnd { get; }
        public double DomainSize => DomainEnd - DomainStart;
        public abstract bool Leaf { get; }

        public bool Contains(double sample) => !(IsBefore(sample) || IsAfter(sample));
        public bool IsBefore(double sample) => DomainEnd < sample;
        public bool IsAfter(double sample) => DomainStart > sample;

        public abstract double Sample(Random random);
        public abstract double Probability(double sample);
        public abstract double CumulativeDistribution(double sample);

        public abstract bool Contains(IDistanceMaterial sample);
        public abstract IDistanceMaterial SampleWithMaterial(Random random);
        public abstract double Probability(IDistanceMaterial sample);
        public abstract double CumulativeDistribution(IDistanceMaterial sample);
    }

    public class SumDistanceMaterialPDF : DistanceMaterialPDF, IRecursivePDF<double>, IRecursivePDF<IDistanceMaterial> {
        public override bool SingleSolution => throw new NotImplementedException();
        public override bool Leaf => false;
        public IDistanceMaterialPDF Left { get; }
        public IDistanceMaterialPDF Right { get; }

        IPDF<double> IRecursivePDF<double>.Left => Left;
        IPDF<double> IRecursivePDF<double>.Right => Right;
        IPDF<IDistanceMaterial> IRecursivePDF<IDistanceMaterial>.Left => Left;
        IPDF<IDistanceMaterial> IRecursivePDF<IDistanceMaterial>.Right => Right;

        public override double DomainStart => Math.Min(Left.DomainStart, Right.DomainStart);
        public override double DomainEnd => Math.Max(Left.DomainEnd, Right.DomainEnd);

        public SumDistanceMaterialPDF(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            Left = left;
            Right = right;
        }

        public override double Sample(Random random) => Math.Min((Left as IPDF<double>).Sample(random), (Right as IPDF<double>).Sample(random));
        public override double Probability(double sample) => (this as IRecursivePDF<double>).Probability(sample);
        public override double CumulativeDistribution(double sample) => IsAfter(sample) ? 0 : (this as IRecursivePDF<double>).CumulativeDistribution(sample);

        public override bool Contains(IDistanceMaterial sample) => Left.Contains(sample) || Right.Contains(sample);

        public override IDistanceMaterial SampleWithMaterial(Random random) {
            IDistanceMaterial left = Left.SampleWithMaterial(random);
            IDistanceMaterial right = Right.SampleWithMaterial(random);
            return left.Distance < right.Distance ? left : right;
        }

        public override double Probability(IDistanceMaterial sample) {
            return (this as IRecursivePDF<IDistanceMaterial>).Probability(sample);
        }

        public override double CumulativeDistribution(IDistanceMaterial sample) {
            return IsAfter(sample.Distance) ? 0 : (this as IRecursivePDF<IDistanceMaterial>).CumulativeDistribution(sample);
        }
    }

    public class SingleDistanceMaterialPDF : DistanceMaterialPDF {
        public override bool SingleSolution => true;
        public override double DomainStart => DistanceMaterial.Distance;
        public override double DomainEnd => DistanceMaterial.Distance;
        public IDistanceMaterial DistanceMaterial { get; }

        public override bool Leaf => throw new NotImplementedException();

        public SingleDistanceMaterialPDF(IDistanceMaterial distanceMaterial) {
            DistanceMaterial = distanceMaterial;
        }

        public override double Sample(Random random) {
            return DistanceMaterial.Distance;
        }

        public override double Probability(double sample) {
            return sample == DistanceMaterial.Distance ? 1 : 0;
        }

        public override double CumulativeDistribution(double sample) {
            return sample < DistanceMaterial.Distance ? 0 : 1;
        }

        public override bool Contains(IDistanceMaterial sample) {
            return sample == DistanceMaterial;
        }

        public override IDistanceMaterial SampleWithMaterial(Random random) {
            return DistanceMaterial;
        }

        public override double Probability(IDistanceMaterial sample) {
            return sample == DistanceMaterial ? 1 : 0;
        }

        public override double CumulativeDistribution(IDistanceMaterial sample) {
            return sample.Material != DistanceMaterial.Material || sample.Distance < DistanceMaterial.Distance ? 0 : 1;
        }
    }

    public class ExponentialDistanceMaterialPDF : DistanceMaterialPDF {
        public override bool SingleSolution => false;
        public override double DomainStart { get; }
        public override double DomainEnd { get; }
        public Exponential Distribution { get; }
        public IMaterial Material { get; }

        public override bool Leaf => throw new NotImplementedException();

        public ExponentialDistanceMaterialPDF(double start, double end, double rate, IMaterial material) {
            Distribution = new Exponential(rate);
            DomainStart = start;
            DomainEnd = end;
            Material = material;
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

        public override bool Contains(IDistanceMaterial sample) {
            return Material == sample.Material && Contains(sample.Distance);
        }

        public override IDistanceMaterial SampleWithMaterial(Random random) {
            return new DistanceMaterial(Sample(random), Material); 
        }

        public override double Probability(IDistanceMaterial sample) {
            return sample.Material == Material ? Probability(sample.Distance) : 0;
        }

        public override double CumulativeDistribution(IDistanceMaterial sample) {
            return sample.Material == Material ? CumulativeDistribution(sample.Distance) : 0;
        }
    }
}
