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
        /// <param name="specularity">How specular the sphere is. A specular object reflects light like a mirror.</param>
        /// <param name="dielectric">How dielectric the sphere is. A dielectric object both passes light and reflects it like water or glass.</param>
        /// <param name="refractionIndex">The refraction index of the sphere if it is a dielectric. This is typically a value between 1 and 3.</param>
        /// <param name="glossyness">The glossyness of the sphere</param>
        /// <param name="glossSpecularity">The gloss specularity of the sphere</param>
        public Sphere(Vector3 position, float radius, Vector3? color = null, float specularity = 0, float dielectric = 0, float refractionIndex = 1, float glossyness = 0, float glossSpecularity = 0)
            : base(position, color, specularity, dielectric, refractionIndex, glossyness, glossSpecularity) {
            Radius = radius;
        }

        /// <summary> Create a glossy red unit sphere </summary>
        /// <param name="position">The position of the sphere</param>
        /// <returns>A glossy red unit sphere</returns>
        public static Sphere GlossyRed(Vector3 position) {
            return new Sphere(position, 1, new Vector3(0.5f, 0, 0), 0, 0, 1, 0.5f, 10f);
        }

        /// <summary> Create a diffuse green unit sphere </summary>
        /// <param name="position">The position of the sphere</param>
        /// <returns>A diffuse green unit sphere</returns>
        public static Sphere DiffuseGreen(Vector3 position) {
            return new Sphere(position, 1, new Vector3(0, 0.5f, 0));
        }

        /// <summary> Create a mirror unit sphere </summary>
        /// <param name="position">The position of the sphere</param>
        /// <returns>A mirror unit sphere</returns>
        public static Sphere Mirror(Vector3 position) {
            return new Sphere(position, 1, new Vector3(1f, 1f, 1f), 1f);
        }

        /// <summary> Create a glass half-unit sphere </summary>
        /// <param name="position">The position of the sphere</param>
        /// <returns>A glass half-unit sphere</returns>
        public static Sphere Glass(Vector3 position) {
            return new Sphere(position, 0.5f, new Vector3(0.4f, 0.4f, 0.9f), 0, 0.97f, 1.62f);
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>The distance at which the ray intersects the sphere</returns>
        public override float Intersect(Ray ray) {
            //Vector3 circlePos = Position - ray.Origin;
            //float rayCircleDot = Vector3.Dot(circlePos, ray.Direction);
            //Vector3 rayNormal = circlePos - rayCircleDot * ray.Direction;
            //float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            //if (rayNormalDot > Radius * Radius) return -1f;
            //rayCircleDot -= (float)Math.Sqrt(Radius * Radius - rayNormalDot);
            //return rayCircleDot;

            Vector3 sphereFromRayOrigin = Position - ray.Origin;
            float sphereInDirectionOfRay = Vector3.Dot(sphereFromRayOrigin, ray.Direction);
            //Vector3 rayNormal = sphereFromRayOrigin - sphereInDirectionOfRay * ray.Direction;
            //float rayNormalLengthSquared = Vector3.Dot(rayNormal, rayNormal);
            //return rayNormalLengthSquared > Radius * Radius || sphereInDirectionOfRay < 0 || sphereInDirectionOfRay > ray.Length;

            float rayNormalDistance = Vector3.Dot(sphereFromRayOrigin, sphereFromRayOrigin) - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) return -1f;
            float thc = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            float t0 = sphereInDirectionOfRay - thc;
            float t1 = sphereInDirectionOfRay + thc;
            //if (t0 > t1) std::swap(t0, t1);

            //if (t0 < 0) {
            //    t0 = t1; // if t0 is negative, let's use t1 instead 
            //    if (t0 < 0) return false; // both t0 and t1 are negative 
            //}
            if (t0 > 0 && t0 < ray.Length) return t0;
            else if (t1 > 0 && t1 < ray.Length) return t1;
            else return - 1f;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>Whether the ray intersects the sphere</returns>
        public override bool IntersectBool(Ray ray) {
            Vector3 sphereFromRayOrigin = Position - ray.Origin;
            float sphereInDirectionOfRay = Vector3.Dot(sphereFromRayOrigin, ray.Direction);
            //Vector3 rayNormal = sphereFromRayOrigin - sphereInDirectionOfRay * ray.Direction;
            //float rayNormalLengthSquared = Vector3.Dot(rayNormal, rayNormal);
            //return rayNormalLengthSquared > Radius * Radius || sphereInDirectionOfRay < 0 || sphereInDirectionOfRay > ray.Length;

            float rayNormalDistance = Vector3.Dot(sphereFromRayOrigin, sphereFromRayOrigin) - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) return false;
            float thc = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            float t0 = sphereInDirectionOfRay - thc;
            float t1 = sphereInDirectionOfRay + thc;
            //if (t0 > t1) std::swap(t0, t1);

            //if (t0 < 0) {
            //    t0 = t1; // if t0 is negative, let's use t1 instead 
            //    if (t0 < 0) return false; // both t0 and t1 are negative 
            //}

            return (t0 > 0 && t0 < ray.Length) || (t1 > 0 && t1 < ray.Length);
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