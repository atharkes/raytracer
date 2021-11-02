using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using PathTracer.Pathtracing.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Utilities;
using System.Collections.Generic;

namespace UnitTests.Pathtracing.SceneObjects.Primitives {
    [TestClass]
    public class TriangleTest {
        [TestMethod]
        public void Constructor() {
            Vector3 P1 = Utils.ThreadRandom.Vector();
            Vector3 P2 = Utils.ThreadRandom.Vector();
            Vector3 P3 = Utils.ThreadRandom.Vector();
            Triangle triangle = new Triangle(P1, P2, P3);
            Assert.AreEqual(P1, triangle.P1);
            Assert.AreEqual(P2, triangle.P2);
            Assert.AreEqual(P3, triangle.P3);
        }

        [TestMethod]
        public void GetClippedPoints_NoneClipped() {
            AxisAlignedPlane plane = new AxisAlignedPlane(new Vector3(0, 0, 1), Vector3.Zero);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
                Vector3[] points = triangle.GetClippedPoints(plane);
                Assert.AreEqual(points.Length, 3);
                Assert.AreEqual(points[0], triangle.P1);
                Assert.AreEqual(points[1], triangle.P2);
                Assert.AreEqual(points[2], triangle.P3);
            }
        }

        [TestMethod]
        public void GetClippedPoints_OneClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            AxisAlignedPlane plane = new AxisAlignedPlane(new Vector3(0, 0, -1), Vector3.Zero);
            Vector3[] points = triangle.GetClippedPoints(plane);
            Assert.AreEqual(points.Length, 3);
            CollectionAssert.DoesNotContain(points, triangle.P1);
            CollectionAssert.DoesNotContain(points, triangle.P2);
            CollectionAssert.Contains(points, triangle.P3);
            Vector3 P11 = Vector3.Lerp(P1, P3, 0.5f);
            Vector3 P21 = Vector3.Lerp(P2, P3, 0.5f);
            CollectionAssert.Contains(points, P11);
            CollectionAssert.Contains(points, P21);
        }

        [TestMethod]
        public void GetClippedPoints_TwoClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            AxisAlignedPlane plane = new AxisAlignedPlane(new Vector3(0, 0, 1), Vector3.Zero);
            List<Vector3> points = new List<Vector3>(triangle.GetClippedPoints(plane));
            Assert.AreEqual(points.Count, 4);
            CollectionAssert.Contains(points, triangle.P1);
            CollectionAssert.Contains(points, triangle.P2);
            CollectionAssert.DoesNotContain(points, triangle.P3);
            Vector3 P31 = Vector3.Lerp(P3, P2, 0.5f);
            Vector3 P32 = Vector3.Lerp(P3, P1, 0.5f);
            CollectionAssert.Contains(points, P31);
            CollectionAssert.Contains(points, P32);
        }

        [TestMethod]
        public void GetClippedPoints_AllClipped() {
            AxisAlignedPlane plane = new AxisAlignedPlane(new Vector3(0, 0, -1), Vector3.Zero);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.ThreadRandom.CreateTriangle(0f, 1f);
                Vector3[] points = triangle.GetClippedPoints(plane);
                Assert.AreEqual(points.Length, 0);
            }
        }
    }
}
