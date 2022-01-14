﻿using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The orientation profile of an <see cref="IMaterial"/> </summary>
    public interface IOrientationProfile {
        /// <summary> Get an orientation distribution from the <see cref="IOrientationProfile"/> </summary>
        /// <param name="position">The position at which to get the orientations</param>
        /// <param name="direction">The incoming direction at the <paramref name="position"/></param>
        /// <param name="shape">The <see cref="IShape"/> of the <see cref="IMaterial"/></param>
        /// <returns>A distribution containing the orientations</returns>
        IProbabilityDistribution<Normal3> GetOrientations(Position3 position, Normal3 direction, IShape shape);
    }
}
