using OpenTK.Mathematics;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Plane"/> of which the normal is a unitvector </summary>
    public class AxisAlignedPlane : Plane {
        /// <summary> Create a new <see cref="AxisAlignedPlane"/> using a <paramref name="normal"/> and a <paramref name="distance"/> </summary>
        /// <param name="normal">The normal of the <see cref="AxisAlignedPlane"/></param>
        /// <param name="distance">The distance of the <see cref="AxisAlignedPlane"/> from the origin along the <paramref name="normal"/></param>
        private AxisAlignedPlane(Vector3 normal, float distance) : base(normal, distance) { }

        public static AxisAlignedPlane X(bool positive, float xPosition) => new(positive ? Vector3.UnitX : -Vector3.UnitX, xPosition);
        public static AxisAlignedPlane Y(bool positive, float yPosition) => new(positive ? Vector3.UnitY : -Vector3.UnitY, yPosition);
        public static AxisAlignedPlane Z(bool positive, float zPosition) => new(positive ? Vector3.UnitZ : -Vector3.UnitZ, zPosition);
    }
}
