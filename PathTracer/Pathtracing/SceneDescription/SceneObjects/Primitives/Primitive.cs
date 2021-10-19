﻿using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
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
        public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
            IBoundaryCollection? boundary = Shape.Intersect(ray);
            if (boundary is null) {
                return null;
            }
            IDistanceDistribution? distanceDistribution = Material.DistanceDistribution(ray, spectrum, boundary);
            if (distanceDistribution is null) {
                return null;
            }
            return new DistanceQuery(ray, this, boundary, distanceDistribution);
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
        public IBoundaryCollection? Intersect(IRay ray) => Shape.Intersect(ray);

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

        public IDistanceDistribution? DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary)
            => Material.DistanceMaterialPDF(ray, spectrum, boundary);
        public IPDF<Vector3, IMedium>? DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint)
            => Material.DirectionDistribution(incomingDirection, spectrum, surfacePoint);
    }
}
