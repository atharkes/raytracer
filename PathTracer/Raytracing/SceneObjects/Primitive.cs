using OpenTK.Mathematics;
using PathTracer.Raytracing.AccelerationStructures.BVH;
using PathTracer.Raytracing.AccelerationStructures.SBVH;
using System;

namespace PathTracer.Raytracing.SceneObjects {
    /// <summary> An abstract primitive for the 3d scene </summary>
    public abstract class Primitive : ISceneObject, IAABB {
        /// <summary> The position of the primitive </summary>
        public Vector3 Position { get; set; }
        /// <summary> The material of the primitive </summary>
        public Material Material { get; set; }
        /// <summary> Get the AABB bounds of this primitive </summary>
        public abstract Vector3[] Bounds { get; }
        /// <summary> The size of the bounds of the primitive </summary>
        public Vector3 Size => Bounds[1] - Bounds[0];
        /// <summary> The center of the AABB of the primitive equals the position </summary>
        public Vector3 Center => Position;

        /// <summary> Create a new primitive for the 3d scene </summary>
        /// <param name="position">The position of the primitive</param>
        /// <param name="material">The material of the primitive</param>
        protected Primitive(Vector3? position = null, Material material = null) {
            Position = position ?? Vector3.Zero;
            Material = material ?? Material.Random();
        }

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>An intersection if the primitive is hit</returns>
        public abstract Intersection Intersect(Ray ray);

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>Whether the ray intersects this primitive</returns>
        public abstract bool IntersectBool(Ray ray);

        /// <summary> Get the normal at an intersection on this primitive </summary>
        /// <param name="intersectionPoint">The point of the intersection</param>
        /// <returns>The normal at the point of intersection on this primitive</returns>
        public abstract Vector3 GetNormal(Vector3 intersectionPoint);

        /// <summary> Clip the AABB of the primitive with an axis-aligned plane </summary>
        /// <param name="plane">The plane to clip the AABB with</param>
        /// <returns>The bounds of the clipped AABB</returns>
        public virtual PrimitiveFragment Clip(AxisAlignedPlane plane) {
            Vector3[] bounds = Bounds;
            Vector3 min = bounds[0];
            Vector3 max = bounds[1];
            if (plane.Normal == Vector3.UnitX) {
                min.X = Math.Max(min.X, plane.Position.X);
            } else if (plane.Normal == -Vector3.UnitX) {
                max.X = Math.Min(max.X, plane.Position.X);
            } else if (plane.Normal == Vector3.UnitY) {
                min.Y = Math.Max(min.Y, plane.Position.Y);
            } else if (plane.Normal == -Vector3.UnitY) {
                max.Y = Math.Min(max.Y, plane.Position.Y);
            } else if (plane.Normal == Vector3.UnitZ) {
                min.Z = Math.Max(min.Z, plane.Position.Z);
            } else if (plane.Normal == -Vector3.UnitZ) {
                max.Z = Math.Min(max.Z, plane.Position.Z);
            } else throw new ArgumentException("Can't clip if plane is not axis-aligned");
            if (max.X < min.X || max.Y < min.Y || max.Z < min.Z) return null;
            else return new PrimitiveFragment(this, new Vector3[] { min, max });
        }
    }
}