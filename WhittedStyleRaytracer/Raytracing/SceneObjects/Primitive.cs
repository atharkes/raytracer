using OpenTK;
using System.Collections.Generic;

namespace WhittedStyleRaytracer.Raytracing.SceneObjects {
    /// <summary> A abstract primitive for the 3d scene </summary>
    abstract class Primitive : ISceneObject {
        /// <summary> The position of the primitive </summary>
        public Vector3 Position { get; set; }
        /// <summary> The color of the primitive </summary>
        public Vector3 Color { get; set; }
        /// <summary> The specularity of the primitive </summary>
        public float Specularity { get; set; }
        /// <summary> The glossyness of the primitive </summary>
        public float Glossyness { get; set; }
        /// <summary> The gloss specularity of the primitive </summary>
        public float GlossSpecularity { get; set; }

        /// <summary> Create a new primitive for the 3d scene </summary>
        /// <param name="position">The position of the primitive</param>
        /// <param name="color">The color of the primitive</param>
        /// <param name="specularity">The specularity of the primitive</param>
        /// <param name="glossyness">The glossyness of the primitive</param>
        /// <param name="glossSpecularity">The gloss specularity of the primitive</param>
        protected Primitive(Vector3? position = null, Vector3? color = null, float specularity = 0, float glossyness = 0, float glossSpecularity = 0) {
            Position = position ?? Vector3.Zero;
            Color = color ?? Vector3.One;
            Specularity = specularity;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>The distance the ray travels untill the intersection with this primitive</returns>
        public abstract float Intersect(Ray ray);

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>Whether the ray intersects this primitive</returns>
        public abstract bool IntersectBool(Ray ray);

        /// <summary> Get the normal at an intersection on this primitive </summary>
        /// <param name="intersectionPoint">The point of the intersection</param>
        /// <returns>The normal at the point of intersection on this primitive</returns>
        public abstract Vector3 GetNormal(Vector3 intersectionPoint);

        /// <summary> Get the center of this primitive </summary>
        /// <returns>The center of this primitive</returns>
        public virtual Vector3 GetCenter() { return Position; }

        /// <summary> Get the axis-alinged bounding box bounds of this primitive </summary>
        /// <returns>The axis-alinged bounds of this primitive</returns>
        public abstract List<Vector3> GetBounds();
    }
}