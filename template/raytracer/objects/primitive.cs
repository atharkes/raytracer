using OpenTK;
using System.Collections.Generic;

namespace raytracer {
    abstract class Primitive {
        public Vector3 Position;
        public Vector3 Color;
        public float Specularity;
        public float Glossyness;
        public float GlossSpecularity;

        protected Primitive(Vector3? position = null, Vector3? color = null, float specularity = 0, float glossyness = 0, float glossSpecularity = 0) {
            Position = position ?? Vector3.Zero;
            Color = color ?? Vector3.One;
            Specularity = specularity;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        public virtual float Intersect(Ray ray) { return 0f; }
        public virtual bool IntersectBool(Ray ray) { return false; }
        public virtual Vector3 GetNormal(Vector3 intersectionPoint) { return Vector3.Zero; }
        public virtual Vector3 GetCenter() { return Vector3.Zero; }
        public virtual List<Vector3> GetBounds() { return new List<Vector3>(); }
    }
}