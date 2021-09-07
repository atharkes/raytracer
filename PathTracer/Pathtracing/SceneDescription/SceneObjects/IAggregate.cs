using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects {
    public interface IAggregate : ISceneObject {
        IEnumerable<ISceneObject> Children { get; }
        void Add(ISceneObject item);
        void AddRange(IEnumerable<ISceneObject> items);
        bool Remove(ISceneObject item);
    }
}
