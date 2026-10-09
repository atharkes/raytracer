using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Tests;

public class AggregateTests {
    private static Primitive Box(float min, float max) => new(new AxisAlignedBox(new Position3(min), new Position3(max)), Material.Diffuse(RGBColors.Gray));

    [Test]
    public async Task BoundingBox_ShouldGrow_WhenItemIsAdded() {
        Aggregate sut = [Box(0, 1)];
        _ = sut.Shape.BoundingBox;

        sut.Add(Box(2, 3));

        await Assert.That(sut.Shape.BoundingBox.MinCorner).IsEqualTo(new Position3(0));
        await Assert.That(sut.Shape.BoundingBox.MaxCorner).IsEqualTo(new Position3(3));
    }

    [Test]
    public async Task BoundingBox_ShouldShrink_WhenItemIsRemoved() {
        var outer = Box(2, 3);
        Aggregate sut = [Box(0, 1), outer];
        _ = sut.Shape.BoundingBox;

        sut.Remove(outer);

        await Assert.That(sut.Shape.BoundingBox.MinCorner).IsEqualTo(new Position3(0));
        await Assert.That(sut.Shape.BoundingBox.MaxCorner).IsEqualTo(new Position3(1));
    }

    [Test]
    public async Task Shape_ShouldBeCached() {
        Aggregate sut = [Box(0, 1), Box(2, 3)];

        await Assert.That(sut.Shape).IsSameReferenceAs(sut.Shape);
    }
}
