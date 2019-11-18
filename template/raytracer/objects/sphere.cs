using OpenTK;
using System;
using System.Collections.Generic;

namespace template {
    class Sphere : Primitive {
        public Vector3 Position;
        public float Radius;

        public Sphere(Vector3 position, float radius, Vector3 color, float specularity = 0, float glossyness = 0, float glossSpecularity = 0) {
            Position = position;
            Radius = radius;
            Color = color;
            Specularity = specularity;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        public override float Intersect(Ray ray) {
            Vector3 circlePos = Position - ray.Origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.Direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.Direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > Radius * Radius)
                return -1f;
            rayCircleDot -= (float)Math.Sqrt(Radius * Radius - rayNormalDot);
            return rayCircleDot;
        }

        public override bool IntersectBool(Ray ray) {
            Vector3 circlePos = Position - ray.Origin;
            float rayCircleDot = Vector3.Dot(circlePos, ray.Direction);
            Vector3 rayNormal = circlePos - rayCircleDot * ray.Direction;
            float rayNormalDot = Vector3.Dot(rayNormal, rayNormal);
            if (rayNormalDot > Radius * Radius || rayCircleDot < 0 || rayCircleDot > ray.Length)
                return false;
            else
                return true;
        }

        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return (intersectionPoint - Position).Normalized();
        }

        public override Vector3 GetCenter() {
            return Position;
        }

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