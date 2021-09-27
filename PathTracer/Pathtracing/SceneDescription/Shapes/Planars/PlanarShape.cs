using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Shape"/> that has no volume </summary>
    public abstract class PlanarShape : Shape, IPlanarShape {
        /// <summary> Whether the <see cref="PlanarShape"/> has a volume </summary>
        public override bool Volumetric => false;
        /// <summary> The volume of the <see cref="PlanarShape"/> </summary>
        public override float Volume => 0F;

        /// <summary> Intersect the <see cref="PlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="PlanarShape"/> with</param>
        /// <returns>The distances of the intersection with the <see cref="PlanarShape"/>, if there are any</returns>
        public override sealed IEnumerable<float> IntersectDistances(IRay ray) {
            float? distance = IntersectDistance(ray);
            if (distance.HasValue) {
                yield return distance.Value;
            }
        }

        /// <summary> Intersect the <see cref="PlanarShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="PlanarShape"/> with</param>
        /// <returns>The distance of the intersection with the <see cref="PlanarShape"/>, if there is any</returns>
        public abstract float? IntersectDistance(IRay ray);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/>s of the intersections with the <see cref="Shape"/>, if there are any</returns>
        public override sealed IBoundaryCollection? Intersect(IRay ray) {
            IBoundaryPoint? boundaryPoint = PlanarIntersect(ray);
            if (boundaryPoint is not null) {
                IBoundaryPoint entry = boundaryPoint.IsEntered(ray) ? boundaryPoint : boundaryPoint.FlippedNormal;
                IBoundaryPoint exit = entry == boundaryPoint ? boundaryPoint.FlippedNormal : boundaryPoint;
                IBoundaryInterval interval = new BoundaryInterval(entry, exit);
                throw new NotImplementedException("Copy boundary point but flip normal. Requires IBoundaryPoint and Collection implementation");
            } else {
                return null;
            }
        }

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/> of the intersections with the <see cref="Shape"/>, if there is any</returns>
        public virtual IBoundaryPoint? PlanarIntersect(IRay ray) {
            float? distance = IntersectDistance(ray);
            if (distance.HasValue) {
                Vector3 position = ray.Travel(distance.Value);
                Vector3 normal = SurfaceNormal(position);
                return new BoundaryPoint(distance.Value, position, normal);
            } else {
                return null;
            }
        }
    }
}
