using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Spectra;
using System;
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
        public virtual AxisAlignedBox BoundingBox => Shape.BoundingBox;

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
        public IDistanceMaterialPDF Trace(IRay ray, ISpectrum spectrum) {
            IEnumerable<IBoundaryPoint> boundaries = Shape.Intersect(ray);
            return Material.DistanceMaterialPDF(ray, spectrum, boundaries);
        }

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Primitive"/></returns>
        public bool Intersects(IRay ray) => Shape.Intersects(ray);

        public bool Inside(Vector3 position) => Shape.Inside(position);
        public Vector3 PointOnSurface(Random random) => Shape.PointOnSurface(random);
        public bool OnSurface(Vector3 position, float epsilon = 0.001F) => Shape.OnSurface(position, epsilon);
        public Vector3 SurfaceNormal(Vector3 position) => Shape.SurfaceNormal(position);
        public IEnumerable<float> IntersectDistances(IRay ray) => Shape.IntersectDistances(ray);
        public IEnumerable<IBoundaryPoint> Intersect(IRay ray) => Shape.Intersect(ray);

        IEnumerable<IShape> IDivisible<IShape>.Clip(AxisAlignedPlane plane) => Clip(plane);

        public virtual IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            foreach (IShape shape in Shape.Clip(plane)) {
                if (shape == Shape) {
                    yield return this;
                } else {
                    yield return new PrimitiveFragment(this, shape);
                }
            }
        }

        public IDistancePDF DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints)
            => Material.DistancePDF(ray, spectrum, boundaryPoints);
        public IDistanceMaterialPDF DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) 
            => Material.DistanceMaterialPDF(ray, spectrum, boundaryPoints);
        public IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance)
            => Material.MaterialPDF(ray, spectrum, boundaryPoints, distance);
        public IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint)
            => Material.DirectionPDF(incomingDirection, spectrum, surfacePoint);
        public IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint)
            => Material.DirectionMediumPDF(incomingDirection, spectrum, surfacePoint);
        public IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection)
            => Material.MediumPDF(incomingDirection, spectrum, surfacePoint, outgoingDirection);
        public ISpectrum Absorb(Vector3 direction, ISurfacePoint surfacePoint, ISpectrum spectrum)
            => Material.Absorb(direction, surfacePoint, spectrum);
        public ISpectrum Emit(ISurfacePoint surfacePoint, Vector3 direction)
            => Material.Emit(surfacePoint, direction);
    }
} 
