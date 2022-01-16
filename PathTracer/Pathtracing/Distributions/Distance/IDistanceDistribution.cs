﻿using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary>
    /// A probability distribution of distances.
    /// The probability at a specified distance is the product of out-scattering since the origin, and the material density at the distance.
    /// </summary>
    public interface IDistanceDistribution : IInterval, ICDF<Position1> {
        /// <summary> The <see cref="IInterval"/> that the <see cref="IDistanceDistribution"/> covers </summary>
        IInterval Interval { get; }
        /// <summary> The entry/start point of the <see cref="IDistanceDistribution"/> </summary>
        Position1 IInterval<Position1>.Entry => Interval.Entry;
        /// <summary> The stop/exit point of the <see cref="IDistanceDistribution"/> </summary>
        Position1 IInterval<Position1>.Exit => Interval.Exit;

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
