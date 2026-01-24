using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.SceneObjects;

[TestClass]
public class PrimitiveTest {
    [TestMethod]
    public void Constructor() => Random.Shared.Primitive();

    [TestMethod]
    public void Clip() {
        for (var i = 0; i < 100; i++) {
            var primitive = Random.Shared.Primitive();
            var bounds = primitive.Shape.BoundingBox;
            AxisAlignedPlane plane = new(primitive.Shape.BoundingBox.Center, Random.Shared.Unit());
            var fragments = primitive.Clip(plane);
            foreach (PrimitiveFragment fragment in fragments) {
                var clipBounds = fragment.Shape.BoundingBox;
                Assert.IsTrue(bounds != clipBounds);
            }
        }
    }
}
