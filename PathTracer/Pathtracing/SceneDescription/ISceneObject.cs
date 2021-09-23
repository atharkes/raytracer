using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An object of the <see cref="Scene"/> </summary>
    public interface ISceneObject : IShape, IMaterial, IDivisible<ISceneObject> {
        /// <summary> Trace a <paramref name="ray"/> through the <see cref="ISceneObject"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="ISceneObject"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <see cref="IRay"/></param>
        /// <returns>The distance and material PDFs</returns>
        IDistanceMaterialPDF Trace(IRay ray, ISpectrum spectrum);
    }
}

