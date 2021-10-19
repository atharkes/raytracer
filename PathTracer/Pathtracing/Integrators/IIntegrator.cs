using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using System;

namespace PathTracer.Pathtracing.Integrators {
    /// <summary> An integrator to render a <see cref="IScene"/> </summary>
    public interface IIntegrator {
        /// <summary> The amount of samples the <see cref="IIntegrator"/> evaluated </summary>
        int SampleCount { get; }

        /// <summary> Integrate the <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to integrate</param>
        /// <param name="integrationTime">The time to spent on integrating the <paramref name="scene"/></param>
        void Integrate(IScene scene, TimeSpan integrationTime);
    }
}
