using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
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
        public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
            IIntervalCollection? intervals = Shape.Intersect(ray);
            if (intervals is null) {
                return null;
            }
            // Note: Intervals are processed individually here.
            // If different types of behaviours are required
            // (Like only using the first entry and last exit)
            // the conversion from intervals to distance distributions
            // has to be moved to, and specified by the material itself.
            IDistanceDistribution? result = null;
            foreach (IInterval interval in intervals) {
                IDistanceDistribution? distanceDistribution = Material.DensityProfile.GetDistances(ray, spectrum, interval);
                if (distanceDistribution is not null) {
                    result += distanceDistribution;
                }
            }
            return result is not null ? new DistanceQuery(result, this) : null;
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
