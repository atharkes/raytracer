using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An object of the <see cref="Scene"/> </summary>
    public interface ISceneObject : IShape, IDivisible<ISceneObject> {
        /// <summary> Trace a <paramref name="ray"/> through the <see cref="ISceneObject"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="ISceneObject"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <see cref="IRay"/></param>
        /// <returns>The distance and material PDFs</returns>
        IDistanceDistribution? Trace(IRay ray, ISpectrum spectrum);
    }
}

