using OpenTK.Mathematics;

namespace PathTracer.Pathtracing {
    /// <summary> A point in space where a scattering event occurs to a <see cref="Ray"/> </summary>
    public interface IScatteringPoint {
        /// <summary> The distance travelled along a <see cref="Ray"/> to find this <see cref="IScatteringPoint"/> </summary>
        public float Distance { get; }
        /// <summary> The world-space position of the <see cref="IScatteringPoint"/> </summary>
        public Vector3 Position { get; }
    }
}
