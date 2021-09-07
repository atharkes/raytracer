using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface that defines that an object can be bounded. (useful for acceleration structures) </summary>
    public interface IBoundable {
        /// <summary> The bounding box of the <see cref="IBoundable"/> </summary>
        AxisAlignedBox BoundingBox { get; }
    }
}
