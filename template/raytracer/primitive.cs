using OpenTK;
using System.Collections.Generic;

namespace template {
    abstract class Primitive {
        public Vector3 Color;
        public float Specularity;
        public float Glossyness;
        public float GlossSpecularity;

        public virtual float Intersect(Ray ray) { return 0f; }
        public virtual bool IntersectBool(Ray ray) { return false; }
        public virtual Vector3 GetNormal(Vector3 intersectionPoint) { return Vector3.Zero; }
        public virtual Vector3 GetCenter() { return Vector3.Zero; }
        public virtual List<Vector3> GetBounds() { return new List<Vector3>(); }
    }
}