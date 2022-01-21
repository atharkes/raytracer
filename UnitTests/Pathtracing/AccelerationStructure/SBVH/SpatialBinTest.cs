using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.AccelerationStructure.SBVH {
    [TestClass]
    public class SpatialBinTest {
        [TestMethod]
        public void Constructor() {
            SpatialBin bin = new(AxisAlignedPlane.X, 0f, 1f);
            Assert.IsTrue(bin != null);
        }

        [TestMethod]
        public void SplitPlanes() {
            for (int i = 0; i < 100; i++) {
                Normal3 direction = Normal3.UnitX;
                float start = (float)Utils.ThreadRandom.NextDouble();
                float end = (float)Utils.ThreadRandom.NextDouble();
                SpatialBin bin = new(AxisAlignedPlane.X, start, end);
                Assert.AreEqual(direction * start, bin.SplitPlaneLeft.Position);
                Assert.AreEqual(direction, bin.SplitPlaneLeft.Normal);
                Assert.AreEqual(direction * end, bin.SplitPlaneRight.Position);
                Assert.AreEqual(-direction, bin.SplitPlaneRight.Normal);
            }
        }

        [TestMethod]
        public void BoundingBox() {
            float start = -1f;
            float end = 1f;
            SpatialBin bin = new(AxisAlignedPlane.X, start, end);
            for (int i = 0; i < 1000; i++) {
                bin.Aggregate.Add(Utils.ThreadRandom.Primitive(0f, 1f));
                bin.ClipAndAdd(Utils.ThreadRandom.Primitive(1f, 3f));
            }
            Position3[] bounds = bin.Aggregate.Shape.BoundingBox.Bounds;
            Assert.AreEqual(-1f, bounds[0].X);
            Assert.AreEqual(1f, bounds[1].X);
        }
    }
}
