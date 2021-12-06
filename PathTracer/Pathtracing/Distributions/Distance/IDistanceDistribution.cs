using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary>
    /// A probability distribution of distances.
    /// The probability at a specified distance is the product of out-scattering since the origin, and the material density at the distance.
    /// </summary>
    public interface IDistanceDistribution : ICDF<Position1> {
        /// <summary> Check whether the <see cref="IDistanceDistribution"/> contains a <paramref name="material"/> </summary>
        /// <param name="material">The <see cref="IMaterial"/> to check</param>
        /// <returns>Whether the <see cref="IDistanceDistribution"/> contains the <paramref name="material"/></returns>
        bool Contains(IMaterial material);

        /// <summary> Check whether the <see cref="IDistanceDistribution"/> contains a <paramref name="interval"/> </summary>
        /// <param name="interval">The <see cref="IShapeInterval"/> to check</param>
        /// <returns>Whether the <see cref="IDistanceDistribution"/> contains the <paramref name="interval"/></returns>
        bool Contains(IShapeInterval interval);

        /// <summary> Get the <see cref="IMaterial"/>s for a specified <paramref name="sample"/> sample </summary>
        /// <param name="sample">The distance sample</param>
        /// <returns>A <see cref="WeightedPMF{T}"/> with the <see cref="IMaterial"/>s</returns>
        WeightedPMF<IMaterial> GetMaterials(Position1 sample);

        /// <summary> Try get the <see cref="IMaterial"/>s for a specified <paramref name="sample"/> sample </summary>
        /// <param name="sample">The distance sample</param>
        /// <returns>A <see cref="WeightedPMF{T}"/> with the <see cref="IMaterial"/>s if there are any</returns>
        WeightedPMF<IMaterial>? TryGetMaterials(Position1 sample) => Contains(sample) ? GetMaterials(sample) : null;

        /// <summary> Get the <see cref="IShapeInterval"/>s for a specified <paramref name="sample"/> and <paramref name="material"/> </summary>
        /// <param name="sample">The distance sample</param>
        /// <param name="material">The <see cref="IMaterial"/></param>
        /// <returns>A <see cref="WeightedPMF{T}"/> with the <see cref="IShapeInterval"/>s</returns>
        WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material);

        /// <summary> Try get the <see cref="IShapeInterval"/>s for a specified <paramref name="sample"/> and <paramref name="material"/> </summary>
        /// <param name="sample">The distance sample</param>
        /// <param name="material">The <see cref="IMaterial"/></param>
        /// <returns>A <see cref="WeightedPMF{T}{T}"/> with the <see cref="IShapeInterval"/>s if there are any</returns>
        WeightedPMF<IShapeInterval>? TryGetShapeIntervals(Position1 sample, IMaterial material) => Contains(sample) && Contains(material) ? GetShapeIntervals(sample, material) : null;

        /// <summary> Combine two <see cref="IDistanceDistribution"/>s </summary>
        /// <param name="left">The left <see cref="IDistanceDistribution"/></param>
        /// <param name="right">The right <see cref="IDistanceDistribution"/></param>
        /// <returns>The combined <see cref="IDistanceDistribution"/></returns>
        public static IDistanceDistribution? operator +(IDistanceDistribution? left, IDistanceDistribution? right) {
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } 
            IDistanceDistribution first = left.Minimum < right.Minimum ? left : right;
            IDistanceDistribution last = first == left ? right : left;
            if (first.CumulativeProbabilityDensity(last.Minimum) >= 1) {
                return first;
            } else {
                return new CombinedDistanceDistribution(first, last);
            }
        }
    }
}
