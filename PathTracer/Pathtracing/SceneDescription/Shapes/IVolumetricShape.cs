namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> Defines the interface of a <see cref="IShape"/> that encompasses a volume. </summary>
    public interface IVolumetricShape {
        /// <summary> The volume of the <see cref="IVolumetricShape"/> </summary>
        public float Volume { get; }
    }
}
