using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Utilities;
using System.Collections.Generic;

namespace UnitTests.Pathtracing.SceneObjects {
    [TestClass]
    public class PrimitiveTest {
        [TestMethod]
        public void Constructor() {
            Utils.ThreadRandom.Primitive();
        }

        [TestMethod]
        public void Clip() {
            for (int i = 0; i < 100; i++) {
                IPrimitive primitive = Utils.ThreadRandom.Primitive();
                AxisAlignedBox bounds = primitive.BoundingBox;
                AxisAlignedPlane plane = new(Utils.ThreadRandom.UnitVector(), primitive.Position);
                IEnumerable<ISceneObject> fragments = (primitive as IDivisible<ISceneObject>).Clip(plane);
                foreach (ISceneObject fragment in fragments) {
                    AxisAlignedBox clipBounds = fragment.BoundingBox;
                    Assert.IsTrue(bounds != clipBounds);
                }
            }
        }
    }
}
