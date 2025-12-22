using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription;

/// <summary> An object of the <see cref="Scene"/> </summary>
public interface ISceneObject : IDivisible<ISceneObject> {
    /// <summary> The <see cref="IShape"/> of the <see cref="ISceneObject"/> </summary>
    IShape Shape { get; }

    /// <summary> Trace a <paramref name="ray"/> through the <see cref="ISceneObject"/> </summary>
    /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="ISceneObject"/></param>
    /// <param name="spectrum">The <see cref="ISpectrum"/> of the <see cref="IRay"/></param>
    /// <returns>The <see cref="IDistanceQuery"/> through the <see cref="ISceneObject"/></returns>
    IDistanceQuery? Trace(IRay ray, ISpectrum spectrum);
}

