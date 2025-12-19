using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.AccelerationStructure.SBVH;

[TestClass]
public class SpatialBinTest {
    [TestMethod]
    public void Constructor() {
        SpatialBin bin = new(AxisAlignedPlane.X, 0f, 1f);
        Assert.IsNotNull(bin);
    }

    [TestMethod]
    public void SplitPlanes() {
        for (var i = 0; i < 100; i++) {
            var direction = Normal3.UnitX;
            var start = (float)Utils.ThreadRandom.NextDouble();
            var end = (float)Utils.ThreadRandom.NextDouble();
            var bin = new SpatialBin(AxisAlignedPlane.X, start, end);
            Assert.AreEqual((direction * start).Vector, bin.SplitPlaneLeft.Position);
            Assert.AreEqual(direction, bin.SplitPlaneLeft.Normal);
            Assert.AreEqual((direction * end).Vector, bin.SplitPlaneRight.Position);
            Assert.AreEqual(-direction, bin.SplitPlaneRight.Normal);
        }
    }

    [TestMethod]
    public void BoundingBox() {
        var start = -1f;
        var end = 1f;
        var bin = new SpatialBin(AxisAlignedPlane.X, start, end);
        for (var i = 0; i < 1000; i++) {
            bin.Aggregate.Add(Utils.ThreadRandom.Primitive(0f, 1f));
            bin.ClipAndAdd(Utils.ThreadRandom.Primitive(1f, 3f));
        }
        var bounds = bin.Aggregate.Shape.BoundingBox.Bounds;
        Assert.AreEqual(-1f, bounds[0].Vector.X);
        Assert.AreEqual(1f, bounds[1].Vector.X);
    }
}
