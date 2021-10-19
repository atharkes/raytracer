using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary> A distance query of a <see cref="IRay"/> through an <see cref="ISceneObject"/> </summary>
    public interface IDistanceQuery : ICDF<ISurfacePoint> {
        /// <summary> The <see cref="ISceneObject"/> of the <see cref="IDistanceQuery"/> </summary>
        ISceneObject SceneObject { get; }
        /// <summary> The <see cref="IShape"/>s boundaries of the <see cref="IDistanceQuery"/> </summary>
        IBoundaryCollection Boundaries { get; }
        /// <summary> The <see cref="IMaterial"/>s distance distribution of the <see cref="IDistanceQuery"/> </summary>
        IDistanceDistribution DistanceDistribution { get; }
    }
}
