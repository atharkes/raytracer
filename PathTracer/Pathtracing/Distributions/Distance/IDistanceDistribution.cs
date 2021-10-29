using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary> A 1-dimensional distance distribution </summary>
    public interface IDistanceDistribution : ICDF<Position1> {
        /// <summary> Get the possible <see cref="IMaterial"/>s for a specified <paramref name="sample"/> sample </summary>
        /// <param name="sample">The distance sample</param>
        /// <returns>A <see cref="PMF{T}"/> with the <see cref="IMaterial"/>s</returns>
        WeightedPMF<IMaterial>? GetMaterials(Position1 sample);

        /// <summary> Get the possible <see cref="IShapeInterval"/>s for a specified <paramref name="sample"/> and <paramref name="material"/> </summary>
        /// <param name="sample">The distance sample</param>
        /// <param name="material">The <see cref="IMaterial"/></param>
        /// <returns>A <see cref="PMF{T}"/> with the <see cref="IShapeInterval"/>s</returns>
        WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material);

        /// <summary> Combine two <see cref="IDistanceDistribution"/>s </summary>
        /// <param name="left">The left <see cref="IDistanceDistribution"/></param>
        /// <param name="right">The right <see cref="IDistanceDistribution"/></param>
        /// <returns>The combined <see cref="IDistanceDistribution"/></returns>
        public static IDistanceDistribution? operator +(IDistanceDistribution? left, IDistanceDistribution? right) {
            return left is null ? right : (right is null ? left : new RecursiveDistanceDistribution(left, right));
        }
    }
}
