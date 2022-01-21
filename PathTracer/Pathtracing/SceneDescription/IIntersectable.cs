using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Rays;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines that an object can be intersected by a <see cref="IRay"/>. (requirement for path tracing) </summary>
    public interface IIntersectable {
        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="IIntersectable"/></returns>
        bool Intersects(IRay ray) => IntersectDistances(ray).Any();

        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <returns>The distances of the intersections with a <see cref="IIntersectable"/>, if there are any</returns>
        IEnumerable<Position1> IntersectDistances(IRay ray);

        /// <summary> Get the position of a <paramref name="ray"/> intersecting the <see cref="IIntersectable"/> at a specified <paramref name="distance"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> that intersects the <see cref="IIntersectable"/></param>
        /// <param name="distance">The distance at which the <paramref name="ray"/> intersects the <see cref="IIntersectable"/></param>
        /// <returns>The intersection position</returns>
        Position3 IntersectPosition(IRay ray, Position1 distance) => ray.Travel(distance);

        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/></param>
        /// <returns>The <see cref="IIntervalCollection"/> if there are any intersections</returns>
        IIntervalCollection? Intersect(IRay ray);
    }
}
