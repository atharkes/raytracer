using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines that an object can be intersected by a <see cref="IRay"/>. (requirement for path tracing) </summary>
    public interface IIntersectable {
        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="IIntersectable"/></returns>
        bool Intersects(IRay ray);

        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <returns>The distances of the intersections with a <see cref="IIntersectable"/>, if there are any</returns>
        IEnumerable<float> IntersectDistances(IRay ray);

        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="IIntersectable"/></param>
        /// <returns>The <see cref="IBoundaryPoint"/> of the intersections with the <see cref="IIntersectable"/>, if there are any</returns>
        IBoundary? Intersect(IRay ray);
    }
}
