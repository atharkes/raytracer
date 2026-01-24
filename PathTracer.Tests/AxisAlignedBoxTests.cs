using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
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
}
