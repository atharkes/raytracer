﻿using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    /// <summary> A distance query from a ray through a scene </summary>
    public interface IDistanceQuery {
        /// <summary> The <see cref="IDistanceDistribution"/> of the <see cref="IDistanceQuery"/> </summary>
        IDistanceDistribution DistanceDistribution { get; }

        /// <summary> Check whether the <see cref="IDistanceDistribution"/> contains a <paramref name="primitive"/> </summary>
        /// <param name="primitive">The <see cref="IMaterial"/> to check</param>
        /// <returns>Whether the <see cref="IDistanceDistribution"/> contains the <paramref name="primitive"/></returns>
        bool Contains(IPrimitive primitive);

        /// <summary> Get the <see cref="IMaterial"/>s for a specified <paramref name="sample"/> sample </summary>
        /// <param name="sample">The distance sample</param>
        /// <returns>A <see cref="WeightedPMF{T}"/> with the <see cref="IMaterial"/>s</returns>
        WeightedPMF<IPrimitive> GetPrimitives(Position1 sample);

        /// <summary> Try get the <see cref="IMaterial"/>s for a specified <paramref name="sample"/> sample </summary>
        /// <param name="sample">The distance sample</param>
        /// <returns>A <see cref="WeightedPMF{T}"/> with the <see cref="IMaterial"/>s if there are any</returns>
        WeightedPMF<IPrimitive>? TryGetPrimitives(Position1 sample) => DistanceDistribution.Contains(sample) ? GetPrimitives(sample) : null;

        /// <summary> Combine two <see cref="IDistanceQuery"/>s </summary>
        /// <param name="left">The left <see cref="IDistanceQuery"/></param>
        /// <param name="right">The right <see cref="IDistanceQuery"/></param>
        /// <returns>The combined <see cref="IDistanceQuery"/></returns>
        public static IDistanceQuery? operator +(IDistanceQuery? left, IDistanceQuery? right) {
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            }
            IDistanceQuery first = left.DistanceDistribution.Domain.Entry < right.DistanceDistribution.Domain.Entry ? left : right;
            IDistanceQuery last = first == left ? right : left;
            if (first.DistanceDistribution.CumulativeProbability(last.DistanceDistribution.Domain.Entry) >= 1) {
                return first;
            } else {
                return new CombinedDistanceQuery(first, last);
            }
        }
    }
}
