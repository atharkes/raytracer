using OpenTK;
using System;
using System.Collections.Generic;

namespace template {
    class Sphere : Primitive {
        public Vector3 position;
        public float radius;

        public Sphere(Vector3 position, float radius, Vector3 color, float specularity = 0, float glossyness = 0, float glossSpecularity = 0) {
            this.position = position;
            this.radius = radius;
            this.color = color;
            this.specularity = specularity;
            this.glossyness = glossyness;
            this.glossSpecularity = glossSpecularity;
        }

        public override float Intersect(Ray ray) {
            Vector3 circlePos = position - ray.origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > radius * radius)
                return -1f;
            rayCircleDot -= (float)Math.Sqrt(radius * radius - rayNormalDot);
            return rayCircleDot;
        }

        public override bool IntersectBool(Ray ray) {
            Vector3 circlePos = position - ray.origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > radius * radius || rayCircleDot < 0 || rayCircleDot > ray.length)
                return false;
            else
                return true;
        }

        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return (intersectionPoint - position).Normalized();
        }

        public override Vector3 GetCenter() {
            return position;
        }

        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2);
            bounds.Add(position - Vector3.One * radius);
            bounds.Add(position + Vector3.One * radius);
            return bounds;
        }
    }
}