using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using System.Collections.Generic;

namespace PathTracer {
    /// <summary> An interface for a static scene </summary>
    public interface IScene : IAggregate {
        /// <summary> The <see cref="Camera"/> that register light in the <see cref="IScene"/> </summary>
        Camera Camera { get; }
        /// <summary> The lights in the <see cref="IScene"/> </summary>
        IEnumerable<ISceneObject> Lights { get; }
    }
}
