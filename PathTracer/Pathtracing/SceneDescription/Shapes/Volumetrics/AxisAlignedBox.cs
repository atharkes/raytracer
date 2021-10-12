using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> An axis aligned box primitive </summary>
    public class AxisAlignedBox : VolumetricShape {
        /// <summary> The bounds of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3[] Bounds { get; }
        /// <summary> Whether the normals of the <see cref="AxisAlignedBox"/> are pointing inwards or outwards </summary>
        public bool InwardNormals { get; set; } = false;

        /// <summary> The minimum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 MinCorner { get => Bounds[0]; set => Bounds[0] = value; }
        /// <summary> The maximum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 MaxCorner { get => Bounds[1]; set => Bounds[1] = value; }
        /// <summary> The size of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 Size => MaxCorner - MinCorner;
        /// <summary> The center of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 Center => MinCorner + Size / 2f;

        /// <summary> The volume of the <see cref="AxisAlignedBox"/> </summary>
        public override float Volume => Size.X * Size.Y * Size.Z;
        /// <summary> The surface area of the <see cref="AxisAlignedBox"/> </summary>
        public override float SurfaceArea => (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z) * 2f;
        /// <summary> The bounding box of the <see cref="AxisAlignedBox"/> </summary>
        public override AxisAlignedBox BoundingBox => this;

        /// <summary> Create an <see cref="AxisAlignedBox"/> using two corners </summary>
        /// <param name="corner1">The first corner of the <see cref="AxisAlignedBox"/></param>
        /// <param name="corner2">The corner on the opposite side of the <see cref="AxisAlignedBox"/></param>
        public AxisAlignedBox(Vector3 corner1, Vector3 corner2) {
            Vector3 minCorner = Vector3.ComponentMin(corner1, corner2);
            Vector3 maxCorner = Vector3.ComponentMax(corner1, corner2);
            Bounds = new Vector3[] { minCorner, maxCorner };
        }

        /// <summary> Create an <see cref="AxisAlignedBox"/> that encompasses <paramref name="positions"/> </summary>
        /// <param name="positions">The positions that the <see cref="AxisAlignedBox"/> should encompass</param>
        public AxisAlignedBox(params Vector3[] positions) {
            Vector3 minCorner = Vector3.PositiveInfinity;
            Vector3 maxCorner = Vector3.NegativeInfinity;
            foreach (Vector3 position in positions) {
                minCorner = Vector3.ComponentMin(minCorner, position);
                maxCorner = Vector3.ComponentMax(maxCorner, position);
            }
            Bounds = new Vector3[] { minCorner, maxCorner };
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position of the point on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="AxisAlignedBox"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="AxisAlignedBox"/></returns>
        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="AxisAlignedBox"/> at a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position to get the normal for</param>
        /// <returns>The normal at the specified <paramref name="position"/></returns>
        public override Vector3 SurfaceNormal(Vector3 position) {
            Vector3 direction = Vector3.Divide(position - Center, Size);
            float x = Math.Abs(direction.X);
            float y = Math.Abs(direction.Y);
            float z = Math.Abs(direction.Z);
            Vector3 normal;
            if (x > y && x > z) {
                normal = direction.X > 0f ? Vector3.UnitX : -Vector3.UnitX;
            } else if (y > z) {
                normal = direction.Y > 0f ? Vector3.UnitY : -Vector3.UnitY;
            } else {
                normal = direction.Z > 0f ? Vector3.UnitZ : -Vector3.UnitZ;
            }
            return InwardNormals ? -normal : normal;
        }

        /// <summary> Intersect the <see cref="AxisAlignedBox"/> by a <paramref name="ray"/>.
        /// Using Amy Williams's "An Efficient and Robust Ray–Box Intersection" Algorithm </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="AxisAlignedBox"/> with</param>
        /// <returns>Whether and when the <see cref="Ray"/> intersects the <see cref="AxisAlignedBox"/></returns>
        public override IEnumerable<float> IntersectDistances(IRay ray) {
            float tmin = (Bounds[ray.Sign.X].X - ray.Origin.X) * ray.InvDirection.X;
            float tmax = (Bounds[1 - ray.Sign.X].X - ray.Origin.X) * ray.InvDirection.X;

            float tymin = (Bounds[ray.Sign.Y].Y - ray.Origin.Y) * ray.InvDirection.Y;
            float tymax = (Bounds[1 - ray.Sign.Y].Y - ray.Origin.Y) * ray.InvDirection.Y;
            if ((tmin > tymax) || (tmax < tymin)) yield break;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign.Z].Z - ray.Origin.Z) * ray.InvDirection.Z;
            float tzmax = (Bounds[1 - ray.Sign.Z].Z - ray.Origin.Z) * ray.InvDirection.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) yield break;
            yield return Math.Max(tmin, tzmin);
            yield return Math.Min(tmax, tzmax);
        }

        /// <summary> Clip the <see cref="AxisAlignedBox"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="AxisAlignedBox"/> with</param>
        /// <returns>A new clipped <see cref="AxisAlignedBox"/> if it's not clipped entirely</returns>
        public override IEnumerable<AxisAlignedBox> Clip(AxisAlignedPlane plane) {
            Vector3 min = MinCorner;
            Vector3 max = MaxCorner;

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
            }
            if (min == MinCorner && max == MaxCorner) {
                yield return this;
            } else if (min.X < max.X && min.Y < max.Y && min.Z < max.Z) {
                yield return new AxisAlignedBox(min, max);
            }
        }
    }
}
