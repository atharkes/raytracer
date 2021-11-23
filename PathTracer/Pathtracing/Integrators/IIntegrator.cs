using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;

namespace PathTracer.Pathtracing.Integrators {
    /// <summary> An integrator to render a <see cref="IScene"/> </summary>
    public interface IIntegrator {
        /// <summary> Integrate the <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to integrate</param>
        /// <param name="sampleCount">The amount of samples to sample</param>
        void Integrate(IScene scene, int sampleCount);
    }
}
