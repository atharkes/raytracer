using OpenTK;
using WhittedRaytracer.Raytracing.AccelerationStructures.BVH;

namespace WhittedRaytracer.Raytracing.SceneObjects {
    /// <summary> An abstract primitive for the 3d scene </summary>
    public abstract class Primitive : ISceneObject, IAABB {
        /// <summary> The position of the primitive </summary>
        public Vector3 Position { get; set; }
        /// <summary> The material of the primitive </summary>
        public Material Material { get; set; }
        /// <summary> The center of the AABB of the primitive equals the position </summary>
        public Vector3 AABBCenter => Position;
        /// <summary> Get the AABB bounds of this primitive </summary>
        public abstract (Vector3 Min, Vector3 Max) AABBBounds { get; }

        /// <summary> Create a new primitive for the 3d scene </summary>
        /// <param name="position">The position of the primitive</param>
        /// <param name="material">The material of the primitive</param>
        protected Primitive(Vector3? position = null, Material material = null) {
            Position = position ?? Vector3.Zero;
            Material = material ?? Material.Random();
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
    }
}