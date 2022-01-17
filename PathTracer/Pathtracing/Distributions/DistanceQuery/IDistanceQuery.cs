using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;

namespace PathTracer.Pathtracing.Distributions.DistanceQuery {
    public interface IDistanceQuery : IDistanceDistribution, IIntervalCollection {
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
        WeightedPMF<IPrimitive>? TryGetPrimitives(Position1 sample) => Contains(sample) ? GetPrimitives(sample) : null;
    }
}
