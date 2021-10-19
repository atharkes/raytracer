﻿using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Rays;
using PathTracer.Spectra;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="ISurfacePoint"/> on creation.
    /// </summary>
    public abstract class SurfaceMaterial : Material, IMaterial {
        /// <summary>
        /// Epsilon used to raise the surface point away from the primitive.
        /// Used to avoid the intersection falling behind the primitive by rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.001f;

        /// <summary> Create a new <see cref="SurfaceMaterial"/> using a specified <paramref name="albedo"/> </summary>
        /// <param name="albedo">The albedo <see cref="ISpectrum"/> of the new <see cref="SurfaceMaterial"/></param>
        public SurfaceMaterial(ISpectrum albedo) : base(albedo) { }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position + surfacePoint.Normal * RaiseEpsilon, direction);
        }

        public override IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary) {
            float entry = boundary.BoundaryIntervals.First(i => i.Entry.Distance > 0).Entry.Distance;
            return new SingleDistanceDistribution(new DistanceMaterial(entry, this));
        }
    }
}
