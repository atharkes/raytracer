using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;
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
                AxisAlignedBox bounds = primitive.Shape.BoundingBox;
                AxisAlignedPlane plane = new(primitive.Shape.BoundingBox.Center, Utils.ThreadRandom.Unit());
                IEnumerable<ISceneObject> fragments = primitive.Clip(plane);
                foreach (PrimitiveFragment fragment in fragments) {
                    AxisAlignedBox clipBounds = fragment.Shape.BoundingBox;
                    Assert.IsTrue(bounds != clipBounds);
                }
            }
        }
    }
}
