using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Tests.Distributions;

public class CombinedDistanceQueryTests {
    private static readonly Primitive A = new(new Sphere(new Position3(0, 0, 0)), Material.Diffuse(RGBColors.Gray));
    private static readonly Primitive B = new(new Sphere(new Position3(5, 0, 0)), Material.Diffuse(RGBColors.Gray));

    private static IDistanceQuery Uniform(float entry, float exit, Primitive primitive) => new DistanceQuery(new UniformInterval(new Interval(entry, exit)), primitive);

    [Test]
    public async Task GetPrimitives_ShouldOnlyGiveThePrimitiveCoveringTheDistance() {
        var sut = (Uniform(0, 2, A) + Uniform(1, 3, B))!;

        var primitives = sut.GetPrimitives(.5f);

        await Assert.That(primitives.Contains(B)).IsFalse();
        await Assert.That(primitives.ProbabilityMass(A)).IsEqualTo(1);
    }

    [Test]
    public async Task GetPrimitives_ShouldWeighOverlappingPrimitivesByTheirDensity() {
        var sut = (Uniform(0, 2, A) + Uniform(1, 3, B))!;

        var primitives = sut.GetPrimitives(1.5f);

        await Assert.That(primitives.ProbabilityMass(A)).IsEqualTo(.75).Within(1e-6);
        await Assert.That(primitives.ProbabilityMass(B)).IsEqualTo(.25).Within(1e-6);
    }

    [Test]
    public async Task Addition_ShouldDropAQueryThatStartsAfterTheFirstHasCertainlyEnded() {
        var sut = Uniform(0, 1, A) + Uniform(2, 3, B);

        await Assert.That(sut is DistanceQuery { Primitive: var primitive } && primitive == A).IsTrue();
    }

    [Test]
    public async Task DistanceDistribution_ShouldCombineTheQueries() {
        var sut = (Uniform(0, 2, A) + Uniform(1, 3, B))!;

        await Assert.That(sut.DistanceDistribution.CumulativeProbability(1.5f)).IsEqualTo(1 - .25 * .75).Within(1e-6);
        await Assert.That((float)sut.DistanceDistribution.Domain.Entry).IsEqualTo(0);
    }

    [Test]
    public async Task GetPrimitives_ShouldKeepQueriesThatEnterAtTheSameDistance() {
        var sut = (Uniform(0, 2, A) + Uniform(0, 2, B))!;

        var primitives = sut.GetPrimitives(1);

        await Assert.That(primitives.ProbabilityMass(A)).IsEqualTo(.5).Within(1e-6);
        await Assert.That(primitives.ProbabilityMass(B)).IsEqualTo(.5).Within(1e-6);
    }

    [Test]
    public async Task DistanceDistribution_ShouldKeepQueriesThatEnterAtTheSameDistance() {
        var sut = (Uniform(0, 2, A) + Uniform(0, 2, B))!;

        await Assert.That(sut.DistanceDistribution.CumulativeProbability(1)).IsEqualTo(1 - .5 * .5).Within(1e-6);
    }
}
