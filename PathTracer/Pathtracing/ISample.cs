using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing {
    public interface ISample {
        IRay Ray { get; }
        int RecursionDepth { get; }
        Vector3 Color { get; }
        IMaterial Material { get; }
    }
}
