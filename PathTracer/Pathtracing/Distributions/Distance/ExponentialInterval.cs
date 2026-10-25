using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.Distributions.Distance;

public readonly struct ExponentialInterval : IDistanceDistribution, IEquatable<ExponentialInterval> {
    public IInterval Domain { get; }
    public double Density { get; }

    public ExponentialInterval(IInterval interval, double density) {
        if (!(density >= 0)) throw new ArgumentOutOfRangeException(nameof(density), density, "The density of an exponential distribution cannot be negative");
        Domain = interval;
        Density = density;
    }

    /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
    /// <param name="distance">The distance to get the material density at</param>
    /// <returns>The material density at the specified <paramref name="distance"/></returns>
    public double MaterialDensity(Position1 distance) => Domain.Includes(distance) ? Density : 0d;

    public Position1 Sample(Random random) {
        var distance = Domain.Entry + (float)InverseCumulativeDistribution(random.NextDouble());
        return float.IsNaN(distance)
            ? throw new InvalidOperationException($"Exponential distribution returned invalid distance {distance}")
            : distance <= Domain.Exit ? distance : Position1.PositiveInfinity;
    }

    public double ProbabilityDensity(Position1 sample) => Domain.Includes(sample) ? DensityAt(sample - Domain.Entry) : 0;

    public double CumulativeProbability(Position1 sample) => sample <= Domain.Entry
            ? 0
            : sample <= Domain.Exit
                ? CumulativeDistribution(((float)sample).Previous() - Domain.Entry)
                : CumulativeDistribution(Domain.CoveredArea);

    private double InverseCumulativeDistribution(double probability) => probability >= 1 ? double.PositiveInfinity : -Math.Log(1 - probability) / Density;
    private double CumulativeDistribution(double distance) => distance < 0 ? 0 : 1 - Math.Exp(-Density * distance);
    private double DensityAt(double distance) => distance < 0 ? 0 : Density * Math.Exp(-Density * distance);

    public override bool Equals(object? obj) => obj is ExponentialInterval ed && Equals(ed);
    public bool Equals(IProbabilityDistribution<Position1>? other) => other is ExponentialInterval ed && Equals(ed);
    public bool Equals(ExponentialInterval other) => Domain.Equals(other.Domain) && Density.Equals(other.Density);
    public override int GetHashCode() => HashCode.Combine(973102703, Domain, Density);
    public override string ToString() => $"Exponential{Density}{Domain}";

    public static bool operator ==(ExponentialInterval left, ExponentialInterval right) => left.Equals(right);
    public static bool operator !=(ExponentialInterval left, ExponentialInterval right) => !(left == right);
}
