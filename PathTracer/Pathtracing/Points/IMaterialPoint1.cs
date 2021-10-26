using PathTracer.Geometry.Points;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A scattering event for a <see cref="IRay"/> that follows behaviour according to an <see cref="IMaterial"/> </summary>
    public interface IMaterialPoint1 : IPoint1 {
        /// <summary> The <see cref="IMaterial"/> of the <see cref="IMaterialPoint1"/> </summary>
        IMaterial Material { get; }
    }
}
