using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System.Collections;

namespace PathTracer.Pathtracing.SceneDescription.Shapes;

public class Union : IShape, ICollection<IShape> {
    public AxisAlignedBox BoundingBox => new(Shapes.Select(s => s.BoundingBox).ToArray()); //= new(Position3.PositiveInfinity, Position3.NegativeInfinity);
    public bool Volumetric => Shapes.Any(i => i.Volumetric);
    public float Volume => Shapes.Sum(i => i.Volume);
    public float SurfaceArea => Shapes.Sum(i => i.SurfaceArea);

    /// <summary> The <see cref="ISceneObject"/>s in the <see cref="IAggregate"/> </summary>
    protected ICollection<IShape> Shapes { get; set; } = new HashSet<IShape>();

    public int Count => Shapes.Count;
    public bool IsReadOnly => Shapes.IsReadOnly;

    public Union(params IShape[] shapes) {
        Shapes = new HashSet<IShape>(shapes);
    }

    public void Add(IShape item) => Shapes.Add(item);//Position3 minCorner = Position3.ComponentMax(BoundingBox.MinCorner, item.BoundingBox.MinCorner);//Position3 maxCorner = Position3.ComponentMin(BoundingBox.MaxCorner, item.BoundingBox.MaxCorner);//BoundingBox = new(minCorner, maxCorner);

    public void Clear() => Shapes.Clear();//BoundingBox = new AxisAlignedBox();

    public bool Contains(IShape item) => Shapes.Contains(item);

    public void CopyTo(IShape[] array, int arrayIndex) => Shapes.CopyTo(array, arrayIndex);

    public bool Remove(IShape item) {
        if (Shapes.Contains(item)) {
            Shapes.Remove(item);
            //BoundingBox = new(Shapes.Select(s => s.BoundingBox).ToArray());
            return true;
        } else {
            return false;
        }
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
