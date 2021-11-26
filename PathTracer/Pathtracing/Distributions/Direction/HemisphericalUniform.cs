using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct HemisphericalUniform : IDirectionDistribution {
        public Normal3 Orientation { get; }
        public bool ContainsDelta => false;
        public double DomainSize => 2 * Math.PI;

        public HemisphericalUniform(Normal3 orientation) {
            Orientation = orientation;
        }

        public bool Contains(Normal3 sample) => IDirection3.InSameClosedHemisphere(Orientation, sample);

        public double ProbabilityDensity(Normal3 sample) => Contains(sample) ? 1 / DomainSize : 0;

        public Normal3 Sample(Random random) {
            ISphere sphere = new UnitSphere(Position3.Origin);
            Normal3 direction = ((Direction3)sphere.SurfacePosition(random)).Normalized();
            return IDirection3.InSameClosedHemisphere(Orientation, direction) ? direction : -direction;
        }
    }
}
