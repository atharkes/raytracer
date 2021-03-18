using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    /// <summary> A <see cref="Plane"/> <see cref="Primitive"/> </summary>
    class Plane : Primitive {
        /// <summary> Normal vector of the <see cref="Plane"/> </summary>
        public Vector3 Normal { get; set; }
        /// <summary> Distance of the <see cref="Plane"/> from the origin </summary>
        public float Distance { get; set; }
        /// <summary> An orthogonal vector in the <see cref="Plane"/> </summary>
        public Vector3 Orthogonal => (Vector3.One - Normal).Normalized();
        /// <summary> Returns the infinite bounds of the <see cref="Plane"/> </summary>
        public override Vector3[] Bounds => new Vector3[] { Vector3.NegativeInfinity, Vector3.PositiveInfinity };
        /// <summary> The surface area of the <see cref="Plane"/> </summary>
        public override float SurfaceArea => float.PositiveInfinity;

        /// <summary> Create a new <see cref="Plane"/> using a <paramref name="normal"/> and a <paramref name="distance"/> </summary>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        /// <param name="distance">The distance of the <see cref="Plane"/> from the origin along the <paramref name="normal"/></param>
        /// <param name="material">The <see cref="Material"/> of the <see cref="Plane"/></param>
        public Plane(Vector3 normal, float distance, Material? material = null) : base(normal.Normalized() * distance, material) {
            Normal = normal.Normalized();
            Distance = distance;
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Plane"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Plane"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Plane"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Plane"/></returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return Normal;
        }

        /// <summary> Intersect the <see cref="Plane"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Plane"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> interescts the <see cref="Plane"/></returns>
        public override float? Intersect(Ray ray) {
            float dist = -((Vector3.Dot(ray.Origin.Position, Normal) - Distance) / Vector3.Dot(ray.Direction, Normal));
            if (ray.WithinBounds(dist)) {
                return dist;
            } else {
                return null;
            }
        }
    }
}