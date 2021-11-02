using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines a shape </summary>
    public interface IShape : IIntersectable, IBoundable, IDivisible<IShape> {
        /// <summary> Whether the <see cref="IShape"/> encompasses a volume </summary>
        bool Volumetric { get; }
        /// <summary> The volume of the <see cref="IShape"/> </summary>
        float Volume { get; }
        /// <summary> The surface area of the <see cref="IShape"/> </summary>
        float SurfaceArea { get; }

        /// <summary> Check whether a <paramref name="position"/> is inside the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is inside the <see cref="IShape"/></returns>
        bool Inside(Position3 position);

        /// <summary> Check whether a <paramref name="position"/> is outside the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is outside the <see cref="IShape"/></returns>
        bool Outside(Position3 position) => !Inside(position);

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="IShape"/></returns>
        bool OnSurface(Position3 position, float epsilon = 0.001F);

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="IShape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="IShape"/></returns>
        Position3 SurfacePosition(Random random);

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        Position2 UVPosition(Position3 position);

        /// <summary> Get the surface normal at a specified <paramref name="position"/>, assuming the position is on the surface </summary>
        /// <param name="position">The specified surface position</param>
        /// <returns>The outward-pointing surface normal at the specified <paramref name="position"/></returns>
        Normal3 SurfaceNormal(Position3 position);

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        Normal3 OutwardsDirection(Position3 position);

        /// <summary> Intersect the <see cref="IShape"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IShape"/></param>
        /// <returns>The collection of intersections with the <see cref="IShape"/>, if there are any</returns>
        IBoundaryCollection? IIntersectable.Intersect(IRay ray) {
            IEnumerable<Position1> distances = IntersectDistances(ray);
            if (distances.Any()) {
                SortedSet<IShapeInterval> intervals = new();
                Queue<Position1> entries = new();
                foreach (Position1 distance in distances.OrderBy(d => d)) {
                    Position3 position = IntersectPosition(ray, distance);
                    bool enters = (SurfaceNormal(position) as IDirection3).SimilarAs(ray.Direction);
                    if (enters) {
                        entries.Enqueue(distance);
                    } else {
                        Debug.Assert(entries.Count > 0, "Boundary intersection is invalid (More exits than entries). Warning: Geometry might be degenerate.");
                        intervals.Add(new ShapeInterval(this, entries.Dequeue(), distance));
                    }
                }
                Debug.Assert(entries.Count == 0, "Boundary intersection is invalid (More entries than exits). Warning: Geometry might be degenerate.");
                return new BoundaryCollection(intervals);
            } else {
                return null;
            }
        }

        /// <summary> Clip the <see cref="IShape"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="IShape"/> with</param>
        /// <returns>The clipped <see cref="IShape"/>s</returns>
        IEnumerable<IShape> IDivisible<IShape>.Clip(AxisAlignedPlane plane) {
            foreach (AxisAlignedBox clippedAABB in BoundingBox.Clip(plane)) {
                if (clippedAABB.Equals(BoundingBox)) {
                    yield return this;
                } else {
                    yield return new ClippedShape(this, clippedAABB);
                }
            }
        }
    }
}
