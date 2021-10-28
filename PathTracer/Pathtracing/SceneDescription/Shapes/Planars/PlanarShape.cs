using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Rays;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Shape"/> that has no volume </summary>
    public abstract class PlanarShape : Shape, IPlanarShape {
        /// <summary> Whether the <see cref="PlanarShape"/> has a volume </summary>
        public override bool Volumetric => false;
        /// <summary> The volume of the <see cref="PlanarShape"/> </summary>
        public override float Volume => 0F;
        /// <summary> The plane in which the <see cref="PlanarShape"/> lies </summary>
        public abstract Plane PlaneOfExistence { get; }

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        public override Normal3 SurfaceNormal(Position3 position) => PlaneOfExistence.Normal;

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        public override Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);

        /// <summary> Intersect the <see cref="PlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="PlanarShape"/> with</param>
        /// <returns>The distances of the intersection with the <see cref="PlanarShape"/>, if there are any</returns>
        public override sealed IEnumerable<Position1> IntersectDistances(IRay ray) {
            Position1? distance = IntersectDistance(ray);
            if (distance.HasValue) {
                yield return distance.Value;
            }
        }

        /// <summary> Intersect the <see cref="PlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="PlanarShape"/> with</param>
        /// <returns>The distance of the intersection with the <see cref="PlanarShape"/>, if there is any</returns>
        public abstract Position1? IntersectDistance(IRay ray);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/>s of the intersections with the <see cref="Shape"/>, if there are any</returns>
        public override sealed IBoundaryCollection? Intersect(IRay ray) {
            Position1? distance = IntersectDistance(ray);
            return distance.HasValue ? new BoundaryCollection(new ShapeInterval(this, distance.Value, distance.Value)) : null;
        }
    }
}
