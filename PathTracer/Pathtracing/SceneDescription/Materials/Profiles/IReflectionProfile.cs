﻿using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Reflection;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The scattering profile of an <see cref="IMaterial"/> </summary>
    public interface IReflectionProfile {
        public static readonly IReflectionProfile Diffuse = new Diffuse();
        public static readonly IReflectionProfile Specular = new Specular();
        public static readonly IReflectionProfile Spherical = new Spherical();
        public static IReflectionProfile Blend(params (IReflectionProfile profile, double weight)[] pairs) => new LinearBlend(pairs);

        /// <summary> Get the distribution of outgoing directions at the specified <paramref name="position"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="position"/></param>
        /// <param name="position">The <see cref="Position3"/> at which the scattering occurs</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="position"/></param>
        /// <returns>The distribution containing the outgoing directions</returns>
        IProbabilityDistribution<Normal3> GetDirections(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum);
    }
}
