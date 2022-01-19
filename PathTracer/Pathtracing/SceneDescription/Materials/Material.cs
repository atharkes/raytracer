using PathTracer.Pathtracing.SceneDescription.Materials.Profiles;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public class Material : IMaterial {
        public IDensityProfile DensityProfile { get; }
        public IAbsorptionProfile AbsorptionProfile { get; }
        public IOrientationProfile OrientationProfile { get; }
        public IReflectionProfile ReflectionProfile { get; }
        public IEmittanceProfile EmittanceProfile { get; }

        public Material(IDensityProfile densityProfile,
                                IAbsorptionProfile absorptionProfile,
                                IOrientationProfile orientationProfile,
                                IReflectionProfile reflectionProfile,
                                IEmittanceProfile emittanceProfile) {
            DensityProfile = densityProfile;
            AbsorptionProfile = absorptionProfile;
            OrientationProfile = orientationProfile;
            ReflectionProfile = reflectionProfile;
            EmittanceProfile = emittanceProfile;
        }

        public static Material Emitter(RGBSpectrum spectrum) {
            return new(IDensityProfile.Surface,
                       IAbsorptionProfile.BlackBody,
                       IOrientationProfile.Flat,
                       IReflectionProfile.Diffuse,
                       IEmittanceProfile.Uniform(spectrum));
        }

        public static Material Diffuse(RGBSpectrum albedo) {
            return new(IDensityProfile.Surface,
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Flat,
                       IReflectionProfile.Diffuse,
                       IEmittanceProfile.None);
        }

        public static Material Specular(RGBSpectrum albedo) {
            return new(IDensityProfile.Surface,
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Flat,
                       IReflectionProfile.Specular,
                       IEmittanceProfile.None);
        }

        public static Material SpecularDiffuseBlend(RGBSpectrum albedo, float specularity) {
            return new(IDensityProfile.Surface,
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Flat,
                       IReflectionProfile.Blend((IReflectionProfile.Diffuse, 1f - specularity), (IReflectionProfile.Specular, specularity)),
                       IEmittanceProfile.None);
        }

        public static Material Glossy(RGBSpectrum albedo, float roughness) {
            return new(IDensityProfile.Surface,
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.SurfaceGGX(roughness),
                       IReflectionProfile.Specular,
                       IEmittanceProfile.None);
        }

        public static Material IsotropicVolumetric(RGBSpectrum albedo, float density) {
            return new(IDensityProfile.Volumetric(density),
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Forwards,
                       IReflectionProfile.Spherical,
                       IEmittanceProfile.None);
        }

        public static Material DiffuseParticleCloud(RGBSpectrum albedo, float density) {
            return new(IDensityProfile.Volumetric(density),
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Uniform,
                       IReflectionProfile.Diffuse,
                       IEmittanceProfile.None);
        }

        public static Material SpecularParticleCloud(RGBSpectrum albedo, float density) {
            return new(IDensityProfile.Volumetric(density),
                       IAbsorptionProfile.Uniform(albedo),
                       IOrientationProfile.Uniform,
                       IReflectionProfile.Specular,
                       IEmittanceProfile.None);
        }
    }
}
