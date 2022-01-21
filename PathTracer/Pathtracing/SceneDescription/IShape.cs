using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
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
        bool Inside(Position3 position) {
            IRay ray = new Ray(position, new Normal3(1, 0, 0));
            return Intersect(ray)?.Includes(0) ?? false;
        }

        /// <summary> Check whether a <paramref name="position"/> is outside the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is outside the <see cref="IShape"/></returns>
        bool Outside(Position3 position) => !Inside(position);

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="IShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="IShape"/></returns>
        bool OnSurface(Position3 position, float epsilon = 0.001F) => DistanceToSurface(position) <= epsilon;

        /// <summary> Get the distance to the surface of the <see cref="IShape"/> from the specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the distance from the surface for</param>
        /// <returns>The distance to the surface of the <see cref="IShape"/> from the specified <paramref name="position"/></returns>
        float DistanceToSurface(Position3 position);

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="IShape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="IShape"/></returns>
        Position3 SurfacePosition(Random random);

        /// <summary> Get a point on the surface of the <see cref="IShape"/> </summary>
        /// <param name="uvPosition">The UV-position to get the point on the surface for</param>
        /// <returns>A point on the surface specified by the <paramref name="uvPosition"/></returns>
        //Position3 SurfacePosition(Position2 uvPosition);

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
        IIntervalCollection? IIntersectable.Intersect(IRay ray) {
            IEnumerable<Position1> distances = IntersectDistances(ray);
            if (distances.Any()) {
                SortedSet<IInterval> intervals = new();
                Queue<Position1> entries = new();
                foreach (Position1 distance in distances.OrderBy(d => d)) {
                    Position3 position = IntersectPosition(ray, distance);
                    bool enters = !IDirection3.InSameOpenHemisphere(SurfaceNormal(position), ray.Direction);
                    if (enters) {
                        entries.Enqueue(distance);
                    } else {
                        if (entries.Count > 0) {
                            intervals.Add(new Interval(entries.Dequeue(), distance));
                        } else {
                            Debug.Write($"Warning: {this} might be degenerate; More exits than entries found.");
                            intervals.Add(new Interval(Position1.NegativeInfinity, distance));
                        }
                    }
                }
                foreach (Position1 entry in entries) {
                    Debug.Write($"Warning: {this} might be degenerate; More entries than exits found.");
                    intervals.Add(new Interval(entry, Position1.PositiveInfinity));
                }
                return new IntervalCollection(intervals.ToArray());
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
