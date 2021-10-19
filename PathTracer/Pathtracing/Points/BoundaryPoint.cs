using OpenTK.Mathematics;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A point in space where a boundary is encountered along a traced <see cref="IRay"/> </summary>
    public struct BoundaryPoint : IBoundaryPoint {
        /// <summary> The distance travelled along a <see cref="IRay"/> to find this <see cref="BoundaryPoint"/> </summary>
        public float Distance { get; }
        /// <summary> The position of the <see cref="IBoundaryPoint"/> </summary>
        public Vector3 Position { get; }
        /// <summary> The outward-pointing normal of the boundary at the <see cref="Position"/> </summary>
        public Vector3 Normal { get; }

        /// <summary> Get a new <see cref="BoundaryPoint"/> with a flipped <see cref="Normal"/> </summary>
        public IBoundaryPoint FlippedNormal => new BoundaryPoint(Distance, Position, -Normal);

        /// <summary> Create a new <see cref="BoundaryPoint"/> </summary>
        /// <param name="distance">The distance travelled by the <see cref="IRay"/></param>
        /// <param name="position">The position of the <see cref="BoundaryPoint"/></param>
        /// <param name="normal">The normal of the boundary at the <paramref name="position"/></param>
        public BoundaryPoint(float distance, Vector3 position, Vector3 normal) {
            Distance = distance;
            Position = position;
            Normal = normal;
        }
    }
}
