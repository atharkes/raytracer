namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> An <see cref="IMedium"/> that emits light along the path of an <see cref="IRay"/> </summary>
    public interface IEmissiveMedium : IMedium, IEmitter { }
}
