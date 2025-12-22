using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.Distributions.Direction;

public readonly struct SpecularReflection : IDirectionDistribution, IEquatable<SpecularReflection> {
    public Normal3 Orientation { get; }
    public Normal3 IncomingDirection { get; }
    public Normal3 OutgoingDirection { get; }
    public bool ContainsDelta => true;
    public double DomainSize => 0;

    public SpecularReflection(Normal3 orientation, Normal3 incomingDirection) {
        Orientation = orientation;
        IncomingDirection = incomingDirection;
        OutgoingDirection = Normal3.Reflect(-IncomingDirection, Orientation);
    }

    public bool Contains(Normal3 sample) => sample.Equals(OutgoingDirection);

    public double ProbabilityDensity(Normal3 sample) => double.MaxValue;

    public Normal3 Sample(Random random) => OutgoingDirection;

    public override bool Equals(object? obj) => obj is SpecularReflection sr && Equals(sr);
    public bool Equals(IProbabilityDistribution<Normal3>? other) => other is SpecularReflection sr && Equals(sr);
    public bool Equals(SpecularReflection other) => Orientation.Equals(other.Orientation) && IncomingDirection.Equals(other.IncomingDirection);
    public override int GetHashCode() => HashCode.Combine(263739481, Orientation, IncomingDirection);

    public static bool operator ==(SpecularReflection left, SpecularReflection right) => left.Equals(right);
    public static bool operator !=(SpecularReflection left, SpecularReflection right) => !(left == right);
}
