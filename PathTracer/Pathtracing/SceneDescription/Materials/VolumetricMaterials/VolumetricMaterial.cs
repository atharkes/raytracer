using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials {
    public abstract class VolumetricMaterial : Material, IVolumetricMaterial {
        public double Density { get; }

        public VolumetricMaterial(ISpectrum albedo, double density) : base(albedo) {
            Density = density;
        }

        public override IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            if (interval.Exit > 0 && interval.Entry < ray.Length) {
                return new ExponentialDistanceDistribution(Math.Max(0, interval.Entry), Math.Min(ray.Length, interval.Exit), Density, this);
            } else {
                return null;
            }
        }

        public override Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance) {
            return ray.Travel(distance);
        }

        public override IRay CreateRay(Position3 position, Normal3 normal, Normal3 direction) {
            return new Ray(position, direction);
        }
    }
}
