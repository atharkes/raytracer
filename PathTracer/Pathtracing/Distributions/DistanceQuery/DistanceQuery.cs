using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class DistanceQuery : IDistanceQuery {
        public IDistanceDistribution DistanceDistribution { get; }
        public IPrimitive Primitive { get; }

        public DistanceQuery(IDistanceDistribution distribution, IPrimitive primitive) {
            DistanceDistribution = distribution;
            Primitive = primitive;
        }

        public bool Contains(IPrimitive primitive) => Primitive.Equals(primitive);

        public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) => new((Primitive, DistanceDistribution.ProbabilityDensity(sample)));
    }
}
