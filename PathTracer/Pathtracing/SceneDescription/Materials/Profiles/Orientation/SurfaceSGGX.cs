using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class SurfaceSGGX : IOrientationProfile {
        public float Roughness { get; }

        public SurfaceSGGX(float roughness) {
            Roughness = roughness;
        }

        public IProbabilityDistribution<Normal3> GetOrientations(IRay ray, IShape shape, Position3 position) {
            return new SurfaceEllipsoid(shape.OutwardsDirection(position), Roughness, ray.Direction);
        }
    }
}
