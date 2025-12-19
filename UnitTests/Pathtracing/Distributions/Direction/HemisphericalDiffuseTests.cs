using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.Distributions.Direction;

[TestClass]
public class HemisphericalDiffuseTests {
    [TestMethod]
    public void Sample() {
        var orientation = Random.Shared.Normal3();
        var distribution = new HemisphericalDiffuse(orientation);
        var samples = 10_000;
        var intervalCount = 10;
        var sampleCount = new int[intervalCount];
        for (var i = 0; i < samples; i++) {
            var sample = distribution.Sample(Random.Shared);
            double dot = Normal3.Similarity(orientation, sample);
            var index = (int)(dot * intervalCount);
            sampleCount[index]++;
        }
    }
}
