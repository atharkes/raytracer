using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary> A distance query of a <see cref="IRay"/> through an <see cref="ISceneObject"/> </summary>
    public interface IDistanceQuery : ICDF<IMaterialPoint1> {
        /// <summary> The <see cref="ISceneObject"/> of the <see cref="IDistanceQuery"/> </summary>
        ISceneObject SceneObject { get; }
        /// <summary> The <see cref="IShape"/>s boundaries of the <see cref="IDistanceQuery"/> </summary>
        IBoundaryCollection Boundaries { get; }
        /// <summary> The <see cref="IMaterial"/>s distance distribution of the <see cref="IDistanceQuery"/> </summary>
        IDistanceDistribution DistanceDistribution { get; }

        public static IDistanceDistribution? operator +(IDistanceDistribution? left, IDistanceDistribution? right) {
            return left is null ? right : (right is null ? left : new SumDistanceDistribution(left, right));
        }
    }
}
