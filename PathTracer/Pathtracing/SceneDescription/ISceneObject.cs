namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An object of the <see cref="Scene"/> </summary>
    public interface ISceneObject : IShape, IDivisible<ISceneObject> {
        /// <summary> Trace a <paramref name="ray"/> through the <see cref="ISceneObject"/> to find the first <see cref="ISurfacePoint"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace</param>
        /// <returns>The first <see cref="ISurfacePoint"/> encountered by the <paramref name="ray"/></returns>
        ISurfacePoint? Trace(IRay ray);
    }
}
