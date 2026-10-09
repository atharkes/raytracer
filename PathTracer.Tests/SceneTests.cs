using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Tests;

public class SceneTests {
    private static int Depth(IBinaryTree? node) => node is null || node.Leaf ? 1 : 1 + Math.Max(Depth(node.Left), Depth(node.Right));

    [Test]
    [Timeout(30_000)]
    public async Task Constructor_ShouldSplitBoundedPrimitives_WhenSceneContainsUnboundedPrimitive(CancellationToken cancellationToken) {
        var infinityPlane = new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White));
        List<ISceneObject> primitives = [infinityPlane, .. Enumerable.Range(0, 500).Select(_ => Random.Shared.Primitive(100f, 1f))];

        var sut = new Scene(null!, primitives);

        await Assert.That(sut).Contains(infinityPlane);
        await Assert.That(Depth(sut.OfType<SpatialBVH>().Single())).IsGreaterThan(2);
    }
}
