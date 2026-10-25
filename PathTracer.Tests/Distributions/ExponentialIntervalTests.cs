using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;

namespace PathTracer.Tests.Distributions;

public class ExponentialIntervalTests {
    private static readonly ExponentialInterval Sut = new(new Interval(1, 11), 2);

    [Test]
    public async Task CumulativeProbability_ShouldFollowTheExponentialFromTheEntry() {
        await Assert.That(Sut.CumulativeProbability(1)).IsEqualTo(0);
        await Assert.That(Sut.CumulativeProbability(2)).IsEqualTo(1 - Math.Exp(-2)).Within(1e-6);
        await Assert.That(Sut.CumulativeProbability(20)).IsEqualTo(1 - Math.Exp(-20)).Within(1e-9);
    }

    [Test]
    public async Task ProbabilityDensity_ShouldFollowTheExponentialInsideTheDomain() {
        await Assert.That(Sut.ProbabilityDensity(2)).IsEqualTo(2 * Math.Exp(-2)).Within(1e-6);
        await Assert.That(Sut.ProbabilityDensity(.5f)).IsEqualTo(0);
        await Assert.That(Sut.ProbabilityDensity(12)).IsEqualTo(0);
    }

    [Test]
    public async Task Sample_ShouldHaveTheExponentialMean_AndPassBeyondTheExit() {
        var random = new Random(9);

        var samples = Enumerable.Range(0, 20_000).Select(_ => (float)Sut.Sample(random)).ToList();

        await Assert.That(samples.Where(float.IsFinite).Average() - 1).IsEqualTo(.5f).Within(.02f);
        await Assert.That(samples.All(s => s is >= 1 and <= 11 || float.IsPositiveInfinity(s))).IsTrue();
    }

    [Test]
    public async Task Constructor_ShouldRejectANegativeDensity() {
        await Assert.That(() => new ExponentialInterval(new Interval(0, 1), -1)).Throws<ArgumentException>();
    }
}
