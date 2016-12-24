using OpenTK;
using System.Collections.Generic;

namespace template
{
    class plane : primitive
    {
        public Vector3 normal;
        public float distance;

        public plane(Vector3 normal, float distance, Vector3 color, float specularity = 0, float glossyness = 0, float glossSpecularity = 0)
        {
            this.normal = normal.Normalized();
            this.distance = distance;
            this.color = color;
            this.specularity = specularity;
            this.glossyness = glossyness;
            this.glossSpecularity = glossSpecularity;
        }

        public override float Intersect(ray ray)
        {
            return -((Vector3.Dot(ray.origin, normal) - distance) / Vector3.Dot(ray.direction, normal));
        }

        public override bool IntersectBool(ray ray)
        {
            float intersectDistance = Intersect(ray);
            if (intersectDistance > 0 && intersectDistance < ray.length)
                return true;
            else
                return false;
        }

        public override Vector3 GetNormal(Vector3 intersectionPoint)
        {
            return normal;
        }

        public override Vector3 GetCenter()
        {
            return normal * distance;
        }

        public override List<Vector3> GetBounds()
        {
            List<Vector3> bounds = new List<Vector3>(2);
            bounds.Add(GetCenter() - (Vector3.One - normal) * 100);
            bounds.Add(GetCenter() + (Vector3.One - normal) * 100);
            return bounds;
        }
    }
}
