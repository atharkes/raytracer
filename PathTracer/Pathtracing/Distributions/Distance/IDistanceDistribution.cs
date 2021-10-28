using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary> A 1-dimensional distance distribution </summary>
    public interface IDistanceDistribution : ICDF<Position1> {
        /// <summary> Get the possible <see cref="IMaterial"/>s for a specified <paramref name="distance"/> sample </summary>
        /// <param name="distance">The distance sample</param>
        /// <returns>A <see cref="PMF{T}"/> with the <see cref="IMaterial"/>s</returns>
        WeightedPMF<IMaterial> GetMaterials(Position1 distance);

        /// <summary> Get the possible <see cref="IShapeInterval"/>s for a specified <paramref name="distance"/> and <paramref name="material"/> </summary>
        /// <param name="distance">The distance sample</param>
        /// <param name="material">The <see cref="IMaterial"/></param>
        /// <returns>A <see cref="PMF{T}"/> with the <see cref="IShapeInterval"/>s</returns>
        WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 distance, IMaterial material);

        /// <summary> Combine two <see cref="IDistanceDistribution"/>s </summary>
        /// <param name="left">The left <see cref="IDistanceDistribution"/></param>
        /// <param name="right">The right <see cref="IDistanceDistribution"/></param>
        /// <returns>The combined <see cref="IDistanceDistribution"/></returns>
        public static IDistanceDistribution? operator +(IDistanceDistribution? left, IDistanceDistribution? right) {
            return left is null ? right : (right is null ? left : new SumDistanceDistribution(left, right));
        }
    }
}
