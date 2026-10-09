using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Services;

namespace PathTracer.Benchmarks;

public static class Bunny {
    /// <summary> The 69k triangle Stanford bunny, one unit tall </summary>
    public static List<ISceneObject> Load() => [.. ObjFileLoader.Load(Path.Combine(AppContext.BaseDirectory, "models", "stanford-bunny.obj"), new Position3(0, 0, 0), 1f)];

    /// <summary> Seeded rays from a sphere around the <paramref name="bounds"/> towards random points inside them </summary>
    public static Ray[] Rays(AxisAlignedBox bounds, int count, int seed = 7) {
        var random = new Random(seed);
        var center = bounds.Center;
        var radius = (float)(bounds.MaxCorner - bounds.MinCorner).Length * 2;
        return [.. Enumerable.Range(0, count).Select(_ => {
            var origin = center + new Normal3(random.NextSingle() * 2 - 1, random.NextSingle() * 2 - 1, random.NextSingle() * 2 - 1) * radius;
            var target = new Position3(
                bounds.MinCorner.X + (bounds.MaxCorner.X - bounds.MinCorner.X) * random.NextSingle(),
                bounds.MinCorner.Y + (bounds.MaxCorner.Y - bounds.MinCorner.Y) * random.NextSingle(),
                bounds.MinCorner.Z + (bounds.MaxCorner.Z - bounds.MinCorner.Z) * random.NextSingle());
            return new Ray(origin, new Normal3((target - origin).Vector));
        })];
    }
}
