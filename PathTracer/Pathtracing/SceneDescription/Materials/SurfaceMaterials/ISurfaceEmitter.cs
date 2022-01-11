using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary> An <see cref="IMaterial"/> that emitts light at the surface </summary>
    public interface ISurfaceEmitter : IEmitter, ISurfaceMaterial {
        /// <summary> A <see cref="ISurfaceEmitter"/> absorbs all incoming light </summary>
        ISpectrum IMaterial.Albedo => RGBSpectrum.Black;

        /// <summary> The emission <see cref="ISpectrum"/> of the <see cref="ISurfaceEmitter"/> </summary>
        /// <param name="position">The position to get the emission at</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/> at the <paramref name="position"/></param>
        /// <param name="direction">The direction of the emission</param>
        /// <returns>The emission at the <paramref name="position"/> in the specified <paramref name="direction"/></returns>
        ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) {
            return Normal3.Similar(orientation, direction) ? Color * Strength : ISpectrum.Black;
        }

        IProbabilityDistribution<Normal3> IMaterial.DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            throw new NotImplementedException();
        }
    }
}
