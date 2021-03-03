using OpenTK.Mathematics;

namespace PathTracer.Pathtracing {
    /// <summary> An object of a <see cref="Scene"/> </summary>
    public interface ISceneObject {
        /// <summary> The position of the <see cref="ISceneObject"/> </summary>
        Vector3 Position { get; set; }
    }
}
