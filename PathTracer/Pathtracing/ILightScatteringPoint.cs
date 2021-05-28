using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing {
    /// <summary> A scattering event for a <see cref="Ray"/> that follows behaviour according to an <see cref="IMaterial"/> </summary>
    public interface ILightScatteringPoint : IScatteringPoint {
        /// <summary> The <see cref="IMaterial"/> that defines the scattering behaviour </summary>
        public IMaterial Material { get; }
    }
}
