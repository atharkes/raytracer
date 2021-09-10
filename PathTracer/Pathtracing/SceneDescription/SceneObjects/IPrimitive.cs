namespace PathTracer.Pathtracing.SceneDescription.SceneObjects {
    /// <summary> A simple <see cref="ISceneObject"/> containing a single <see cref="IShape"/> and <see cref="IMaterial"/> </summary>
    public interface IPrimitive : ISceneObject {
        /// <summary> The bounding <see cref="IShape"/> of the <see cref="IPrimitive"/> </summary>
        IShape Shape { get; }
        /// <summary> The <see cref="IMaterial"/> of the <see cref="IPrimitive"/> </summary>
        IMaterial Material { get; }
    }
}
