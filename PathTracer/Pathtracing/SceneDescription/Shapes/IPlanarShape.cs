using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> A planar <see cref="IShape"/> </summary>
    public interface IPlanarShape : IShape {
        /// <summary> Whether the <see cref="IPlanarShape"/> has a volume </summary>
        bool IShape.Volumetric => false;
        /// <summary> The volume of the <see cref="IPlanarShape"/> </summary>
        float IShape.Volume => 0F;
        /// <summary> The plane in which the <see cref="IPlanarShape"/> lies </summary>
        Plane PlaneOfExistence { get; }

        /// <summary> Check whether a <paramref name="position"/> is inside the <see cref="IPlanarShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is inside the <see cref="IPlanarShape"/></returns>
        bool IShape.Inside(Position3 position) => false;

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        Normal3 IShape.SurfaceNormal(Position3 position) => PlaneOfExistence.Normal;

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        Normal3 IShape.OutwardsDirection(Position3 position) => SurfaceNormal(position);

        /// <summary> Intersect the <see cref="IPlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IPlanarShape"/> with</param>
        /// <returns>The distances of the intersection with the <see cref="IPlanarShape"/>, if there are any</returns>
        IEnumerable<Position1> IIntersectable.IntersectDistances(IRay ray) {
            Position1? distance = IntersectDistance(ray);
            if (distance.HasValue) {
                yield return distance.Value;
            }
        }

        /// <summary> Intersect the <see cref="IPlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IPlanarShape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/>s of the intersections with the <see cref="IPlanarShape"/>, if there are any</returns>
        IIntervalCollection? IIntersectable.Intersect(IRay ray) {
            Position1? distance = IntersectDistance(ray);
            return distance.HasValue ? new IntervalCollection(new Interval(distance.Value, distance.Value)) : null;
        }

        /// <summary> Intersect the <see cref="IPlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IPlanarShape"/> with</param>
        /// <returns>The distance of the intersection with the <see cref="IPlanarShape"/>, if there is any</returns>
        Position1? IntersectDistance(IRay ray);
    }
}
