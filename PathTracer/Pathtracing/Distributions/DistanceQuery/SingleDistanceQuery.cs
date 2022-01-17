using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public class SingleDistanceQuery : IDistanceQuery {
        public IDistanceDistribution Distribution { get; }
        public IPrimitive Primitive { get; }

        public IInterval Interval => Distribution.Interval;
        public Position1 Minimum => Distribution.Minimum;
        public Position1 Maximum => Distribution.Maximum;
        public bool ContainsDelta => Distribution.ContainsDelta;
        public double DomainSize => Distribution.DomainSize;

        public ICollection<IInterval> Intervals => Distribution.Interval;

        public Position1 Entry => throw new NotImplementedException();

        public Position1 Exit => throw new NotImplementedException();

        public SingleDistanceQuery(IDistanceDistribution distribution, IPrimitive primitive) {
            Distribution = distribution;
            Primitive = primitive;
        }

        public bool Contains(IPrimitive primitive) {
            throw new NotImplementedException();
        }

        public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) {
            throw new NotImplementedException();
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            throw new NotImplementedException();
        }

        public double ProbabilityDensity(Position1 sample) {
            throw new NotImplementedException();
        }

        public Position1 Sample(Random random) {
            throw new NotImplementedException();
        }

        public bool Contains(Position1 sample) {
            throw new NotImplementedException();
        }

        public bool Equals(IProbabilityDistribution<Position1>? other) {
            throw new NotImplementedException();
        }
    }
}
