using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;

namespace UnitTests.Pathtracing.SceneObjects {
    [TestClass]
    public class PrimitiveTest {
        [TestMethod]
        public void Constructor() {
            Shape primitive = Utils.Random.Primitive();
        }

        [TestMethod]
        public void Clip() {
            for (int i = 0; i < 100; i++) {
                Shape primitive = Utils.Random.Sphere();
                Vector3[] bounds = primitive.Bounds;
                AxisAlignedPlane plane = new AxisAlignedPlane(Utils.Random.UnitVector(), primitive.Position);
                PrimitiveFragment fragment = primitive.Clip(plane);
                Vector3[] clipBounds = fragment.Bounds;
                Assert.IsTrue(bounds[0] != clipBounds[0] || bounds[1] != clipBounds[1]);
            }
        }
    }
}
