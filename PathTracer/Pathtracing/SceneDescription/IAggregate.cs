using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription {
    public interface IAggregate {
        IEnumerable<ISceneObject> Children { get; }
        void Add(ISceneObject item);
        bool Remove(ISceneObject item);
    }
}
