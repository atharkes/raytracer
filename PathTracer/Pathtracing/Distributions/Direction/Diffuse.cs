using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public class Diffuse : IDirectionDistribution {
        public Normal3 Orientation { get; }
        public bool SingleSolution => false;
        public double DomainSize => 2 * Math.PI;

        public Diffuse(Normal3 orientation) {
            Orientation = orientation;
        }

        public bool Contains(Normal3 sample) {
            return IDirection3.Similar(Orientation, sample);
        }

        public double Probability(Normal3 sample) {
            return Contains(sample) ? 1 / DomainSize : 0;
        }

        public Normal3 Sample(Random random) {
            /// Sample on Sphere
            double z = 1 - 2 * random.NextDouble();
            double r = Math.Sqrt(Math.Max(0, 1 - z * z));
            double phi = 2 * Math.PI * random.NextDouble();
            Normal3 direction = new((float)(r * Math.Cos(phi)), (float)(r * Math.Sin(phi)), (float)z);
            /// Orient Direction
            return IDirection3.Similar(Orientation, direction) ? direction : -direction;
        }
    }
}
