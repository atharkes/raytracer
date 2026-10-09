using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using System.Diagnostics;

namespace PathTracer.Tests.Quality;

/// <summary> Quality of a bounding volume hierarchy, independent of the cost constants the hierarchy is built with </summary>
/// <param name="Primitives">The amount of primitives the hierarchy was built from</param>
/// <param name="References">The amount of items in all leaves; above <paramref name="Primitives"/> when spatial splits duplicate them</param>
/// <param name="Nodes">The amount of nodes, leaves included</param>
/// <param name="MaxDepth">The depth of the deepest leaf</param>
/// <param name="AverageLeafSize">The average amount of items in a leaf</param>
/// <param name="MaxLeafSize">The largest amount of items in a leaf</param>
/// <param name="SahCost">The surface area heuristic cost of the whole tree, with traversal and intersection weighted equally</param>
/// <param name="PrimitiveTestsPerRay">The average amount of primitives traced per ray</param>
/// <param name="BoxTestsPerRay">The average amount of node bounding boxes tested per ray</param>
/// <param name="BuildMilliseconds">The time to build the hierarchy</param>
public sealed record HierarchyMetrics(int Primitives, int References, int Nodes, int MaxDepth, double AverageLeafSize, int MaxLeafSize, double SahCost, double PrimitiveTestsPerRay, double BoxTestsPerRay, long BuildMilliseconds) {
    public override string ToString()
        => $"primitives {Primitives}, references {References}, nodes {Nodes}, depth {MaxDepth}, leaf avg {AverageLeafSize:F1} max {MaxLeafSize}, "
         + $"SAH {SahCost:F1}, tests/ray {PrimitiveTestsPerRay:F1}, boxes/ray {BoxTestsPerRay:F1}, build {BuildMilliseconds} ms";

    public static HierarchyMetrics Measure(IReadOnlyList<ISceneObject> primitives, Func<List<ISceneObject>, BoundingVolumeHierarchy> build, int rayCount = 2_000) {
        var counter = new TraceCounter();
        List<ISceneObject> counted = [.. primitives.Select(primitive => new CountingSceneObject(primitive, counter))];
        var timer = Stopwatch.StartNew();
        var root = build(counted);
        var buildMilliseconds = timer.ElapsedMilliseconds;

        var leaves = new List<(BoundingVolumeHierarchy Node, int Depth)>();
        var nodes = 0;
        var rootArea = (double)root.Shape.BoundingBox.SurfaceArea;
        var sah = 0d;
        void Visit(BoundingVolumeHierarchy node, int depth) {
            nodes++;
            var relativeArea = node.Shape.BoundingBox.SurfaceArea / rootArea;
            if (node.Leaf) {
                leaves.Add((node, depth));
                sah += node.ItemCount * relativeArea;
            } else {
                sah += relativeArea;
                Visit(node.Left!, depth + 1);
                Visit(node.Right!, depth + 1);
            }
        }
        Visit(root, 1);

        var boxTests = 0L;
        foreach (var ray in Rays(root.Shape.BoundingBox, rayCount)) {
            var cameraRay = new CameraRay(ray.Origin, ray.Direction, null!);
            root.Trace(cameraRay, RGBSpectrum.White);
            boxTests += cameraRay.BVHTraversals;
        }

        return new HierarchyMetrics(
            Primitives: primitives.Count,
            References: leaves.Sum(leaf => leaf.Node.ItemCount),
            Nodes: nodes,
            MaxDepth: leaves.Max(leaf => leaf.Depth),
            AverageLeafSize: leaves.Average(leaf => leaf.Node.ItemCount),
            MaxLeafSize: leaves.Max(leaf => leaf.Node.ItemCount),
            SahCost: sah,
            PrimitiveTestsPerRay: (double)counter.Count / rayCount,
            BoxTestsPerRay: (double)boxTests / rayCount,
            BuildMilliseconds: buildMilliseconds);
    }

    /// <summary> Seeded rays from a sphere around the <paramref name="bounds"/> towards random points inside them </summary>
    public static IEnumerable<Ray> Rays(AxisAlignedBox bounds, int count, int seed = 7) {
        var random = new Random(seed);
        var center = bounds.Center;
        var radius = (float)(bounds.MaxCorner - bounds.MinCorner).Length * 2;
        for (var i = 0; i < count; i++) {
            var origin = center + new Normal3(random.NextSingle() * 2 - 1, random.NextSingle() * 2 - 1, random.NextSingle() * 2 - 1) * radius;
            var target = new Position3(
                bounds.MinCorner.X + (bounds.MaxCorner.X - bounds.MinCorner.X) * random.NextSingle(),
                bounds.MinCorner.Y + (bounds.MaxCorner.Y - bounds.MinCorner.Y) * random.NextSingle(),
                bounds.MinCorner.Z + (bounds.MaxCorner.Z - bounds.MinCorner.Z) * random.NextSingle());
            yield return new Ray(origin, new Normal3((target - origin).Vector));
        }
    }
}
