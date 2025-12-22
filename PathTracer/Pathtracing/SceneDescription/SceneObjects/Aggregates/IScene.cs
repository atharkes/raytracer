using PathTracer.Pathtracing.Observers.Cameras;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;

/// <summary> An interface for a static scene </summary>
public interface IScene : IAggregate {
    /// <summary> The <see cref="ICamera"/> that register light in the <see cref="IScene"/> </summary>
    ICamera Camera { get; }
    /// <summary> The lights in the <see cref="IScene"/> </summary>
    IEnumerable<ISceneObject> Lights { get; }
}
