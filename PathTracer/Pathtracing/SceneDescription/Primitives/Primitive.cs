using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.Shapes;

namespace PathTracer.Pathtracing.SceneDescription.Primitives {
    /// <summary> A simple primitive object for the scene </summary>
    public class Primitive : IPrimitive {
        /// <summary> The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/> </summary>
        public IShape Shape { get; }
        /// <summary> The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/> </summary>
        public IMaterial Material { get; }

        /// <summary> Create a new <see cref="Primitive"/> with a <paramref name="shape"/> and <paramref name="material"/> </summary>
        /// <param name="shape">The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/></param>
        /// <param name="material">The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/></param>
        public Primitive(IShape shape, IMaterial material) {
            Shape = shape;
            Material = material;
        }

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Primitive"/></returns>
        public bool Intersects(Ray ray) => Shape.Intersects(ray);

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>An intersection with a primitive if there is any</returns>
        public (float Distance, Primitive Primitive)? Intersection(Ray ray) {
            float? intersectionDistance = Shape.Intersect(ray);
            if (intersectionDistance.HasValue) {
                return (intersectionDistance.Value, this);
            } else {
                return null;
            }
        }
    }
}
