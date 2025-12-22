using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density;

public class Uniform : IDensityProfile {
    private float Density { get; }

    public Uniform(float density) {
        Density = density;
    }

    public IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction) => new Ray(position, direction);

    public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) => interval.Exit > 0 && interval.Entry < ray.Length
            ? new ExponentialInterval(new Interval(Math.Max(0, interval.Entry), Math.Min(ray.Length, interval.Exit)), Density)
            : null;

    public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) => ray.Travel(distance);
}
