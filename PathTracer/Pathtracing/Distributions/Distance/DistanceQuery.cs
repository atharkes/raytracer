using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public abstract class DistanceQuery : IDistanceQuery {
        public abstract ISceneObject SceneObject { get; }
        public abstract IBoundaryCollection Boundaries { get; }
        public abstract IDistanceDistribution DistanceDistribution { get; }
        public abstract bool SingleSolution { get; }
        public abstract double DomainSize { get; }

        public abstract bool Contains(ISurfacePoint sample);
        public abstract double CumulativeDistribution(ISurfacePoint sample);
        public abstract double Probability(ISurfacePoint sample);
        public abstract ISurfacePoint Sample(Random random);
    }

    public class SingleDistanceQuery : DistanceQuery { }
    public class ExponentialDistanceQuery : DistanceQuery { }
    public class SumDistanceQuery : DistanceQuery { }
}
