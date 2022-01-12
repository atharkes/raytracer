using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public class ProfiledMaterial : IMaterial {
        public IDensityProfile DensityProfile { get; }
        public IAbsorptionProfile AbsorptionProfile { get; }
        public IOrientationProfile OrientationProfile { get; }
        public IReflectionProfile ScatteringProfile { get; }
        public IEmittanceProfile EmittanceProfile { get; }

        public ISpectrum Albedo => AbsorptionProfile.Albedo;
        public bool IsEmitting => EmittanceProfile.IsEmitting;
        public bool IsSensing => AbsorptionProfile.IsSensing;

        public ProfiledMaterial(IDensityProfile densityProfile,
                                IAbsorptionProfile absorptionProfile,
                                IOrientationProfile orientationProfile,
                                IReflectionProfile scatteringProfile,
                                IEmittanceProfile emittanceProfile) {
            DensityProfile = densityProfile;
            AbsorptionProfile = absorptionProfile;
            OrientationProfile = orientationProfile;
            ScatteringProfile = scatteringProfile;
            EmittanceProfile = emittanceProfile;
        }

        public ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) => EmittanceProfile.GetEmittance(position, orientation, direction);
        public IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) => DensityProfile.GetDistances(ray, spectrum, interval);
        public Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance) => DensityProfile.GetPosition(ray, interval, distance);
        public IRay CreateRay(Position3 position, Normal3 orientation, Normal3 direction) => DensityProfile.CreateRay(position, orientation, direction);
        public IProbabilityDistribution<Normal3> GetOrientationDistribution(IRay ray, IShape shape, Position3 position) => OrientationProfile.GetOrientations(ray, shape, position);
        public IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) => ScatteringProfile.GetDirections(incomingDirection, position, orientation, spectrum);
    }
}
