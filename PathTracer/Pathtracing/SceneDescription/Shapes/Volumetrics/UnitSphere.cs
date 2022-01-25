using PathTracer.Geometry.Positions;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> An <see cref="ISphere"/> shape with radius 1 </summary>
    public struct UnitSphere : ISphere, IEquatable<UnitSphere> {
        /// <summary> Position of the <see cref="UnitSphere"/> </summary>
        public Position3 Position { get; }
        /// <summary> The radius of the <see cref="UnitSphere"/> </summary>
        public float Radius => 1f;

        /// <summary> Create a new <see cref="Sphere"/> </summary>
        /// <param name="position">The position of the <see cref="Sphere"/></param>
        /// <param name="radius">The radius of the <see cref="Sphere"/></param>
        /// <param name="material">The material of the <see cref="Sphere"/></param>
        public UnitSphere(Position3 position) {
            Position = position;
        }

        public static bool operator ==(UnitSphere left, UnitSphere right) => left.Equals(right);
        public static bool operator !=(UnitSphere left, UnitSphere right) => !(left == right);

        public override int GetHashCode() => Position.GetHashCode();
        public override bool Equals(object? obj) => obj is UnitSphere unitSphere && Equals(unitSphere);
        public bool Equals(UnitSphere unitSphere) => Position.Equals(unitSphere.Position);
    }
}
