using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct HemisphericalCosPower : IDirectionDistribution, IEquatable<HemisphericalCosPower> {
        public Normal3 Orientation { get; }
        public double Exponent { get; }
        public bool ContainsDelta => false;
        public double DomainSize => 2 * Math.PI;

        public HemisphericalCosPower(Normal3 orientation, double exponent) {
            Orientation = orientation;
            Exponent = exponent;
        }

        public bool Contains(Normal3 sample) => IDirection3.InSameClosedHemisphere(Orientation, sample);

        public double ProbabilityDensity(Normal3 sample) => throw new NotImplementedException("Requires complicated normalization factor");

        public Normal3 Sample(Random random) {
            /// Compute Height
            double theta = Math.Pow((random.NextDouble() - 0.5d) * Math.PI, Exponent);
            double r = Math.Cos(theta);
            double w = Math.Sin(theta);
            /// Compute u and v
            double angle = random.NextDouble() * 2 * Math.PI;
            double u = r * Math.Cos(angle);
            double v = r * Math.Sin(angle);
            /// Transform to Orientation
            Normal3 uDirection = Normal3.AnyPerpendicular(Orientation);
            Normal3 vDirection = Normal3.Perpendicular(Orientation, uDirection);
            return new Normal3(uDirection * (float)u + vDirection * (float)v + Orientation * (float)w);
        }

        public override bool Equals(object? obj) => obj is HemisphericalCosPower hcp && Equals(hcp);
        public bool Equals(IProbabilityDistribution<Normal3>? other) => other is HemisphericalCosPower hcp && Equals(hcp);
        public bool Equals(HemisphericalCosPower other) => Orientation.Equals(other.Orientation) && Exponent.Equals(other.Exponent);
        public override int GetHashCode() => HashCode.Combine(100280489, Orientation, Exponent);

        public static bool operator ==(HemisphericalCosPower left, HemisphericalCosPower right) => left.Equals(right);
        public static bool operator !=(HemisphericalCosPower left, HemisphericalCosPower right) => !(left == right);
    }
}
