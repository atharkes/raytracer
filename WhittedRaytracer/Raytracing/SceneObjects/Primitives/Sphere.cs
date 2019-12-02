using OpenTK;
using System;
using System.Collections.Generic;

namespace WhittedRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A sphere primitive for the 3d scene </summary>
    class Sphere : Primitive {
        /// <summary> The radius of the sphere </summary>
        public float Radius;

        /// <summary> Create a new sphere object for the 3d scene </summary>
        /// <param name="position">The position of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="color">The color of the sphere</param>
        /// <param name="specularity">The specularity of the sphere</param>
        /// <param name="glossyness">The glossyness of the sphere</param>
        /// <param name="glossSpecularity">The gloss specularity of the sphere</param>
        public Sphere(Vector3 position, float radius, Vector3? color = null, float specularity = 0, float glossyness = 0, float glossSpecularity = 0)
            : base(position, color, specularity, glossyness, glossSpecularity) {
            Radius = radius;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>The distance at which the ray intersects the sphere</returns>
        public override float Intersect(Ray ray) {
            Vector3 circlePos = Position - ray.Origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.Direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.Direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > Radius * Radius) return -1f;
            rayCircleDot -= (float)Math.Sqrt(Radius * Radius - rayNormalDot);
            return rayCircleDot;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>Whether the ray intersects the sphere</returns>
        public override bool IntersectBool(Ray ray) {
            Vector3 circlePos = Position - ray.Origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.Direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.Direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > Radius * Radius || rayCircleDot < 0 || rayCircleDot > ray.Length) {
                return false;
            } else {
                return true;
            }   
        }

        /// <summary> Get the normal of the sphere at a point of intersection </summary>
        /// <param name="intersectionPoint">The intersection point to get the normal at</param>
        /// <returns>The normal at the point of intersection</returns>
        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return (intersectionPoint - Position).Normalized();
        }

        /// <summary> Get the center of the sphere </summary>
        /// <returns>The center of the sphere</returns>
        public override Vector3 GetCenter() {
            return Position;
        }

        /// <summary> Get the bounds of the sphere </summary>
        /// <returns>The bounds of the sphere</returns>
        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2)
            {
                Position - Vector3.One * Radius,
                Position + Vector3.One * Radius
            };
            return bounds;
        }
    }
}