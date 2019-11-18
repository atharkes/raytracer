using OpenTK;
using System.Collections.Generic;

namespace raytracer {
    class Triangle : Primitive {
        Vector3 p1, p2, p3;
        Vector3 normal;
        const float epsilon = 0.000001f;

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 color, float specularity = 0, float glossyness = 0, float glossSpecularity = 0)
            : base (null, color, specularity, glossyness, glossSpecularity) {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            normal = Vector3.Cross(p2 - p1, p3 - p1).Normalized();
            Position = (p1 + p2 + p3) * 0.333333f;
        }

        public override float Intersect(Ray ray) {
            // Get vectors for two edges sharing V1
            Vector3 e1 = p2 - p1;
            Vector3 e2 = p3 - p1;

            // Begin calculating determinant - also used to calculate u parameter
            Vector3 P = Vector3.Cross(ray.Direction, e2);
            // If determinant is near zero, ray lies in plane of triangle
            float determinant = Vector3.Dot(e1, P);
            if (determinant > -epsilon && determinant < epsilon) return -1f;
            float determinantInverted = 1f / determinant;

            // Calculate distance from V1 to ray origin
            Vector3 T = ray.Origin - p1;
            // Calculate u parameter and test bound
            float u = Vector3.Dot(T, P) * determinantInverted;
            if (u < 0f || u > 1f)  return -1f;

            // Prepare to test v parameter
            Vector3 Q = Vector3.Cross(T, e1);
            // Calculate V parameter and test bound
            float v = Vector3.Dot(ray.Direction, Q) * determinantInverted;
            if (v < 0f || u + v > 1f)  return -1f;

            float t = Vector3.Dot(e2, Q) * determinantInverted;
            if (t > epsilon) return t;

            return -1f;
        }

        public override bool IntersectBool(Ray ray) {
            float intersectDistance = Intersect(ray);
            if (intersectDistance > 0 && intersectDistance < ray.Length) {
                return true;
            } else {
                return false;
            }
        }

        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return normal;
        }

        public override Vector3 GetCenter() {
            return Position;
        }

        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2)
            {
                Vector3.ComponentMin(p1, Vector3.ComponentMin(p2, p3)),
                Vector3.ComponentMax(p1, Vector3.ComponentMax(p2, p3))
            };
            return bounds;
        }
    }
}