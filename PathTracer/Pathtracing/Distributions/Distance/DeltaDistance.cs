using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class DeltaDistance : IDistanceDistribution {
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

        public Position1 Sample(Random random) {
            return Distance;
        }

        public double ProbabilityDensity(Position1 sample) {
            return sample == Distance ? 1 / DomainSize : 0;
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            return sample >= Distance ? 1 : 0;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return sample == Distance ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return sample == Distance && material == Material ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }
    }
}
