using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    /// <summary> A plane for the 3d scene </summary>
    class Plane : Primitive {
        /// <summary> Normal vector of the <see cref="Plane"/> </summary>
        public Vector3 Normal { get; set; }
        /// <summary> Distance of the <see cref="Plane"/> from the origin </summary>
        public float Distance { get; set; }
        /// <summary> An orthogonal vector in the <see cref="Plane"/> </summary>
        public Vector3 Orthogonal => (Vector3.One - Normal).Normalized();
        /// <summary> Returns the infinite bounds of the <see cref="Plane"/> </summary>
        public override Vector3[] Bounds => new Vector3[] { Vector3.NegativeInfinity, Vector3.PositiveInfinity };

        /// <summary> Create a new <see cref="Plane"/> using a <paramref name="normal"/> and a <paramref name="distance"/> </summary>
        /// <param name="normal">The normal of the plane</param>
        /// <param name="distance">The distance of the plane from the origin</param>
        /// <param name="material">The material of the plane</param>
        public Plane(Vector3 normal, float distance, Material? material = null) : base(normal.Normalized() * distance, material) {
            Normal = normal.Normalized();
            Distance = distance;
        }

        public override Vector3 GetSurfacePoint(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the plane </summary>
        /// <param name="surfacePoint">The point to get the normal at</param>
        /// <returns>The normal of the plane</returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return Normal;
        }

        /// <summary> Intersect the plane with a ray </summary>
        /// <param name="ray">The ray to intersect the plane with</param>
        /// <returns>Whether the ray intersects the plane</returns>
        public override bool IntersectBool(Ray ray) {
            return Intersect(ray).HasValue;
        }

        /// <summary> Returns the distance of the intersection </summary>
        /// <param name="ray">The ray to intersect this plane with</param>
        /// <returns>The distance of the intersection</returns>
        public override float? Intersect(Ray ray) {
            float dist = -((Vector3.Dot(ray.Origin, Normal) - Distance) / Vector3.Dot(ray.Direction, Normal));
            if (ray.WithinBounds(dist)) {
                return dist;
            } else {
                return null;
            }
        }
    }
}