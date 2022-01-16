using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Utilities.Extensions;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct DeltaDistance : IDistanceDistribution, IEquatable<DeltaDistance> {
        public Position1 Distance { get; }
        public IInterval Interval { get; }
        
        public Position1 Minimum => Distance;
        public Position1 Maximum => Distance;
        public double DomainSize => Distance.Vector.Value - Distance.Vector.Value.Previous();
        public bool ContainsDelta => true;

        readonly IMaterial material;
        readonly IShapeInterval shapeInterval;

        public DeltaDistance(Position1 distance, IMaterial material, IShapeInterval shapeInterval) {
            Distance = distance;
            Interval = new Interval(distance, distance);
            this.material = material;
            this.shapeInterval = shapeInterval;
        }

        public Position1 Sample(Random random) => Distance;

        public double ProbabilityDensity(Position1 sample) => sample.Equals(Distance) ? 1 / DomainSize : 0;

        public double CumulativeProbabilityDensity(Position1 sample) => sample >= Distance ? 1 : 0;

        public bool Contains(Position1 sample) => sample.Equals(Distance);

        public bool Contains(IMaterial material) => material.Equals(this.material);

        public bool Contains(IShapeInterval interval) => interval.Equals(shapeInterval);

        public WeightedPMF<IMaterial> GetMaterials(Position1 sample) => new((material, 1));

        public WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material) => new((shapeInterval, 1));

        public override bool Equals(object? obj) => obj is DeltaDistance dd && Equals(dd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is DeltaDistance dd && Equals(dd);
        public bool Equals(DeltaDistance other) => Distance.Equals(other.Distance) && material.Equals(other.material) && shapeInterval.Equals(other.shapeInterval);
        public override int GetHashCode() => HashCode.Combine(818834969, Distance, material, shapeInterval);

        public static bool operator ==(DeltaDistance left, DeltaDistance right) => left.Equals(right);
        public static bool operator !=(DeltaDistance left, DeltaDistance right) => !(left == right);
    }
}
