using OpenTK.Mathematics;

namespace PathTracer.Pathtracing {
    /// <summary> A point in space where a scattering event occurs to a <see cref="Ray"/> </summary>
    public interface IBoundaryPoint {
        /// <summary> The distance travelled along a <see cref="Ray"/> to find this <see cref="IBoundaryPoint"/> </summary>
        public float Distance { get; }
        /// <summary> The world-space position of the <see cref="IBoundaryPoint"/> </summary>
        public Vector3 Position { get; }
        /// <summary> The outward pointing normal of the boundary at the <see cref="Position"/> </summary>
        public Vector3 Normal { get; }
    }
}
