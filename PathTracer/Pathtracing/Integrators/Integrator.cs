using System;

namespace PathTracer.Pathtracing.Integrators {
    /// <summary> An abstract <see cref="IIntegrator"/> </summary>
    public abstract class Integrator : IIntegrator {
        /// <summary> The amount of samples the <see cref="Integrator"/> evaluated </summary>
        public abstract int SampleCount { get; }

        /// <summary> Integrate the <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to integrate</param>
        /// <param name="integrationTime">The time to spent on integrating the <paramref name="scene"/></param>
        public abstract void Integrate(IScene scene, TimeSpan integrationTime);
    }
}
