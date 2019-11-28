using OpenTK;
using System.Collections.Generic;

namespace WhittedStyleRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A triangle primitive for the 3d scene </summary>
    class Triangle : Primitive {
        /// <summary> A point of the triangle </summary>
        public Vector3 P1, P2, P3;
        /// <summary> The normal of the triangle </summary>
        public Vector3 Normal;

        /// <summary> Epsilon used for the Möller–Trumbore triangle intersection </summary>
        public const float IntersectionEpsilon = 0.0000001f;

        /// <summary> Create a new triangle object for the 3d scene </summary>
        /// <param name="p1">The first point of the triangle</param>
        /// <param name="p2">The second point of the triangle</param>
        /// <param name="p3">The third point of the triangle</param>
        /// <param name="color">The color of the triangle</param>
        /// <param name="specularity">The specularity of the triangle</param>
        /// <param name="glossyness">The glossyness of the triangle</param>
        /// <param name="glossSpecularity">The gloss specularity of the triangle</param>
        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3? color = null, float specularity = 0, float glossyness = 0, float glossSpecularity = 0)
            : base (null, color, specularity, glossyness, glossSpecularity) {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Normal = Vector3.Cross(p2 - p1, p3 - p1).Normalized();
            Position = (p1 + p2 + p3) * 0.333333f;
        }

        /// <summary> Intersect the triangle with a ray (Möller–Trumbore triangle intersection) </summary>
        /// <param name="ray">The ray to intersect the triangle with</param>
        /// <returns>The distance at which the ray intersects the triangle</returns>
        public override float Intersect(Ray ray) {
            // Get vectors for two edges sharing V1
            Vector3 e1 = P2 - P1;
            Vector3 e2 = P3 - P1;

            // Begin calculating determinant - also used to calculate u parameter
            Vector3 P = Vector3.Cross(ray.Direction, e2);
            // If determinant is near zero, ray lies in plane of triangle
            float determinant = Vector3.Dot(e1, P);
            if (determinant > -IntersectionEpsilon && determinant < IntersectionEpsilon) return -1f;
            float determinantInverted = 1f / determinant;

            // Calculate distance from V1 to ray origin
            Vector3 T = ray.Origin - P1;
            // Calculate u parameter and test bound
            float u = Vector3.Dot(T, P) * determinantInverted;
            if (u < 0f || u > 1f)  return -1f;

            // Prepare to test v parameter
            Vector3 Q = Vector3.Cross(T, e1);
            // Calculate V parameter and test bound
            float v = Vector3.Dot(ray.Direction, Q) * determinantInverted;
            if (v < 0f || u + v > 1f)  return -1f;

            float t = Vector3.Dot(e2, Q) * determinantInverted;
            if (t > IntersectionEpsilon) return t;

            return -1f;
        }

        /// <summary> Intersect the triangle with a ray </summary>
        /// <param name="ray">The ray to intersect the triangle with</param>
        /// <returns>Whether the ray intersects the triangle</returns>
        public override bool IntersectBool(Ray ray) {
            float intersectDistance = Intersect(ray);
            if (intersectDistance > 0 && intersectDistance < ray.Length) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary> Get the normal of the triangle </summary>
        /// <param name="intersectionPoint">The intersection point to get the normal at</param>
        /// <returns>The normal of the triangle at the intersection point</returns>
        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return Normal;
        }

        /// <summary> Get the center of the triangle </summary>
        /// <returns>The center of the triangle</returns>
        public override Vector3 GetCenter() {
            return Position;
        }

        /// <summary> Get the bounds of the triangle </summary>
        /// <returns>The bounds of the triangle</returns>
        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2)
            {
                Vector3.ComponentMin(P1, Vector3.ComponentMin(P2, P3)),
                Vector3.ComponentMax(P1, Vector3.ComponentMax(P2, P3))
            };
            return bounds;
        }
    }
}