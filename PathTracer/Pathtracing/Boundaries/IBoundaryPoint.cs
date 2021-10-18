using OpenTK.Mathematics;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.Boundaries {
    /// <summary> An interface for a point in space where a boundary is encountered along a traced <see cref="IRay"/> </summary>
    public interface IBoundaryPoint {
        /// <summary> The distance travelled along a <see cref="IRay"/> to find this <see cref="IBoundaryPoint"/> </summary>
        float Distance { get; }
        /// <summary> The position of the <see cref="IBoundaryPoint"/> </summary>
        Vector3 Position { get; }
        /// <summary> The outward-pointing normal of the boundary at the <see cref="Position"/> </summary>
        Vector3 Normal { get; }

        /// <summary> Get an <see cref="IBoundaryPoint"/> with a flipped <see cref="Normal"/> </summary>
        IBoundaryPoint FlippedNormal { get; }

        /// <summary> Check if a <paramref name="ray"/> enters at the <see cref="IBoundaryPoint"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to check for</param>
        /// <returns>Whether the <paramref name="ray"/> enters at the <see cref="IBoundaryPoint"/></returns>
        bool IsEnteredBy(IRay ray) => Vector3.Dot(ray.Direction, Normal) < 0;

        /// <summary> Check if a <paramref name="ray"/> exits at the <see cref="IBoundaryPoint"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to check for</param>
        /// <returns>Whether the <paramref name="ray"/> exits at the <see cref="IBoundaryPoint"/></returns>
        bool IsExitedBy(IRay ray) => Vector3.Dot(ray.Direction, Normal) > 0;
    }
}
