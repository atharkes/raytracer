using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> Defines the interface of a 2-dimensional shape laying in a plane </summary>
    public interface IPlanarShape : IShape {
        /// <summary> The plane in which the <see cref="IPlanarShape"/> lies </summary>
        public Plane PlaneOfExistence { get; }
    }
}
