using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Services;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Tests.Quality;

public static class QualityScenes {
    private static readonly Lazy<List<ISceneObject>> bunny = new(() => [.. ObjFileLoader.Load(Path.Combine(AppContext.BaseDirectory, "models", "stanford-bunny.obj"), new Position3(0, 0, 0), 1f)]);

    /// <summary> The 69k triangle Stanford bunny, one unit tall </summary>
    public static List<ISceneObject> Bunny => bunny.Value;

    /// <summary> Seeded random triangles, <paramref name="size"/> large, spread over a cube of <paramref name="range"/> </summary>
    public static List<ISceneObject> RandomTriangles(int count, float range = 100f, float size = 2f, int seed = 42) {
        var random = new Random(seed);
        return [.. Enumerable.Range(0, count).Select(_ => new Primitive(random.CreateTriangle(range, size), Material.Diffuse(RGBColors.Gray)))];
    }
}
