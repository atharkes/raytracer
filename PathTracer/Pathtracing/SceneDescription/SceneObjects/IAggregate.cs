using System.Collections;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects;

/// <summary> A container <see cref="ISceneObject"/> </summary>
public interface IAggregate : ISceneObject, IEnumerable<ISceneObject> {
    /// <summary> The amount of children of the <see cref="IAggregate"/> </summary>
    int ItemCount { get; }

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

    /// <summary> Get the <see cref="IEnumerator"/> of the <see cref="IAggregate"/> </summary>
    /// <returns>The <see cref="IEnumerator"/></returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
