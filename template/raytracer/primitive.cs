using OpenTK;
using System.Collections.Generic;

namespace template {
    abstract class Primitive {
        public Vector3 color;
        public float specularity;
        public float glossyness;
        public float glossSpecularity;

        public virtual float Intersect(Ray ray) { return 0f; }
        public virtual bool IntersectBool(Ray ray) { return false; }
        public virtual Vector3 GetNormal(Vector3 intersectionPoint) { return new Vector3(0, 0, 0); }
        public virtual Vector3 GetCenter() { return new Vector3(0, 0, 0); }
        public virtual List<Vector3> GetBounds() { return new List<Vector3>(); }
    }
}