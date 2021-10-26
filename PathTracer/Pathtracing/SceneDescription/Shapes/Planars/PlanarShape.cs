using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
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
            IShapePoint1? shapePoint = PlanarIntersect(ray);
            if (shapePoint is not null) {
                IShapePoint1 entry = shapePoint.Normal == Normal1.One ? shapePoint : shapePoint.NormalFlipped();
                IShapePoint1 exit = shapePoint.Normal == -Normal1.One ? shapePoint : shapePoint.NormalFlipped();
                IBoundaryInterval interval = new BoundaryInterval(entry, exit);
                return new BoundaryCollection(interval);
            } else {
                return null;
            }
        }

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/> of the intersections with the <see cref="Shape"/>, if there is any</returns>
        public virtual IShapePoint1? PlanarIntersect(IRay ray) {
            Position1? distance = IntersectDistance(ray);
            if (distance.HasValue) {
                Position3 position = ray.Travel(distance.Value);
                Normal1 normal = (SurfaceNormal(position) as IDirection3).SimilarAs(ray.Direction) ? Normal1.One : -Normal1.One;
                return new ShapePoint1(this, distance.Value, normal);
            } else {
                return null;
            }
        }
    }
}
