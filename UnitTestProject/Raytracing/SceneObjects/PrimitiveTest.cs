using OpenTK;
using WhittedRaytracer.Raytracing.AccelerationStructures.SBVH;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Utilities;
using Xunit;

namespace UnitTests.Raytracing.SceneObjects {
    public class PrimitiveTest {
        [Fact]
        public void Constructor() {
            Primitive primitive = Utils.Random.Primitive();
        }

        [Fact]
        public void Clip() {
            for (int i = 0; i < 100; i++) {
                Primitive primitive = Utils.Random.Sphere();
                Vector3[] bounds = primitive.Bounds;
                SplitPlane plane = new SplitPlane(Utils.Random.UnitVector(), primitive.Position);
                Fragment fragment = primitive.Clip(plane);
                Vector3[] clipBounds = fragment.Bounds;
                Assert.True(bounds[0] != clipBounds[0] || bounds[1] != clipBounds[1]);
            }
        }
    }
}
