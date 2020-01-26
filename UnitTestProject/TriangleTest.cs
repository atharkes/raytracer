using Xunit;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;
using OpenTK;
using WhittedRaytracer.Utilities;

namespace UnitTests {
    public class TriangleTest {
        Triangle RandomTriangle => new Triangle(Utils.RandomVector, Utils.RandomVector, Utils.RandomVector);

        [Fact]
        public void Constructor() {
            Vector3 P1 = Utils.RandomVector;
            Vector3 P2 = Utils.RandomVector;
            Vector3 P3 = Utils.RandomVector;
            Triangle triangle = new Triangle(P1, P2, P3);
            Assert.Equal(P1, triangle.P1);
            Assert.Equal(P2, triangle.P2);
            Assert.Equal(P3, triangle.P3);
        }

        [Fact]
        public void ClipByPlane_NoneClipped() {
            for (int i = 0; i < 100; i++) {
                Triangle triangle = RandomTriangle;
                Vector3 planeNormal = new Vector3(0, 0, 1);
                int points = triangle.ClipByPlane(planeNormal);
                Assert.Equal(3, points);
            }
        }

        [Fact]
        public void ClipByPlane_OneClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            Vector3 planeNormal = new Vector3(0, 0, -1);
            int points = triangle.ClipByPlane(planeNormal);
            Assert.Equal(3, points);
        }

        [Fact]
        public void ClipByPlane_TwoClipped() {
            Vector3 P1 = new Vector3(1, 0, 1);
            Vector3 P2 = new Vector3(0, 0, 1);
            Vector3 P3 = new Vector3(0, 0, -1);
            Triangle triangle = new Triangle(P1, P2, P3);
            Vector3 planeNormal = new Vector3(0, 0, 1);
            int points = triangle.ClipByPlane(planeNormal);
            Assert.Equal(4, points);
        }

        [Fact]
        public void ClipByPlane_AllClipped() {
            for (int i = 0; i < 100; i++) {
                Triangle triangle = RandomTriangle;
                Vector3 planeNormal = new Vector3(0, 0, -1);
                int points = triangle.ClipByPlane(planeNormal);
                Assert.Equal(0, points);
            }
        }
    }
}
