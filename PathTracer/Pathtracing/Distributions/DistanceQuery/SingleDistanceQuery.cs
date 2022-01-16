using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class SingleDistanceQuery : IDistanceQuery {
        public IDistanceDistribution Distribution { get; }
        public IPrimitive Primitive { get; }

        public SingleDistanceQuery(IDistanceDistribution distribution, IPrimitive primitive) {
            Distribution = distribution;
            Primitive = primitive;
        }
    }
}
