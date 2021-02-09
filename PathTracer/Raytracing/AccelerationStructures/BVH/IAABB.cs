using OpenTK.Mathematics;

namespace PathTracer.Raytracing.AccelerationStructures.BVH {
    /// <summary> An interface for a class that supports an AABB </summary>
    public interface IAABB {
        /// <summary> The center of the AABB </summary>
        Vector3 Center { get; }
        /// <summary> The bounds of the AABB </summary>
        Vector3[] Bounds { get; }
    }
}
