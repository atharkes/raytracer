using OpenTK.Mathematics;
using PathTracer.Pathtracing.AccelerationStructures.BVH;
using PathTracer.Pathtracing.AccelerationStructures.SBVH;
using System;
using System.Diagnostics;

namespace PathTracer.Pathtracing.SceneObjects {
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

        /// <summary> Create a new <see cref="Primitive"/> </summary>
        /// <param name="position">The <see cref="Position"/> of the <see cref="Primitive"/></param>
        /// <param name="material">The <see cref="Material"/> of the <see cref="Primitive"/></param>
        protected Primitive(Vector3? position = null, Material? material = null) {
            Position = position ?? Vector3.Zero;
            Material = material ?? Material.Random();
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Primitive"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the location of the point </param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Primitive"/></returns>
        public abstract Vector3 GetSurfacePoint(Random random);

        /// <summary> Get the normal at a <paramref name="surfacePoint"/> on this <see cref="Primitive"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal at</param>
        /// <returns>The normal at the specified <paramref name="surfacePoint"/> on the <see cref="Primitive"/></returns>
        public abstract Vector3 GetNormal(Vector3 surfacePoint);

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Primitive"/></returns>
        public abstract bool IntersectBool(Ray ray);

        /// <summary> Intersect this <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>The distance if the <see cref="Primitive"/> is hit by the <paramref name="ray"/></returns>
        public abstract float? Intersect(Ray ray);

        /// <summary> Clip the <see cref="Bounds"/> of the <see cref="Primitive"/> using a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="Bounds"/> with</param>
        /// <returns>The <see cref="PrimitiveFragment"/> with the clipped <see cref="Bounds"/> </returns>
        public virtual PrimitiveFragment? Clip(AxisAlignedPlane plane) {
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

        public virtual Vector3 GetEmmitance(Ray ray) {
            return Material.EmittingLight * Vector3.Dot(GetNormal(ray.Destination), ray.Direction);
        }
    }
}