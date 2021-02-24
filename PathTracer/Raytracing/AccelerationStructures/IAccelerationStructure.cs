namespace PathTracer.Raytracing.AccelerationStructures {
    /// <summary> An interface for an acceleration structure </summary>
    public interface IAccelerationStructure {
        /// <summary> Intersect the acceleration structure with a ray </summary>
        /// <param name="ray">The ray to intersect the acceleration structure with</param>
        /// <returns>An intersection with a primitive if there is any</returns>
        Intersection? Intersect(Ray ray);

        /// <summary> Intersect the acceleration structure with a ray </summary>
        /// <param name="ray">The ray to intersect the acceleration structure with</param>
        /// <returns>Whether there is an intersection with a primitive</returns>
        bool IntersectBool(Ray ray);
    }
}
