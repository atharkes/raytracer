using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Integrators {
    /// <summary> An abstract <see cref="IIntegrator"/> </summary>
    public abstract class Integrator : IIntegrator {
        /// <summary> Integrate the <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to integrate</param>
        /// <param name="sampleCount">The amount of sample to sample</param>
        public abstract void Integrate(IScene scene, int sampleCount);
    }
}
