using OpenTK;
using System.Collections.Generic;

namespace template {
    class Plane : Primitive {
        public Vector3 Normal;
        public float Distance;

        public Plane(Vector3 normal, float distance, Vector3 color, float specularity = 0, float glossyness = 0, float glossSpecularity = 0) {
            Normal = normal.Normalized();
            Distance = distance;
            Color = color;
            Specularity = specularity;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        public override float Intersect(Ray ray) {
            return -((Vector3.Dot(ray.Origin, Normal) - Distance) / Vector3.Dot(ray.Direction, Normal));
        }

        public override bool IntersectBool(Ray ray) {
            float intersectDistance = Intersect(ray);
            if (intersectDistance > 0 && intersectDistance < ray.Length)
                return true;
            else
                return false;
        }

        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return Normal;
        }

        public override Vector3 GetCenter() {
            return Normal * Distance;
        }

        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2)
            {
                GetCenter() - (Vector3.One - Normal) * 100,
                GetCenter() + (Vector3.One - Normal) * 100
            };
            return bounds;
        }
    }
}