using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary>
    /// A probability distribution of distances.
    /// The probability at a specified distance is the product of out-scattering since the origin, and the material density at the distance.
    /// </summary>
    public interface IDistanceDistribution : ICDF<Position1> {
        /// <summary> The <see cref="IInterval"/> that the <see cref="IDistanceDistribution"/> covers </summary>
        new IInterval Domain { get; }
        /// <summary> The domain of the <see cref="IDistanceDistribution"/> </summary>
        IInterval<Position1> ICDF<Position1>.Domain => Domain;
        /// <summary> The size of the domain of the <see cref="IDistanceDistribution"/> </summary>
        double IProbabilityDistribution.DomainSize => Domain.CoveredArea;

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
            IDistanceDistribution first = left.Domain.Entry < right.Domain.Entry ? left : right;
            IDistanceDistribution last = first == left ? right : left;
            if (first.CumulativeProbability(last.Domain.Entry) >= 1) {
                return first;
            } else {
                return new CombinedDistanceDistribution(first, last);
            }
        }

        /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
        /// <param name="distance">The distance to get the material density at</param>
        /// <returns>The material density at the specified <paramref name="distance"/></returns>
        double MaterialDensity(Position1 distance);

        /// <summary> Convert the <see cref="IDistanceDistribution"/> to a <see cref="string"/> </summary>
        /// <returns>The <see cref="string"/> representing the <see cref="IDistanceDistribution"/></returns>
        string ToString();
    }
}
