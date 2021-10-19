using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A scattering event for a <see cref="IRay"/> that follows behaviour according to an <see cref="IMaterial"/> </summary>
    public interface ISurfacePoint : IPoint {
        /// <summary> The <see cref="IMaterial"/> of the <see cref="ISurfacePoint"/> </summary>
        IMaterial Material { get; }
    }
}
