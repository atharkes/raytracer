using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Tests;

public class ShapeIntersectTests {
    /// <summary> Walls perpendicular to the X axis at the given positions, facing -X where the ray enters and +X where it exits </summary>
    private sealed class Walls(float[] entering, float[] exiting) : IShape {
        public bool Volumetric => true;
        public float Volume => 0;
        public float SurfaceArea => 0;
        public AxisAlignedBox BoundingBox => new(new Position3(-100), new Position3(100));

        public IEnumerable<Position1> IntersectDistances(IRay ray) => [.. entering.Concat(exiting).Order().Select(x => (Position1)x)];
        public Normal3 SurfaceNormal(Position3 position) => entering.Contains(MathF.Round(position.X)) ? -Normal3.UnitX : Normal3.UnitX;
        public Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);
        public float DistanceToSurface(Position3 position) => throw new NotSupportedException();
        public Position3 SurfacePosition(Random random) => throw new NotSupportedException();
        public Position2 UVPosition(Position3 position) => throw new NotSupportedException();
    }

    private static readonly Ray AlongX = new(new Position3(0, 0, 0), Normal3.UnitX);

    private static async Task AssertIntervals(IShape shape, params (float Entry, float Exit)[] expected) {
        var intervals = shape.Intersect(AlongX).Select(i => ((float)i.Entry, (float)i.Exit)).ToList();
        await Assert.That(intervals).IsEquivalentTo(expected, TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Intersect_ShouldPairEachExitWithTheEarliestOpenEntry() {
        await AssertIntervals(new Walls([1, 2, 3, 4, 5], [6, 7, 8, 9, 10]), (1, 6), (2, 7), (3, 8), (4, 9), (5, 10));
    }

    [Test]
    public async Task Intersect_ShouldGiveAnIntervalPerPassedSlab() {
        await AssertIntervals(new Walls([1, 3, 5, 7, 9], [2, 4, 6, 8, 10]), (1, 2), (3, 4), (5, 6), (7, 8), (9, 10));
    }

    [Test]
    public async Task Intersect_ShouldOpenIntervalsToInfinity_WhenEntriesOrExitsAreUnmatched() {
        await AssertIntervals(new Walls([3], [1]), (float.NegativeInfinity, 1), (3, float.PositiveInfinity));
    }

    [Test]
    public async Task Intersect_ShouldGiveNothing_WhenTheRayMissesEverything() {
        await AssertIntervals(new Walls([], []));
    }
}
