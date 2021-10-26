using PathTracer.Geometry.Points;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Points {
    /// <summary> An interface for a point in space where a boundary is encountered along a traced <see cref="IRay"/> </summary>
    public interface IShapePoint1 : IPoint1 {
        /// <summary> The <see cref="IShape"/> of the <see cref="IShapePoint1"/> </summary>
        IShape Shape { get; }

        /// <summary> Get an <see cref="IShapePoint1"/> with the normal flipped </summary>
        /// <returns>A <see cref="IShapePoint1"/> with the normal flipped</returns>
        new IShapePoint1 NormalFlipped();

        /// <summary> Get an <see cref="IPoint1"/> with the normal flipped </summary>
        /// <returns>A <see cref="IPoint1"/> with the normal flipped</returns>
        IPoint1 IPoint1.NormalFlipped() => NormalFlipped();
    }
}
