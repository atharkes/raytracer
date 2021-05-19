namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines that an object can be intersected by a <see cref="Ray"/>. (requirement for path tracing) </summary>
    public interface IIntersectable {
        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="IIntersectable"/></returns>
        bool Intersects(Ray ray);

        /// <summary> Intersect the <see cref="IIntersectable"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="IIntersectable"/> with</param>
        /// <param name="intersectable">The <see cref="IIntersectable"/> that is intersected</param>
        /// <returns>The distance of the intersection with a <see cref="IIntersectable"/> if there is any</returns>
        float? IntersectDistance(Ray ray, out IIntersectable? intersectable);
    }
}
