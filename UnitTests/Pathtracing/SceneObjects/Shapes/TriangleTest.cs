using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.SceneObjects.Shapes;

[TestClass]
public class TriangleTest {
    [TestMethod]
    public void Constructor() {
        var P1 = Utils.ThreadRandom.Vector();
        var P2 = Utils.ThreadRandom.Vector();
        var P3 = Utils.ThreadRandom.Vector();
        Triangle triangle = new(P1, P2, P3);
        Assert.AreEqual(P1, triangle.P1);
        Assert.AreEqual(P2, triangle.P2);
        Assert.AreEqual(P3, triangle.P3);
    }

    [TestMethod]
    public void GetClippedPoints_NoneClipped() {
        var plane = new AxisAlignedPlane(Unit3.Z, 0);
        for (var i = 0; i < 100; i++) {
            var triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
            var points = triangle.GetClippedPoints(plane).ToArray();
            Assert.HasCount(3, points);
            Assert.AreEqual(points[0], triangle.P1);
            Assert.AreEqual(points[1], triangle.P2);
            Assert.AreEqual(points[2], triangle.P3);
        }
    }

    [TestMethod]
    public void GetClippedPoints_OneClipped() {
        Position3 P1 = new(1, 0, 1);
        Position3 P2 = new(0, 0, 1);
        Position3 P3 = new(0, 0, -1);
        Triangle triangle = new(P1, P2, P3);
        AxisAlignedPlane plane = new(Unit3.MinZ, 0);
        var points = triangle.GetClippedPoints(plane).ToArray();
        Assert.HasCount(3, points);
        CollectionAssert.DoesNotContain(points, triangle.P1);
        CollectionAssert.DoesNotContain(points, triangle.P2);
        CollectionAssert.Contains(points, triangle.P3);
        var P11 = Position3.Lerp(P1, P3, 0.5f);
        var P21 = Position3.Lerp(P2, P3, 0.5f);
        CollectionAssert.Contains(points, P11);
        CollectionAssert.Contains(points, P21);
    }

    [TestMethod]
    public void GetClippedPoints_TwoClipped() {
        Position3 P1 = new(1, 0, 1);
        Position3 P2 = new(0, 0, 1);
        Position3 P3 = new(0, 0, -1);
        Triangle triangle = new(P1, P2, P3);
        AxisAlignedPlane plane = new(Unit3.Z, 0);
        List<Position3> points = [.. triangle.GetClippedPoints(plane)];
        Assert.HasCount(4, points);
        CollectionAssert.Contains(points, triangle.P1);
        CollectionAssert.Contains(points, triangle.P2);
        CollectionAssert.DoesNotContain(points, triangle.P3);
        var P31 = Position3.Lerp(P3, P2, 0.5f);
        var P32 = Position3.Lerp(P3, P1, 0.5f);
        CollectionAssert.Contains(points, P31);
        CollectionAssert.Contains(points, P32);
    }

    [TestMethod]
    public void GetClippedPoints_AllClipped() {
        AxisAlignedPlane plane = new(Unit3.MinZ, 0);
        for (var i = 0; i < 100; i++) {
            var triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
            var points = triangle.GetClippedPoints(plane).ToArray();
            Assert.IsEmpty(points);
        }
    }
}
