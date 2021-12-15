using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct HemisphericalDiffuse : IDirectionDistribution, IEquatable<HemisphericalDiffuse> {
        public Normal3 Orientation { get; }
        public bool ContainsDelta => false;
        public double DomainSize => 2 * Math.PI;

        public HemisphericalDiffuse(Normal3 orientation) {
            Orientation = orientation;
        }

        public bool Contains(Normal3 sample) => IDirection3.InSameClosedHemisphere(Orientation, sample);

        public double ProbabilityDensity(Normal3 sample) => Math.Abs(IDirection3.Dot(Orientation, sample));

        public Normal3 Sample(Random random) {
            /// Get Point uniformly on Disc
            double angle = random.NextDouble() * 2 * Math.PI;
            double r = random.NextDouble() + random.NextDouble();
            r = r < 1 ? r : 2 - r;
            double u = r * Math.Cos(angle);
            double v = r * Math.Sin(angle);
            /// Raise Disc point to Hemisphere
            double w = Math.Sqrt(1 - u * u - v * v);
            Normal3 uDirection = Normal3.AnyPerpendicular(Orientation);
            Normal3 vDirection = Normal3.Perpendicular(Orientation, uDirection);
            return new Normal3(uDirection * (float)u + vDirection * (float)v + Orientation * (float)w);
        }

        public override bool Equals(object? obj) => obj is HemisphericalDiffuse hd && Equals(hd);
        public bool Equals(IProbabilityDistribution<Normal3>? other) => other is HemisphericalDiffuse hd && Equals(hd);
        public bool Equals(HemisphericalDiffuse other) => Orientation.Equals(other.Orientation);
        public override int GetHashCode() => HashCode.Combine(859190539, Orientation);

        public static bool operator ==(HemisphericalDiffuse left, HemisphericalDiffuse right) => left.Equals(right);
        public static bool operator !=(HemisphericalDiffuse left, HemisphericalDiffuse right) => !(left == right);
    }
}
