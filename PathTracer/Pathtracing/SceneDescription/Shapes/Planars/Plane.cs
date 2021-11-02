using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Plane"/> <see cref="Shape"/> </summary>
    public struct Plane : IPlanarShape {
        /// <summary> Normal vector of the <see cref="Plane"/> </summary>
        public Normal3 Normal { get; set; }
        /// <summary> Distance of the <see cref="Plane"/> from the origin </summary>
        public Position1 Distance { get; set; }
        /// <summary> The position of the <see cref="Plane"/> </summary>
        public Position3 Position => Position3.Origin + Normal * Distance;

        /// <summary> The surface area of the <see cref="Plane"/> </summary>
        public float SurfaceArea => float.PositiveInfinity;
        /// <summary> The plane of existence of the plane is the plane itself </summary>
        public Plane PlaneOfExistence => this;
        /// <summary> The bounding box of the <see cref="Plane"/> </summary>
        public AxisAlignedBox BoundingBox => new(Position3.NegativeInfinity, Position3.PositiveInfinity);

        /// <summary> Create a new <see cref="Plane"/> using a <paramref name="normal"/> and a <paramref name="distance"/> </summary>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        /// <param name="distance">The distance of the <see cref="Plane"/> from the origin along the <paramref name="normal"/></param>
        public Plane(Normal3 normal, Position1 distance) {
            Normal = normal;
            Distance = distance;
        }

        /// <summary> Create a new <see cref="Plane"/> with a <paramref name="position"/> on the plane and a <paramref name="normal"/> </summary>
        /// <param name="position">The position on the <see cref="Plane"/></param>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        public Plane(Position3 position, Normal3 normal) {
            Normal = normal;
            Distance = Position3.Dot(position, normal);
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Plane"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Plane"/></returns>
        public Position3 SurfacePosition(Random random) => throw new NotImplementedException();

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        public Position2 UVPosition(Position3 position) => throw new NotImplementedException();

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Plane"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Plane"/></returns>
        public bool OnSurface(Position3 position, float epsilon = 0.001F) => throw new NotImplementedException();

        /// <summary> Get the normal of the <see cref="Plane"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Plane"/></returns>
        public Normal3 SurfaceNormal(Position3 surfacePoint) => Normal;

        /// <summary> Intersect the <see cref="Plane"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Plane"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> interescts the <see cref="Plane"/></returns>
        public Position1? IntersectDistance(IRay ray) {
           return -((Vector3.Dot(ray.Origin.Vector, Normal.Vector) - Distance.Vector) / Vector3.Dot(ray.Direction.Vector, Normal.Vector));
        }
    }
}