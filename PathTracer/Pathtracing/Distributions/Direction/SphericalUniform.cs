using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct SphericalUniform : IDirectionDistribution, IEquatable<SphericalUniform> {
        public Normal3 Orientation => Normal3.UnitY;
        public bool ContainsDelta => false;
        public double DomainSize => 4 * Math.PI;

        public bool Contains(Normal3 sample) => true;

        public double ProbabilityDensity(Normal3 sample) => 1 / DomainSize;

        public Normal3 Sample(Random random) {
            ISphere sphere = new UnitSphere(Position3.Origin);
            return new Normal3(sphere.SurfacePosition(random).Vector);
        }

        public override bool Equals(object? obj) => obj is SphericalUniform su && Equals(su);
        public bool Equals(IProbabilityDistribution<Normal3>? other) => other is SphericalUniform su && Equals(su);
        public bool Equals(SphericalUniform other) => true;
        public override int GetHashCode() => HashCode.Combine(303068573);

        public static bool operator ==(SphericalUniform left, SphericalUniform right) => left.Equals(right);
        public static bool operator !=(SphericalUniform left, SphericalUniform right) => !(left == right);
    }
}
