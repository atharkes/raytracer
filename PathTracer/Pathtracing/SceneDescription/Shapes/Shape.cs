using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
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
        public virtual bool Inside(Position3 position) {
            IRay ray = new Ray(position, new Normal3(1, 0, 0));
            return Intersect(ray)?.Inside(0) ?? false;
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Shape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Shape"/></returns>
        public abstract bool OnSurface(Position3 position, float epsilon = 0.001F);

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Shape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Shape"/></returns>
        public abstract Position3 SurfacePosition(Random random);

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        public abstract Normal3 SurfaceNormal(Position3 position);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Shape"/></returns>
        public virtual bool Intersects(IRay ray) => IntersectDistances(ray).Any();

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/> with</param>
        /// <returns>The distances of the intersections with a <see cref="Shape"/>, if there are any</returns>
        public abstract IEnumerable<Position1> IntersectDistances(IRay ray);

        /// <summary> Intersect the <see cref="Shape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Shape"/></param>
        /// <returns>The <see cref="IShapePoint1"/> of the intersections with the <see cref="Shape"/>, if there are any</returns>
        public virtual IBoundaryCollection? Intersect(IRay ray) {
            var distances = IntersectDistances(ray);
            if (distances.Any()) {
                SortedSet<IBoundaryInterval> intervals = new SortedSet<IBoundaryInterval>();
                Queue<IShapePoint1> entries = new Queue<IShapePoint1>();
                foreach (Position1 distance in distances.OrderBy(d => d)) {
                    Position3 position = ray.Travel(distance);
                    Normal1 normal = (SurfaceNormal(position) as IDirection3).SimilarAs(ray.Direction) ? Normal1.One : -Normal1.One;
                    IShapePoint1 boundaryPoint = new ShapePoint1(this, distance, normal);
                    if (boundaryPoint.Normal == -Normal1.One) {
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