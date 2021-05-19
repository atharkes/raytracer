namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An object of the <see cref="Scene"/> </summary>
    public interface ISceneObject : IShape {
        float? IntersectDistance(Ray ray, out IPrimitive? primitive);
    }
}
