using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Pathtracing.SceneObjects.Shapes {
    [TestClass]
    public class TriangleTest {
        [TestMethod]
        public void Constructor() {
            Position3 P1 = Utils.ThreadRandom.Vector();
            Position3 P2 = Utils.ThreadRandom.Vector();
            Position3 P3 = Utils.ThreadRandom.Vector();
            Triangle triangle = new(P1, P2, P3);
            Assert.AreEqual(P1, triangle.P1);
            Assert.AreEqual(P2, triangle.P2);
            Assert.AreEqual(P3, triangle.P3);
        }

        [TestMethod]
        public void GetClippedPoints_NoneClipped() {
            AxisAlignedPlane plane = new(Unit3.Z, 0);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
                Position3[] points = triangle.GetClippedPoints(plane).ToArray();
                Assert.AreEqual(points.Length, 3);
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
            Position3[] points = triangle.GetClippedPoints(plane).ToArray();
            Assert.AreEqual(points.Length, 3);
            CollectionAssert.DoesNotContain(points, triangle.P1);
            CollectionAssert.DoesNotContain(points, triangle.P2);
            CollectionAssert.Contains(points, triangle.P3);
            Position3 P11 = Position3.Lerp(P1, P3, 0.5f);
            Position3 P21 = Position3.Lerp(P2, P3, 0.5f);
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
            List<Position3> points = new(triangle.GetClippedPoints(plane));
            Assert.AreEqual(points.Count, 4);
            CollectionAssert.Contains(points, triangle.P1);
            CollectionAssert.Contains(points, triangle.P2);
            CollectionAssert.DoesNotContain(points, triangle.P3);
            Position3 P31 = Position3.Lerp(P3, P2, 0.5f);
            Position3 P32 = Position3.Lerp(P3, P1, 0.5f);
            CollectionAssert.Contains(points, P31);
            CollectionAssert.Contains(points, P32);
        }

        [TestMethod]
        public void GetClippedPoints_AllClipped() {
            AxisAlignedPlane plane = new(Unit3.MinZ, 0);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
                Position3[] points = triangle.GetClippedPoints(plane).ToArray();
                Assert.AreEqual(points.Length, 0);
            }
        }
    }
}
