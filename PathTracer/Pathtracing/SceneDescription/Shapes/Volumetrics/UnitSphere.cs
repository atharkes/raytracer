using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> An <see cref="ISphere"/> shape with radius 1 </summary>
    public struct UnitSphere : ISphere {
        /// <summary> Position of the <see cref="UnitSphere"/> </summary>
        public Position3 Position { get; }
        /// <summary> The radius of the <see cref="UnitSphere"/> </summary>
        public Vector1 Radius => 1;

        /// <summary> Create a new <see cref="Sphere"/> </summary>
        /// <param name="position">The position of the <see cref="Sphere"/></param>
        /// <param name="radius">The radius of the <see cref="Sphere"/></param>
        /// <param name="material">The material of the <see cref="Sphere"/></param>
        public UnitSphere(Position3 position) {
            Position = position;
        }
    }
}
