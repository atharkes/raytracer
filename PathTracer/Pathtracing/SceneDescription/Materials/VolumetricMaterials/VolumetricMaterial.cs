using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials {
    public abstract class VolumetricMaterial : Material, IVolumetricMaterial {
        public double Density { get; }

        public VolumetricMaterial(ISpectrum albedo, double density) : base(albedo) {
            Density = density;
        }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position, direction);
        }

        public override ISurfacePoint CreateSurfacePoint(IRay ray, IBoundaryInterval interval, float distance) {
            throw new NotImplementedException("Normal is supposed to be a random vector or a forward vector. Depending on the direction sampling");
            return new SurfacePoint(this, ray.Travel(distance), Vector3.Zero);
        }

        public override IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary) {
            IDistanceDistribution? result = null;
            foreach (IBoundaryInterval interval in boundary.BoundaryIntervals) {
                if (interval.Exit.Distance > 0 && interval.Entry.Distance < ray.Length) {
                    result += new ExponentialDistanceDistribution(Math.Max(0, interval.Entry.Distance), Math.Min(ray.Length, interval.Exit.Distance), Density, this);
                }
            }
            return result;
        }
    }
}
