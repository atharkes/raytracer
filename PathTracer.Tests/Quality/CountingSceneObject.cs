using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Tests.Quality;

/// <summary> Counts how often a scene object is traced; fragments clipped from it count towards the same <see cref="TraceCounter"/> </summary>
public sealed class CountingSceneObject(ISceneObject inner, TraceCounter counter) : ISceneObject {
    public CountingSceneObject(ISceneObject inner) : this(inner, new TraceCounter()) { }

    public TraceCounter Counter => counter;
    public IShape Shape => inner.Shape;

    public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
        counter.Count++;
        return inner.Trace(ray, spectrum);
    }

    public IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) => inner.Clip(plane).Select(fragment => new CountingSceneObject(fragment, counter));
}

public sealed class TraceCounter {
    public long Count { get; set; }
}
