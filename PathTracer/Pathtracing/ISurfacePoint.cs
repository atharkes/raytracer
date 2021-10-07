using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;

namespace PathTracer.Pathtracing {
    /// <summary> A scattering event for a <see cref="IRay"/> that follows behaviour according to an <see cref="IMaterial"/> </summary>
    public interface ISurfacePoint : IBoundaryPoint {
        /// <summary> The <see cref="IPrimitive"/> on which the <see cref="ISurfacePoint"/> lies </summary>
        IPrimitive Primitive { get; }
    }
}
