using OpenTK;

namespace WhittedRaytracer.Raytracing.AccelerationStructure {
    /// <summary> An interface for a class that has an AABB </summary>
    public interface IAABB {
        /// <summary> The center of the AABB </summary>
        Vector3 AABBCenter { get; }
        /// <summary> The bounds of the AABB </summary>
        (Vector3 Min, Vector3 Max) AABBBounds { get; }
    }
}
