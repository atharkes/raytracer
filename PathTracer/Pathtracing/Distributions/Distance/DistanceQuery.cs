using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using System;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class DistanceQuery : IDistanceQuery {
        public IRay Ray { get; }
        public ISceneObject SceneObject { get; }
        public IBoundaryCollection Boundaries { get; }
        public IDistanceDistribution DistanceDistribution { get; }

        public bool SingleSolution => throw new NotImplementedException();
        public double DomainSize => throw new NotImplementedException();

        public DistanceQuery(IRay ray, ISceneObject sceneObject, IBoundaryCollection boundaries, IDistanceDistribution distanceDistribution) {
            Ray = ray;
            SceneObject = sceneObject;
            Boundaries = boundaries;
            DistanceDistribution = distanceDistribution;
        }

        public bool Contains(ISurfacePoint sample) {
            throw new NotImplementedException();
        }

        public double CumulativeDistribution(ISurfacePoint sample) {
            throw new NotImplementedException();
        }

        public double Probability(ISurfacePoint sample) {
            throw new NotImplementedException("Consists of the probability of the distance sample, and the normal sample");
        }

        public ISurfacePoint Sample(Random random) {
            (double distance, IMaterial material) = (DistanceDistribution as IPDF<IDistanceMaterial>).Sample(random);
            IBoundaryInterval interval = Boundaries.First(b => b.Includes(distance));
            return material.CreateSurfacePoint(Ray, interval, (float)distance);
        }
    }

    public class SumDistanceQuery : DistanceQuery { }
}
