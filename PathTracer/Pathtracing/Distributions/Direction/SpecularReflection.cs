using PathTracer.Geometry.Normals;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct SpecularReflection : IDirectionDistribution {
        public Normal3 Orientation { get; }
        public Normal3 IncomingDirection { get; }
        public Normal3 OutgoingDirection { get; }
        public bool ContainsDelta => true;
        public double DomainSize => 0;

        public SpecularReflection(Normal3 orientation, Normal3 incomingDirection) {
            Orientation = orientation;
            IncomingDirection = incomingDirection;
            OutgoingDirection = Normal3.Reflect(-IncomingDirection, Orientation);
        }

        public bool Contains(Normal3 sample) => sample.Equals(OutgoingDirection);

        public double ProbabilityDensity(Normal3 sample) => double.MaxValue;

        public Normal3 Sample(Random random) => OutgoingDirection;
    }
}
