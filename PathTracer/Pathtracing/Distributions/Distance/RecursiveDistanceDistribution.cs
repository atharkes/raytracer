using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public class RecursiveDistanceDistribution : IDistanceDistribution, IRecursiveCDF<Position1> {
        public IDistanceDistribution Left { get; }
        public IDistanceDistribution Right { get; }

        ICDF<Position1> IRecursiveCDF<Position1>.Left => Left;
        ICDF<Position1> IRecursiveCDF<Position1>.Right => Right;

        public RecursiveDistanceDistribution(IDistanceDistribution left, IDistanceDistribution right) {
            Left = left;
            Right = right;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            var left = Left.GetMaterials(sample);
            var right = Right.GetMaterials(sample);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IMaterial>((left, probabilityLeft), (right, probabilityRight));
            }
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            var left = Left.GetShapeIntervals(sample, material);
            var right = Right.GetShapeIntervals(sample, material);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IShapeInterval>((left, probabilityLeft), (right, probabilityRight));
            }
        }
    }
}
