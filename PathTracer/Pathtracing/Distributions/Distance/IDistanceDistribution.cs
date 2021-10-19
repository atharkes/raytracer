using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public interface IDistanceDistribution : IDistanceCDF, ICDF<IDistanceMaterial> {
        double IPDF<double>.Sample(Random random) => SampleDistance(random);
        double SampleDistance(Random random);

        public static IDistanceDistribution? operator +(IDistanceDistribution? left, IDistanceDistribution? right) {
            return left is null ? right : (right is null ? left : new SumDistanceDistribution(left, right));
        }
    }

    public interface IDistanceMaterial : IComparable<IDistanceMaterial>, IEquatable<IDistanceMaterial> {
        double Distance { get; }
        IMaterial Material { get; }

        void Deconstruct(out double distance, out IMaterial material) {
            distance = Distance;
            material = Material;
        }
    }
}
