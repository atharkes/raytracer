using OpenTK.Mathematics;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines that an object can be bounded. (useful for acceleration structures) </summary>
    public interface IBoundable {
        /// <summary> The center of the object </summary>
        Vector3 Center { get; }
        /// <summary> The bounds of the object </summary>
        Vector3[] Bounds { get; }
    }
}
