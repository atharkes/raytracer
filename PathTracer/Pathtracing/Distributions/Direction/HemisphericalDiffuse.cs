using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct HemisphericalDiffuse : IDirectionDistribution {
        public Normal3 Orientation { get; }
        public bool ContainsDelta => false;
        public double DomainSize => 2 * Math.PI;

        public HemisphericalDiffuse(Normal3 orientation) {
            Orientation = orientation;
        }

        public bool Contains(Normal3 sample) => IDirection3.InClosedHemisphere(Orientation, sample);

        public double ProbabilityDensity(Normal3 sample) => Math.Abs(IDirection3.Dot(Orientation, sample));

        public Normal3 Sample(Random random) {
            double angle = random.NextDouble() * 2 * Math.PI;
            double r = random.NextDouble() + random.NextDouble();
            r = r < 1 ? r : 2 - r;
            double u = r * Math.Cos(angle);
            double v = r * Math.Sin(angle);
            double w = Math.Sqrt(1 - u * u - v * v);
            Normal3 uDirection = Normal3.AnyPerpendicular(Orientation);
            Normal3 vDirection = Normal3.Perpendicular(Orientation, uDirection);
            return new Normal3(uDirection * (float)u + vDirection * (float)v + Orientation * (float)w);
        }
    }
}
