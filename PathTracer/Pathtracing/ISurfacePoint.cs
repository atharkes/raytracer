using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing {
    /// <summary> A scattering event for a <see cref="IRay"/> that follows behaviour according to an <see cref="IMaterial"/> </summary>
    public interface ISurfacePoint : IBoundaryPoint {
        /// <summary> The <see cref="IMaterial"/> that defines the scattering behaviour </summary>
        IMaterial Material { get; }
    }
}
