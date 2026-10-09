using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Tests;

public class AxisAlignedBoxTests {
    public static object[][] Intersections() => [
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(-2, 0, 0), Normal3.UnitX)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(2, 0, 0), -Normal3.UnitX)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, -2, 0), Normal3.UnitY)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, 2, 0), -Normal3.UnitY)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, 0, -2), Normal3.UnitZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, 0, 2), -Normal3.UnitZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(-2, -2, 0), Normal3.XY)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(2, 2, 0), -Normal3.XY)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(-2, 0, -2), Normal3.XZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(2, 0, 2), -Normal3.XZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, -2, -2), Normal3.YZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(0, 2, 2), -Normal3.YZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(-2, -2, -2), Normal3.XYZ)],
        [new AxisAlignedBox(new Position3(-1), new Position3(1)), new Ray(new Position3(2, 2, 2), -Normal3.XYZ)],
    ];

    [Test]
    [MethodDataSource(nameof(Intersections))]
    public async Task IntersectDistances_ShouldBeInAscendingOrder(AxisAlignedBox sut, Ray ray) {
        var result = sut.IntersectDistances(ray);

        await Assert.That(result).IsNotEmpty().And.IsInOrder();
    }

    public static object[][] Clips() => [
        [AxisAlignedPlane.X(true, .5f), new Position3(.5f, 0, 0), new Position3(1, 1, 1)],
        [AxisAlignedPlane.X(false, .5f), new Position3(0, 0, 0), new Position3(.5f, 1, 1)],
        [AxisAlignedPlane.Y(true, .5f), new Position3(0, .5f, 0), new Position3(1, 1, 1)],
        [AxisAlignedPlane.Y(false, .5f), new Position3(0, 0, 0), new Position3(1, .5f, 1)],
        [AxisAlignedPlane.Z(true, .5f), new Position3(0, 0, .5f), new Position3(1, 1, 1)],
        [AxisAlignedPlane.Z(false, .5f), new Position3(0, 0, 0), new Position3(1, 1, .5f)],
    ];

    [Test]
    [MethodDataSource(nameof(Clips))]
    public async Task Clip_ShouldKeepThePartOnThePositiveSideOfThePlane(AxisAlignedPlane plane, Position3 expectedMin, Position3 expectedMax) {
        var sut = new AxisAlignedBox(new Position3(0), new Position3(1));

        var result = sut.Clip(plane).Single();

        await Assert.That(result.MinCorner).IsEqualTo(expectedMin);
        await Assert.That(result.MaxCorner).IsEqualTo(expectedMax);
    }

    [Test]
    public async Task Clip_ShouldKeepFlatBox() {
        var sut = new AxisAlignedBox(new Position3(0, 0, 0), new Position3(1, 0, 1));

        var result = sut.Clip(AxisAlignedPlane.X(false, .5f)).ToList();

        await Assert.That(result).HasSingleItem();
    }
}
