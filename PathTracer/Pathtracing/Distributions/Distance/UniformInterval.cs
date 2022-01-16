using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct UniformInterval : IDistanceDistribution {
        public IInterval Interval { get; }

        public Position1 Minimum => Interval.Entry;
        public Position1 Maximum => Interval.Exit;
        public double DomainSize => Maximum - Minimum;
        public bool ContainsDelta => true;

        readonly IMaterial material;
        readonly IShapeInterval shapeInterval;

        public UniformInterval(IInterval interval, IMaterial material, IShapeInterval shapeInterval) {
            Interval = interval;
            this.material = material;
            this.shapeInterval = shapeInterval;
        }

        public Position1 Sample(Random random) {
            return Minimum + (float)(random.NextDouble() * DomainSize);
        }

        public double ProbabilityDensity(Position1 sample) {
            return 1 / DomainSize;
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            if (sample < Minimum) {
                return 0;
            } else if (sample < Maximum) {
                return (sample - Minimum) / DomainSize;
            } else {
                return 1;
            }
        }

        public bool Contains(Position1 sample) => Minimum <= sample && sample <= Maximum;

        public bool Contains(IMaterial material) => material.Equals(this.material);

        public bool Contains(IShapeInterval interval) => interval.Equals(shapeInterval);

        public WeightedPMF<IMaterial> GetMaterials(Position1 sample) => new((material, 1));

        public WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material) => new((shapeInterval, 1));

        public override bool Equals(object? obj) => obj is UniformInterval ud && Equals(ud);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is UniformInterval ud && Equals(ud);
        public bool Equals(UniformInterval other) => Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum) && material.Equals(other.material) && shapeInterval.Equals(other.shapeInterval);
        public override int GetHashCode() => HashCode.Combine(963929819, Minimum, Maximum, material, shapeInterval);



        public static bool operator ==(UniformInterval left, UniformInterval right) => left.Equals(right);
        public static bool operator !=(UniformInterval left, UniformInterval right) => !(left == right);
    }
}
