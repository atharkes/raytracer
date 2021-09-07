using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives {
    /// <summary> A fragment of a <see cref="Shape"/> used for spatial splits in the <see cref="SBVHTree"/> </summary>
    public class PrimitiveFragment : Primitive {
        /// <summary> The original <see cref="IPrimitive"/> contained in the <see cref="PrimitiveFragment"/> </summary>
        public IPrimitive Original { get; }

        /// <summary> Create a new <see cref="PrimitiveFragment"/> </summary>
        /// <param name="original">The original <see cref="Shape"/></param>
        /// <param name="bounds">The bounds of the <see cref="PrimitiveFragment"/></param>
        public PrimitiveFragment(IPrimitive original, IShape clippedShape) : base (clippedShape, original.Material) {
            Original = original;
        }

        /// <summary> Clip the <see cref="PrimitiveFragment"/> by a <paramref name="plane"/></summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="PrimitiveFragment"/> with</param>
        /// <returns>A new <see cref="PrimitiveFragment"/> with clipped bounds</returns>
        public override IEnumerable<PrimitiveFragment> Clip(AxisAlignedPlane plane) {
            foreach (IShape shape in Shape.Clip(plane)) {
                if (shape == Shape) {
                    yield return this;
                } else {
                    yield return new PrimitiveFragment(Original, shape);
                }
            }
        }
    }
}
