using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Utilities.Extensions;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct DeltaDistance : IDistanceDistribution, IEquatable<DeltaDistance> {
        public Position1 Distance { get; }
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }
        public Position1 Minimum => Distance;
        public Position1 Maximum => Distance;
        public double DomainSize => Distance.Vector.Value - Distance.Vector.Value.Previous();
        public bool ContainsDelta => true;

        public DeltaDistance(Position1 distance, IMaterial material, IShapeInterval interval) {
            Distance = distance;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) => Distance;

        public double ProbabilityDensity(Position1 sample) => sample.Equals(Distance) ? 1 / DomainSize : 0;

        public double CumulativeProbabilityDensity(Position1 sample) => sample >= Distance ? 1 : 0;

        public bool Contains(Position1 sample) => sample.Equals(Distance);

        public bool Contains(IMaterial material) => material.Equals(Material);

        public bool Contains(IShapeInterval interval) => interval.Equals(Interval);

        public WeightedPMF<IMaterial> GetMaterials(Position1 sample) => new((Material, 1));

        public WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material) => new((Interval, 1));

        public override bool Equals(object? obj) => obj is DeltaDistance dd && Equals(dd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is DeltaDistance dd && Equals(dd);
        public bool Equals(DeltaDistance other) => Distance.Equals(other.Distance) && Material.Equals(other.Material) && Interval.Equals(other.Interval);
        public override int GetHashCode() => HashCode.Combine(818834969, Distance, Material, Interval);

        public static bool operator ==(DeltaDistance left, DeltaDistance right) => left.Equals(right);
        public static bool operator !=(DeltaDistance left, DeltaDistance right) => !(left == right);
    }
}
