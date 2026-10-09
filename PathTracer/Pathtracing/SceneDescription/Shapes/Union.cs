using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System.Collections;

namespace PathTracer.Pathtracing.SceneDescription.Shapes;

public class Union : IShape, ICollection<IShape> {
    public AxisAlignedBox BoundingBox => boundingBox ??= new(Shapes.Select(s => s.BoundingBox).ToArray());
    public bool Volumetric => Shapes.Any(i => i.Volumetric);
    public float Volume => Shapes.Sum(i => i.Volume);
    public float SurfaceArea => Shapes.Sum(i => i.SurfaceArea);

    /// <summary> The <see cref="ISceneObject"/>s in the <see cref="IAggregate"/> </summary>
    protected HashSet<IShape> Shapes { get; }

    private AxisAlignedBox? boundingBox;

    public int Count => Shapes.Count;
    public bool IsReadOnly => false;

    public Union(params IShape[] shapes) {
        Shapes = new HashSet<IShape>(shapes);
    }

    public void Add(IShape item) {
        if (Shapes.Add(item) && boundingBox is { } box) {
            boundingBox = AxisAlignedBox.Encompass(box, item.BoundingBox);
        }
    }

    public void Clear() {
        Shapes.Clear();
        boundingBox = null;
    }

    public bool Contains(IShape item) => Shapes.Contains(item);

    public void CopyTo(IShape[] array, int arrayIndex) => Shapes.CopyTo(array, arrayIndex);

    public bool Remove(IShape item) {
        if (!Shapes.Remove(item)) return false;
        boundingBox = null;
        return true;
    }

    public IEnumerator<IShape> GetEnumerator() => Shapes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Shapes.GetEnumerator();

    public IEnumerable<Position1> IntersectDistances(IRay ray) {
        foreach (var shape in Shapes) {
            foreach (var distance in shape.IntersectDistances(ray)) {
                yield return distance;
            }
        }
    }

    public float DistanceToSurface(Position3 position) => Shapes.Min(t => t.DistanceToSurface(position));
    public bool OnSurface(Position3 position, float epsilon = 0.001F) => Shapes.Any(s => s.OnSurface(position, epsilon));

    public Normal3 OutwardsDirection(Position3 position) => throw new NotImplementedException("Requires to find the shape with the surface closest to the position");

    public Normal3 SurfaceNormal(Position3 position) => throw new NotImplementedException("Requires to find the shape with the surface closest to the position");

    public Position3 SurfacePosition(Random random) => throw new NotImplementedException("Sample based on surface area?");

    public Position2 UVPosition(Position3 position) => throw new NotImplementedException("Requires combining of UV-maps");
}
