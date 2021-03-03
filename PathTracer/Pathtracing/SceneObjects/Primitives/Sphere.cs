using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    /// <summary> A sphere primitive for the 3d scene </summary>
    public class Sphere : Primitive {
        /// <summary> The radius of the sphere </summary>
        public float Radius { get; set; }
        /// <summary> Get the AABB bounds of the sphere </summary>
        public override Vector3[] Bounds => new Vector3[] { Position - Vector3.One * Radius, Position + Vector3.One * Radius };

        /// <summary> Create a new sphere object for the 3d scene </summary>
        /// <param name="position">The position of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="material">The material of the sphere</param>
        public Sphere(Vector3 position, float radius = 1, Material? material = null) : base(position, material) {
            Radius = radius;
        }

        public override Vector3 GetSurfacePoint(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the sphere at a point of intersection </summary>
        /// <param name="surfacePoint">The intersection point to get the normal at</param>
        /// <returns>The normal at the point of intersection</returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return (surfacePoint - Position).Normalized();
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>Whether the ray intersects the sphere</returns>
        public override bool IntersectBool(Ray ray) {
            return Intersect(ray).HasValue;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>The intersection with the sphere if there is any</returns>
        public override float? Intersect(Ray ray) {
            Vector3 sphereFromRayOrigin = Position - ray.Origin;
            float sphereInDirectionOfRay = Vector3.Dot(sphereFromRayOrigin, ray.Direction);
            float rayNormalDistance = Vector3.Dot(sphereFromRayOrigin, sphereFromRayOrigin) - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) {
                return null;
            }
            float raySphereDist = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            float intersection1 = sphereInDirectionOfRay - raySphereDist;
            float intersection2 = sphereInDirectionOfRay + raySphereDist;
            if (ray.WithinBounds(intersection1)) {
                return intersection1;
            } else if (ray.WithinBounds(intersection2)) {
                return intersection2;
            } else {
                return null;
            }
        }
    }
}