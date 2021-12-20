using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The orientation profile of an <see cref="IMaterial"/> </summary>
    public interface IOrientationProfile {
        /// <summary> Get the orientation distribution for the specified <paramref name="position"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> along which the <paramref name="position"/> is found</param>
        /// <param name="shape">The <see cref="IShape"/> in which the <paramref name="position"/> is found</param>
        /// <param name="position">The position at which to get the orientation distribution</param>
        /// <returns>The distribution containing the orientations</returns>
        IProbabilityDistribution<Normal3> GetOrientations(IRay ray, IShape shape, Position3 position);
    }
}
