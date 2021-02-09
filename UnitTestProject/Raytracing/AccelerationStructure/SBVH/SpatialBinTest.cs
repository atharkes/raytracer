using OpenTK;
using PathTracer.Raytracing.AccelerationStructures.SBVH;
using PathTracer.Utilities;
using Xunit;

namespace UnitTests.Raytracing.AccelerationStructure.SBVH {
    public class SpatialBinTest {
        [Fact]
        public void Constructor() {
            SpatialBin bin = new SpatialBin(Vector3.UnitX, 0f, 1f);
            Assert.True(bin != null);
        }

        [Fact]
        public void SplitPlanes() {
            for (int i = 0; i < 100; i++) {
                Vector3 direction = Utils.Random.UnitVector();
                float start = (float)Utils.Random.NextDouble();
                float end = (float)Utils.Random.NextDouble();
                SpatialBin bin = new SpatialBin(direction, start, end);
                Assert.Equal(direction * start, bin.Left.Position);
                Assert.Equal(direction, bin.Left.Normal);
                Assert.Equal(direction * end, bin.Right.Position);
                Assert.Equal(-direction, bin.Right.Normal);
            }
        }

        [Fact]
        public void BoundingBox() {
            Vector3 direction = Vector3.UnitX;
            float start = -1f;
            float end = 1f;
            SpatialBin bin = new SpatialBin(direction, start, end);
            for (int i = 0; i < 1000; i++) {
                bin.AABB.Add(Utils.Random.Sphere(0f, 1f));
                bin.ClipAndAdd(Utils.Random.Primitive(1f, 3f));
            }
            Vector3[] bounds = bin.AABB.Bounds;
            Assert.Equal(-1f, bounds[0].X);
            Assert.Equal(1f, bounds[1].X);
        }
    }
}
