using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects {
    /// <summary> A simple <see cref="ISceneObject"/> containing a single <see cref="IShape"/> and <see cref="IMaterial"/> </summary>
    public interface IPrimitive : ISceneObject, IMaterial {
        /// <summary> The bounding <see cref="IShape"/> of the <see cref="IPrimitive"/> </summary>
        IShape Shape { get; }
        /// <summary> The <see cref="IMaterial"/> of the <see cref="IPrimitive"/> </summary>
        IMaterial Material { get; }

        #region IShape
        bool IIntersectable.Intersects(IRay ray) => Shape.Intersects(ray);
        IEnumerable<Position1> IIntersectable.IntersectDistances(IRay ray) => Shape.IntersectDistances(ray);
        Position3 IIntersectable.IntersectPosition(IRay ray, Position1 distance) => Shape.IntersectPosition(ray, distance);
        IBoundaryCollection? IIntersectable.Intersect(IRay ray) => Shape.Intersect(ray);

        bool IShape.Inside(Position3 position) => Shape.Inside(position);
        Position3 IShape.SurfacePosition(Random random) => Shape.SurfacePosition(random);
        Position2 IShape.UVPosition(Position3 position) => Shape.UVPosition(position);
        bool IShape.OnSurface(Position3 position, float epsilon = 0.001F) => Shape.OnSurface(position, epsilon);
        Normal3 IShape.SurfaceNormal(Position3 position) => Shape.SurfaceNormal(position);
        Normal3 IShape.OutwardsDirection(Position3 position) => Shape.OutwardsDirection(position);
        #endregion

        #region IMaterial
        IDistanceDistribution? IMaterial.DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) => Material.DistanceDistribution(ray, spectrum, interval);
        Position3 IMaterial.GetPosition(IRay ray, IShapeInterval interval, Position1 distance) => Material.GetPosition(ray, interval, distance);
        IPDF<Normal3> IMaterial.GetOrientationDistribution(IRay ray, IShape shape, Position3 position) => Material.GetOrientationDistribution(ray, shape, position);
        
        IDirectionDistribution? IMaterial.DirectionDistribution(Normal3 incomingDirection, ISpectrum spectrum, Position3 position) => Material.DirectionDistribution(incomingDirection, spectrum, position);
        IRay IMaterial.CreateRay(Position3 position, Normal3 normal, Normal3 direction) => Material.CreateRay(position, normal, direction);
        #endregion
    }
}
