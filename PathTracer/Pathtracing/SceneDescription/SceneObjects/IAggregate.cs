using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects {
    /// <summary> A container <see cref="ISceneObject"/> </summary>
    public interface IAggregate : ISceneObject {
        /// <summary> The amount of children of the <see cref="IAggregate"/> </summary>
        int ChildrenCount { get; }
        /// <summary> The children of this <see cref="IAggregate"/> </summary>
        IEnumerable<ISceneObject> Children { get; }

        /// <summary> Add an <paramref name="item"/> to the <see cref="IAggregate"/> </summary>
        /// <param name="item">The <see cref="ISceneObject"/> to add</param>
        void Add(ISceneObject item);

        /// <summary> Add <paramref name="items"/> to the <see cref="IAggregate"/> </summary>
        /// <param name="items">The colleciton of <see cref="ISceneObject"/>s to add</param>
        void AddRange(IEnumerable<ISceneObject> items);

        /// <summary> Remove an <paramref name="item"/> from the <see cref="IAggregate"/> </summary>
        /// <param name="item">The <see cref="ISceneObject"/> to remove</param>
        /// <returns>Whether the <paramref name="item"/> was found and removed from the <see cref="IAggregate"/></returns>
        bool Remove(ISceneObject item);
    }
}
