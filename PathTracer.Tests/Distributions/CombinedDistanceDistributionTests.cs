using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;

namespace PathTracer.Tests.Distributions;

public class CombinedDistanceDistributionTests {
    private static UniformInterval Uniform(float entry, float exit) => new(new Interval(entry, exit));

    [Test]
    public async Task CumulativeProbability_ShouldBeTheChanceThatAnyDistributionHasEnded() {
        var sut = new CombinedDistanceDistribution(Uniform(1, 3), Uniform(0, 2));

        await Assert.That(sut.CumulativeProbability(-1)).IsEqualTo(0);
        await Assert.That(sut.CumulativeProbability(.5f)).IsEqualTo(.25).Within(1e-6);
        await Assert.That(sut.CumulativeProbability(1.5f)).IsEqualTo(1 - .25 * .75).Within(1e-6);
        await Assert.That(sut.CumulativeProbability(2.5f)).IsEqualTo(1).Within(1e-6);
    }

    [Test]
    public async Task ProbabilityDensity_ShouldWeighEachDensityByTheOthersNotHavingEnded() {
        var sut = new CombinedDistanceDistribution(Uniform(1, 3), Uniform(0, 2));

        await Assert.That(sut.ProbabilityDensity(.5f)).IsEqualTo(.5).Within(1e-6);
        await Assert.That(sut.ProbabilityDensity(1.5f)).IsEqualTo(.5 * .75 + .5 * .25).Within(1e-6);
    }

    [Test]
    public async Task Domain_ShouldKeepTheGapBetweenDistributions() {
        IDistanceDistribution sut = new CombinedDistanceDistribution(Uniform(2, 3), Uniform(0, 1));

        await Assert.That((float)sut.Domain.Entry).IsEqualTo(0);
        await Assert.That((float)sut.Domain.Exit).IsEqualTo(3);
        await Assert.That(sut.Contains(.5f)).IsTrue();
        await Assert.That(sut.Contains(1.5f)).IsFalse();
        await Assert.That(sut.Contains(2.5f)).IsTrue();
    }

    [Test]
    public async Task Sample_ShouldBeTheNearestSampleOfTheDistributions() {
        var sut = new CombinedDistanceDistribution(Uniform(2, 3), Uniform(0, 1));
        var random = new Random(5);

        var samples = Enumerable.Range(0, 1_000).Select(_ => (float)sut.Sample(random)).ToList();

        await Assert.That(samples.All(s => s is >= 0 and <= 1)).IsTrue();
    }

    [Test]
    public async Task Constructor_ShouldFlattenCombinedDistributions() {
        var inner = new CombinedDistanceDistribution(Uniform(1, 3), Uniform(0, 2));

        var sut = new CombinedDistanceDistribution(inner, Uniform(.5f, 4));

        await Assert.That(sut.Count()).IsEqualTo(3);
        await Assert.That(sut.Select(d => (float)d.Domain.Entry)).IsEquivalentTo([0f, .5f, 1f]);
    }
}
