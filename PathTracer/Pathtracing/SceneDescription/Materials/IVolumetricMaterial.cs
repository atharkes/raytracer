namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> A volumetric <see cref="IMaterial"/> like smoke or clouds. </summary>
    public interface IVolumetricMaterial : IMaterial {
        /// <summary> The density of the (reactive molecules in the) <see cref="IVolumetricMaterial"/> </summary>
        double Density { get; }
    }
}
