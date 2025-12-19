using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.Distributions.Distance;

public readonly struct ExponentialInterval : IDistanceDistribution, IEquatable<ExponentialInterval> {
    public IInterval Domain { get; }
    public double Density { get; }

    private readonly Exponential distribution;

    public ExponentialInterval(IInterval interval, double density) {
        Domain = interval;
        Density = density;
        distribution = new Exponential(density);
    }

    /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
    /// <param name="distance">The distance to get the material density at</param>
    /// <returns>The material density at the specified <paramref name="distance"/></returns>
    public double MaterialDensity(Position1 distance) => Domain.Includes(distance) ? Density : 0d;

    public Position1 Sample(Random random) {
        var distance = Domain.Entry + (float)distribution.InverseCumulativeDistribution(random.NextDouble());
        return float.IsNaN(distance)
            ? throw new InvalidOperationException($"Exponential distribution returned invalid distance {distance}")
            : distance <= Domain.Exit ? distance : Position1.PositiveInfinity;
    }

    public double ProbabilityDensity(Position1 sample) => Domain.Includes(sample) ? distribution.Density(sample - Domain.Entry) : 0;

    public double CumulativeProbability(Position1 sample) => sample <= Domain.Entry
            ? 0
            : sample <= Domain.Exit
                ? distribution.CumulativeDistribution(((float)sample).Previous() - Domain.Entry)
                : distribution.CumulativeDistribution(Domain.CoveredArea);

    public override bool Equals(object? obj) => obj is ExponentialInterval ed && Equals(ed);
    public bool Equals(IProbabilityDistribution<Position1>? other) => other is ExponentialInterval ed && Equals(ed);
    public bool Equals(ExponentialInterval other) => Domain.Equals(other.Domain) && Density.Equals(other.Density);
    public override int GetHashCode() => HashCode.Combine(973102703, Domain, Density);
    public override string ToString() => $"Exponential{Density}{Domain}";

    public static bool operator ==(ExponentialInterval left, ExponentialInterval right) => left.Equals(right);
    public static bool operator !=(ExponentialInterval left, ExponentialInterval right) => !(left == right);
}
