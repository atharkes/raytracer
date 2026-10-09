using PathTracer.Geometry.Positions;
using PathTracer.Services;

namespace PathTracer.Tests;

public class ObjFileLoaderTests {
    private static readonly string BunnyPath = Path.Combine(AppContext.BaseDirectory, "models", "stanford-bunny.obj");

    [Test]
    public async Task Load_ShouldStandModelOnPosition_WithRequestedHeight() {
        var sut = ObjFileLoader.Load(BunnyPath, new Position3(1, 0, 2), 1.5f);

        var box = sut.Shape.BoundingBox;
        await Assert.That((float)box.MinCorner.Y).IsEqualTo(0f).Within(1e-4f);
        await Assert.That((float)box.MaxCorner.Y).IsEqualTo(1.5f).Within(1e-4f);
        await Assert.That((float)(box.MinCorner.X + box.MaxCorner.X) / 2).IsEqualTo(1f).Within(1e-4f);
        await Assert.That((float)(box.MinCorner.Z + box.MaxCorner.Z) / 2).IsEqualTo(2f).Within(1e-4f);
    }
}
