using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.Points {
    /// <summary> An interface for a point in space where a boundary is encountered along a traced <see cref="IRay"/> </summary>
    public interface IBoundaryPoint : IPoint {
        /// <summary> The distance travelled along a <see cref="IRay"/> to find this <see cref="IBoundaryPoint"/> </summary>
        float Distance { get; }
        /// <summary> Get an <see cref="IBoundaryPoint"/> with a flipped <see cref="Normal"/> </summary>
        IBoundaryPoint FlippedNormal { get; }
    }
}
