using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;

/// <summary> Simple container <see cref="ISceneObject"/> holding multiple <see cref="ISceneObject"/>s </summary>
public class Aggregate : IAggregate {
    /// <summary> The amount of children of the <see cref="Aggregate"/> </summary>
    public int ItemCount => Items.Count;
    /// <summary> The <see cref="IShape"/> of the items in this <see cref="Aggregate"/> </summary>
    public IShape Shape => shape ??= new Union(items.Select(i => i.Shape).ToArray());

    /// <summary> The <see cref="ISceneObject"/>s in the <see cref="IAggregate"/> </summary>
    protected IReadOnlyCollection<ISceneObject> Items => items;

    private readonly HashSet<ISceneObject> items = [];
    private Union? shape;

    public Aggregate() { }

    public Aggregate(IEnumerable<ISceneObject> items) {
        AddRange(items);
    }

    public void Add(ISceneObject item) {
        if (items.Add(item)) shape?.Add(item.Shape);
    }

    public void AddRange(IEnumerable<ISceneObject> items) {
        foreach (var item in items) {
            Add(item);
        }
    }

    public bool Remove(ISceneObject item) {
        if (!items.Remove(item)) return false;
        shape = null;
        return true;
    }

    public void Clear() {
        items.Clear();
        shape = null;
    }

    public IEnumerator<ISceneObject> GetEnumerator() => items.GetEnumerator();

    public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
        IDistanceQuery? result = null;
        foreach (var sceneObject in items) {
            result += sceneObject.Trace(ray, spectrum);
        }
        return result;
    }

    public IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) => throw new NotImplementedException("Split items and clip items on the border");
}
