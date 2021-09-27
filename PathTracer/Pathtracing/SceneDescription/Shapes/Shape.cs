using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> An abstract <see cref="Shape"/> for displaying objects in the <see cref="Scene"/> </summary>
    public abstract class Shape : IShape {
        /// <summary> Whether the <see cref="Shape"/> has a volume </summary>
        public abstract bool Volumetric { get; }
        /// <summary> The volume of the <see cref="Shape"/> </summary>
        public abstract float Volume { get; }
        /// <summary> The surface area of the <see cref="Shape"/> </summary>
        public abstract float SurfaceArea { get; }
        /// <summary> The bounding box of the <see cref="Shape"/> </summary>
        public abstract AxisAlignedBox BoundingBox { get; }

        /// <summary> Check whether a <paramref name="position"/> is inside the <see cref="Shape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is inside the <see cref="Shape"/></returns>
        public virtual bool Inside(Vector3 position) {
            IRay ray = new Ray(position, Vector3.UnitX);
            return Intersect(ray)?.Inside(0) ?? false;
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Shape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Shape"/></returns>
        public abstract bool OnSurface(Vector3 position, float epsilon = 0.001F);

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Shape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Shape"/></returns>
        public abstract Vector3 PointOnSurface(Random random);

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        public abstract Vector3 SurfaceNormal(Vector3 position);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Shape"/></returns>
        public virtual bool Intersects(IRay ray) => IntersectDistances(ray).Any();

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/> with</param>
        /// <returns>The distances of the intersections with a <see cref="Shape"/>, if there are any</returns>
        public abstract IEnumerable<float> IntersectDistances(IRay ray);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/> of the intersections with the <see cref="Shape"/>, if there are any</returns>
        public virtual IBoundaryCollection? Intersect(IRay ray) {
            var distances = IntersectDistances(ray);
            if (distances.Any()) {
                SortedSet<IBoundaryInterval> intervals = new SortedSet<IBoundaryInterval>();
                Queue<IBoundaryPoint> entries = new Queue<IBoundaryPoint>();
                foreach (float distance in distances.OrderBy(d => d)) {
                    Vector3 position = ray.Travel(distance);
                    Vector3 normal = SurfaceNormal(position);
                    IBoundaryPoint boundaryPoint = new BoundaryPoint(distance, position, normal);
                    if (boundaryPoint.IsEntered(ray)) {
                        Debug.Assert(entries.Count > 0, "Boundary intersection is invalid (More exits than entries). Warning: Geometry might be degenerate.");
                        entries.Enqueue(boundaryPoint);
                    } else {
                        intervals.Add(new BoundaryInterval(entries.Dequeue(), boundaryPoint));
                    }
                }
                Debug.Assert(entries.Count == 0, "Boundary intersection is invalid (More entries than exits). Warning: Geometry might be degenerate.");
                return new BoundaryCollection(intervals);
            } else {
                return null;
            }
        }

        /// <summary> Clip the <see cref="Shape"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="Shape"/> with</param>
        /// <returns>The clipped <see cref="Shape"/>s</returns>
        public virtual IEnumerable<IShape> Clip(AxisAlignedPlane plane) {
            foreach (AxisAlignedBox clippedAABB in BoundingBox.Clip(plane)) {
                if (clippedAABB == BoundingBox) {
                    yield return this;
                } else {
                    yield return new ClippedShape(this, clippedAABB);
                }
            }
        }
    }
}