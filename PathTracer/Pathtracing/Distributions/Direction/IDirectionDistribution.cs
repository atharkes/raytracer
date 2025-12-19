using PathTracer.Geometry.Normals;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.Distributions.Direction;

/// <summary> A 3-dimensional direction distribution </summary>
public interface IDirectionDistribution : IPDF<Normal3> {
    /// <summary> The orientation (normal) of the <see cref="IDirectionDistribution"/> </summary>
    Normal3 Orientation { get; }

    /// <summary> Get the possible <see cref="IMedium"/>s for a specified <paramref name="sample"/> sample </summary>
    /// <param name="sample">The direction sample</param>
    /// <returns>A <see cref="PMF{T}"/> with the <see cref="IMedium"/>s</returns>
    //WeightedPMF<IMedium>? GetMedia(Normal3 sample);

    /// <summary> Combine two <see cref="IDirectionDistribution"/>s </summary>
    /// <param name="left">The left <see cref="IDirectionDistribution"/></param>
    /// <param name="right">The right <see cref="IDirectionDistribution"/></param>
    /// <returns>The combined <see cref="IDirectionDistribution"/></returns>
    static IDirectionDistribution? operator +(IDirectionDistribution? left, IDirectionDistribution? right) => left is null ? right : (right is null ? left : throw new NotImplementedException());
}
