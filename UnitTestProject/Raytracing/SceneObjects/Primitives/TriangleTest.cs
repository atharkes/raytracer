using OpenTK;
using PathTracer.Raytracing.AccelerationStructures.SBVH;
using PathTracer.Raytracing.SceneObjects.Primitives;
using PathTracer.Utilities;
using Xunit;

namespace UnitTests.Raytracing.SceneObjects.Primitives {
    public class TriangleTest {
        [Fact]
        public void Constructor() {
            Vector3 P1 = Utils.Random.Vector();
            Vector3 P2 = Utils.Random.Vector();
            Vector3 P3 = Utils.Random.Vector();
            Triangle triangle = new Triangle(P1, P2, P3);
            Assert.Equal(P1, triangle.P1);
            Assert.Equal(P2, triangle.P2);
            Assert.Equal(P3, triangle.P3);
        }

        [Fact]
        public void GetClippedPoints_NoneClipped() {
            SplitPlane plane = new SplitPlane(new Vector3(0, 0, 1), Vector3.Zero);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.Random.Triangle(0f, 1f);
                Vector3[] points = triangle.GetClippedPoints(plane);
                Assert.Equal(3, points.Length);
                Assert.Equal(triangle.P1, points[0]);
                Assert.Equal(triangle.P2, points[1]);
                Assert.Equal(triangle.P3, points[2]);
            }
        }

        [Fact]
        public void GetClippedPoints_OneClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            SplitPlane plane = new SplitPlane(new Vector3(0, 0, -1), Vector3.Zero);
            Vector3[] points = triangle.GetClippedPoints(plane);
            Assert.Equal(3, points.Length);
            Assert.DoesNotContain(triangle.P1, points);
            Assert.DoesNotContain(triangle.P2, points);
            Assert.Contains(triangle.P3, points);
            Vector3 P11 = Vector3.Lerp(P1, P3, 0.5f);
            Vector3 P21 = Vector3.Lerp(P2, P3, 0.5f);
            Assert.Contains(P11, points);
            Assert.Contains(P21, points);
        }

        [Fact]
        public void GetClippedPoints_TwoClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            SplitPlane plane = new SplitPlane(new Vector3(0, 0, 1), Vector3.Zero);
            Vector3[] points = triangle.GetClippedPoints(plane);
            Assert.Equal(4, points.Length);
            Assert.Contains(triangle.P1, points);
            Assert.Contains(triangle.P2, points);
            Assert.DoesNotContain(triangle.P3, points);
            Vector3 P31 = Vector3.Lerp(P3, P2, 0.5f);
            Vector3 P32 = Vector3.Lerp(P3, P1, 0.5f);
            Assert.Contains(P31, points);
            Assert.Contains(P32, points);
        }

        [Fact]
        public void GetClippedPoints_AllClipped() {
            SplitPlane plane = new SplitPlane(new Vector3(0, 0, -1), Vector3.Zero);
            for (int i = 0; i < 100; i++) {
                Triangle triangle = Utils.Random.Triangle(0f, 1f);
                Vector3[] points = triangle.GetClippedPoints(plane);
                Assert.Empty(points);
            }
        }
    }
}
