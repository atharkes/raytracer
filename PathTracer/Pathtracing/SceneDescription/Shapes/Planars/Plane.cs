using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Plane"/> <see cref="Shape"/> </summary>
    public class Plane : PlanarShape {
        /// <summary> Normal vector of the <see cref="Plane"/> </summary>
        public Vector3 Normal { get; set; }
        /// <summary> Distance of the <see cref="Plane"/> from the origin </summary>
        public float Distance { get; set; }
        /// <summary> The position of the <see cref="Plane"/> </summary>
        public Vector3 Position => Normal * Distance;

        /// <summary> The surface area of the <see cref="Plane"/> </summary>
        public override float SurfaceArea => float.PositiveInfinity;
        /// <summary> The plane of existence of the plane is the plane itself </summary>
        public override Plane PlaneOfExistence => this;
        /// <summary> The bounding box of the <see cref="Plane"/> </summary>
        public override AxisAlignedBox BoundingBox => new(Vector3.NegativeInfinity, Vector3.PositiveInfinity);

        /// <summary> Create a new <see cref="Plane"/> using a <paramref name="normal"/> and a <paramref name="distance"/> </summary>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        /// <param name="distance">The distance of the <see cref="Plane"/> from the origin along the <paramref name="normal"/></param>
        public Plane(Vector3 normal, float distance) {
            Normal = normal.Normalized();
            Distance = distance;
        }

        /// <summary> Create a new <see cref="Plane"/> with a <paramref name="position"/> on the plane and a <paramref name="normal"/> </summary>
        /// <param name="position">The position on the <see cref="Plane"/></param>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        public Plane(Vector3 position, Vector3 normal) {
            Normal = normal;
            Distance = Vector3.Dot(position, normal);
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Plane"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Plane"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Plane"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Plane"/></returns>
        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Plane"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Plane"/></returns>
        public override Vector3 SurfaceNormal(Vector3 surfacePoint) {
            return Normal;
        }

        /// <summary> Intersect the <see cref="Plane"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Plane"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> interescts the <see cref="Plane"/></returns>
        public override float? IntersectDistance(IRay ray) {
           return -((Vector3.Dot(ray.Origin, Normal) - Distance) / Vector3.Dot(ray.Direction, Normal));
        }
    }
}