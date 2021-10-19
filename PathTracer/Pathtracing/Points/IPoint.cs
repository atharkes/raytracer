using OpenTK.Mathematics;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A point in space with a normal specifying the outwards hemisphere of the point </summary>
    public interface IPoint {
        /// <summary> The position of the <see cref="IBoundaryPoint"/> </summary>
        Vector3 Position { get; }
        /// <summary> The outward-pointing normal of the boundary at the <see cref="Position"/> </summary>
        Vector3 Normal { get; }

        /// <summary> Check whether a <paramref name="direction"/> goes into the <see cref="IPoint"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes into the surface</returns>
        bool IsTowards(Vector3 direction) {
            return Vector3.Dot(direction, Normal) < 0;
        }

        /// <summary> Check whether a <paramref name="direction"/> goes away from the <see cref="IPoint"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes away from the surface</returns>
        bool IsFrom(Vector3 direction) {
            return Vector3.Dot(direction, Normal) > 0;
        }
    }
}
