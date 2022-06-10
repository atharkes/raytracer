using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Integrators.DistanceSamplers {
    public class ImmediateDistanceSampler : IDistanceDistribution, IDistanceQuery {

        public IRay Ray { get; }
        public ISceneObject Scene { get; }

        public IInterval Domain => throw new NotImplementedException();

        public IDistanceDistribution DistanceDistribution => throw new NotImplementedException();

        readonly PriorityQueue<(IInterval, ISceneObject), IInterval> toQuery;
        IDistanceQuery? queriedDistribution = null;

        public ImmediateDistanceSampler(IRay ray, ISceneObject scene) {
            Ray = ray;
            Scene = scene;
            toQuery = new PriorityQueue<(IInterval, ISceneObject), IInterval>();
            IInterval? interval = scene.Shape.BoundingBox.Intersect(ray);
            if (interval is not null) {
                toQuery.Enqueue((interval, scene), interval);
            }
        }

        public Position1 Sample(Random random) {
            Position1 sample = Position1.PositiveInfinity;
            if (queriedDistribution is not null) {
                sample = queriedDistribution.DistanceDistribution.Sample(random);
            }
            var toEvaluate = new PriorityQueue<(IInterval, ISceneObject), IInterval>();
            while (toQuery.Count > 0 && toQuery.Peek().Item1.Entry < sample) {
                (IInterval interval, ISceneObject sceneObject) = toQuery.Dequeue();
                toEvaluate.Enqueue((interval, sceneObject), interval);
            }
            while (toEvaluate.Count > 0) {
                (IInterval interval, ISceneObject sceneObject) = toEvaluate.Dequeue();
                if (sceneObject is IPrimitive primitive) {
                    // Create distribution, sample, and add to queriedDistribution
                } else if (sceneObject is IAggregate aggregate) {
                    IEnumerable<ISceneObject> children = aggregate.GetChildren(Ray);
                    foreach (ISceneObject child in children) {
                        // Intersect child and discard or add to toQuery/toEvaluate
                    }
                }
            }
            return sample;
        }


        public double MaterialDensity(Position1 distance) {
            throw new NotImplementedException();
        }

        public double CumulativeProbability(Position1 sample) {
            throw new NotImplementedException();
        }

        public double ProbabilityDensity(Position1 sample) {
            throw new NotImplementedException();
        }

        public bool Equals(IProbabilityDistribution<Position1>? other) {
            throw new NotImplementedException();
        }

        public bool Contains(IPrimitive primitive) {
            throw new NotImplementedException();
        }

        public WeightedPMF<IPrimitive> GetPrimitives(Position1 sample) {
            throw new NotImplementedException();
        }
    }
}
