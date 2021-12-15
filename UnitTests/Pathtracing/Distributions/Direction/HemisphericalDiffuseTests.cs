using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Utilities.Extensions;
using System;

namespace UnitTests.Pathtracing.Distributions.Direction {
    [TestClass]
    public class HemisphericalDiffuseTests {
        [TestMethod]
        public void Sample() {
            Normal3 orientation = Random.Shared.Normal3();
            var distribution = new HemisphericalDiffuse(orientation);
            int samples = 10_000;
            int intervalCount = 10;
            int[] sampleCount = new int[intervalCount];
            for (int i = 0; i < samples; i++) {
                Normal3 sample = distribution.Sample(Random.Shared);
                double dot = Normal3.Similarity(orientation, sample);
                int index = (int)(dot * intervalCount);
                sampleCount[index]++;
            }
        }
    }
}
