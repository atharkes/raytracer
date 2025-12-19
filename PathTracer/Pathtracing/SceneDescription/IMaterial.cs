using PathTracer.Pathtracing.SceneDescription.Materials.Profiles;

namespace PathTracer.Pathtracing.SceneDescription;

/// <summary> The material of an <see cref="ISceneObject"/> </summary>
public interface IMaterial {
    /// <summary> The <see cref="IDensityProfile"/> of the <see cref="IMaterial"/> </summary>
    IDensityProfile DensityProfile { get; }
    /// <summary> The <see cref="IAbsorptionProfile"/> of the <see cref="IMaterial"/> </summary>
    IAbsorptionProfile AbsorptionProfile { get; }
    /// <summary> The <see cref="IOrientationProfile"/> of the <see cref="IMaterial"/> </summary>
    IOrientationProfile OrientationProfile { get; }
    /// <summary> The <see cref="IReflectionProfile"/> of the <see cref="IMaterial"/> </summary>
    IReflectionProfile ReflectionProfile { get; }
    /// <summary> The <see cref="IEmittanceProfile"/> of the <see cref="IMaterial"/> </summary>
    IEmittanceProfile EmittanceProfile { get; }
}
