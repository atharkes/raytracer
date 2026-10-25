using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Tests.Distributions;

public class WeightedPMFTests {
    [Test]
    public async Task ProbabilityMass_ShouldAddTheWeightsOfDuplicateItems() {
        var sut = new WeightedPMF<string>(("a", 1), ("b", 2), ("a", 1));

        await Assert.That(sut.DomainSize).IsEqualTo(2);
        await Assert.That(sut.ProbabilityMass("a")).IsEqualTo(.5);
        await Assert.That(sut.ProbabilityMass("b")).IsEqualTo(.5);
    }

    [Test]
    public async Task ProbabilityMass_ShouldWeighNestedDistributionsByTheirTotalWeight() {
        var ab = new WeightedPMF<string>(("a", 1), ("b", 1));
        var c = new WeightedPMF<string>(("c", 2));

        var sut = new WeightedPMF<string>((ab, 1), (c, .5));

        await Assert.That(sut.ProbabilityMass("a")).IsEqualTo(1d / 3).Within(1e-12);
        await Assert.That(sut.ProbabilityMass("b")).IsEqualTo(1d / 3).Within(1e-12);
        await Assert.That(sut.ProbabilityMass("c")).IsEqualTo(1d / 3).Within(1e-12);
    }

    [Test]
    public async Task Contains_ShouldOnlyHoldTheGivenItems() {
        var sut = new WeightedPMF<string>(("a", 1), ("b", 3));

        await Assert.That(sut.Contains("a")).IsTrue();
        await Assert.That(sut.Contains("c")).IsFalse();
    }

    [Test]
    public async Task Sample_ShouldFollowTheWeights() {
        var sut = new WeightedPMF<string>(("a", 1), ("b", 3));
        var random = new Random(3);

        var b = Enumerable.Range(0, 40_000).Count(_ => sut.Sample(random) == "b");

        await Assert.That(b / 40_000d).IsEqualTo(.75).Within(.01);
    }

    [Test]
    public async Task Sample_ShouldReturnTheOnlyItem() {
        var sut = new WeightedPMF<string>(("a", .2));

        await Assert.That(sut.Sample(new Random(1))).IsEqualTo("a");
        await Assert.That(sut.SingleSolution).IsTrue();
    }
}
