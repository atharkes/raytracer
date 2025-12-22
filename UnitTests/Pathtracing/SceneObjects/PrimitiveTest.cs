using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.SceneObjects;

[TestClass]
public class PrimitiveTest {
    [TestMethod]
    public void Constructor() => Utils.ThreadRandom.Primitive();

    [TestMethod]
    public void Clip() {
        for (var i = 0; i < 100; i++) {
            var primitive = Utils.ThreadRandom.Primitive();
            var bounds = primitive.Shape.BoundingBox;
            AxisAlignedPlane plane = new(primitive.Shape.BoundingBox.Center, Utils.ThreadRandom.Unit());
            var fragments = primitive.Clip(plane);
            foreach (PrimitiveFragment fragment in fragments) {
                var clipBounds = fragment.Shape.BoundingBox;
                Assert.IsTrue(bounds != clipBounds);
            }
        }
    }
}
