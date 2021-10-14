using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Spectra;

namespace PathTracer.Pathtracing {
    public interface ISample {
        IRay Ray { get; }
        int RecursionDepth { get; }
        ISpectrum Color { get; }
        IMaterial Material { get; }
    }
}
