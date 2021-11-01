using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives {
    /// <summary> A simple primitive object for the scene </summary>
    public class Primitive : IPrimitive {
        /// <summary> The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/> </summary>
        public IShape Shape { get; }
        /// <summary> The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/> </summary>
        public IMaterial Material { get; }

        /// <summary> Whether the <see cref="Primitive"/> encompasses a volume </summary>
        public bool Volumetric => Shape.Volumetric;
        /// <summary> The volume of the <see cref="Primitive"/> </summary>
        public float Volume => Shape.Volume;
        /// <summary> The surface area of the <see cref="Primitive"/> </summary>
        public float SurfaceArea => Shape.SurfaceArea;
        /// <summary> The bounding box of the <see cref="Primitive"/> </summary>
        public AxisAlignedBox BoundingBox => Shape.BoundingBox;

        /// <summary> Create a new <see cref="Primitive"/> with a <paramref name="shape"/> and <paramref name="material"/> </summary>
        /// <param name="shape">The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/></param>
        /// <param name="material">The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/></param>
        public Primitive(IShape shape, IMaterial material) {
            Shape = shape;
            Material = material;
        }

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Primitive"/> with</param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <returns>The distance and material pdfs</returns>
        public IDistanceDistribution? Trace(IRay ray, ISpectrum spectrum) {
            IBoundaryCollection? boundary = Shape.Intersect(ray);
            if (boundary is null) {
                return null;
            }
            return Material.DistanceDistribution(ray, spectrum, boundary);
        }

        public virtual IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            foreach (IShape shape in Shape.Clip(plane)) {
                if (shape == Shape) {
                    yield return this;
                } else {
                    yield return new PrimitiveFragment(this, shape);
                }
            }
        }
    }
}
