using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Plane"/> of which the normal is a unitvector </summary>
    public struct AxisAlignedPlane : IPlanarShape {
        /// <summary> The plane of existence of the <see cref="AxisAlignedPlane"/> </summary>
        public Plane PlaneOfExistence { get; }
        /// <summary> The bounding box of the <see cref="AxisAlignedPlane"/></summary>
        public AxisAlignedBox BoundingBox { get; }
        /// <summary> The normal of the <see cref="AxisAlignedPlane"/> </summary>
        public Normal3 Normal => PlaneOfExistence.Normal;
        /// <summary> The position of the <see cref="AxisAlignedPlane"/> </summary>
        public Position3 Position => PlaneOfExistence.Position;
        /// <summary> The surface area of the <see cref="AxisAlignedPlane"/> </summary>
        public float SurfaceArea => float.PositiveInfinity;

        /// <summary> Create an <see cref="AxisAlignedPlane"/> with a normal in the X-direction </summary>
        /// <param name="positive">Whether the normal is positive or not</param>
        /// <param name="xPosition">The position of the <see cref="AxisAlignedPlane"/></param>
        /// <returns>The <see cref="AxisAlignedPlane"/></returns>
        public static AxisAlignedPlane X(bool positive, float xPosition) => new(positive ? Unit3.X : Unit3.MinX, xPosition);

        /// <summary> Create an <see cref="AxisAlignedPlane"/> with a normal in the Y-direction </summary>
        /// <param name="positive">Whether the normal is positive or not</param>
        /// <param name="yPosition">The position of the <see cref="AxisAlignedPlane"/></param>
        /// <returns>The <see cref="AxisAlignedPlane"/></returns>
        public static AxisAlignedPlane Y(bool positive, float yPosition) => new(positive ? Unit3.Y : Unit3.MinY, yPosition);

        /// <summary> Create an <see cref="AxisAlignedPlane"/> with a normal in the Z-direction </summary>
        /// <param name="positive">Whether the normal is positive or not</param>
        /// <param name="zPosition">The position of the <see cref="AxisAlignedPlane"/></param>
        /// <returns>The <see cref="AxisAlignedPlane"/></returns>
        public static AxisAlignedPlane Z(bool positive, float zPosition) => new(positive ? Unit3.Z : Unit3.MinZ, zPosition);

        /// <summary> Create a new <see cref="AxisAlignedPlane"/> using a <paramref name="unit"/> and a <paramref name="distance"/> </summary>
        /// <param name="unit">The normal of the <see cref="AxisAlignedPlane"/></param>
        /// <param name="distance">The distance of the <see cref="AxisAlignedPlane"/> from the origin along the <paramref name="unit"/></param>
        public AxisAlignedPlane(Unit3 unit, float distance) {
            Normal3 normal = new(unit);
            PlaneOfExistence = new Plane(normal, distance);
            BoundingBox = new AxisAlignedBox(normal.Vector * Vector3.NegativeInfinity, normal.Vector * Vector3.PositiveInfinity);
        }

        /// <summary> Create a new <see cref="AxisAlignedPlane"/> using a <paramref name="unit"/> and a <paramref name="distance"/> </summary>
        /// <param name="position">The position of the <see cref="AxisAlignedPlane"/></param>
        /// <param name="unit">The normal of the <see cref="AxisAlignedPlane"/></param>
        public AxisAlignedPlane(Position3 position, Unit3 unit) {
            Normal3 normal = new(unit);
            PlaneOfExistence = new Plane(position, normal);
            BoundingBox = new AxisAlignedBox(normal.Vector * Vector3.NegativeInfinity, normal.Vector * Vector3.PositiveInfinity);
        }

        public static bool operator ==(AxisAlignedPlane left, AxisAlignedPlane right) => left.Equals(right);
        public static bool operator !=(AxisAlignedPlane left, AxisAlignedPlane right) => !(left == right);

        public override int GetHashCode() => PlaneOfExistence.GetHashCode();
        public override bool Equals(object? obj) => obj is AxisAlignedPlane plane && Equals(plane);
        public bool Equals(AxisAlignedPlane other) => PlaneOfExistence.Equals(other.PlaneOfExistence);

        public Position1? IntersectDistance(IRay ray) => PlaneOfExistence.IntersectDistance(ray);
        public bool OnSurface(Position3 position, float epsilon = 0.001F) => PlaneOfExistence.OnSurface(position, epsilon);
        public Position3 SurfacePosition(Random random) => PlaneOfExistence.SurfacePosition(random);
        public Position2 UVPosition(Position3 position) => PlaneOfExistence.UVPosition(position);
    }
}
