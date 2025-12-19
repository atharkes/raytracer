using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density;

public class InverseMultiplicative : IDensityProfile {
    public static uint IntervalLength { get; set; } = 64;

    public IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction) => new Ray(position, direction);

    public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) {
        if (ray.WithinBounds(interval.Entry)) {
            var entry = Position1.Max(0, ((float)interval.Entry).Decrement(IntervalLength));
            var exit = interval.Entry;
            return new UniformInterval(new Interval(entry, exit));
        } else {
            return null;
        }
    }

    public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) => ray.Travel(distance);
}
