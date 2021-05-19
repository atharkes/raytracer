using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> An axis aligned box primitive </summary>
    public class AxisAlignedBox : Shape {
        /// <summary> The bounds of the <see cref="AxisAlignedBox"/> </summary>
        public override Vector3[] Bounds { get; }
        /// <summary> The minimum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 MinCorner { get => Bounds[0]; protected set => Bounds[0] = value; }
        /// <summary> The maximum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Vector3 MaxCorner { get => Bounds[1]; protected set => Bounds[1] = value; }
        /// <summary> Whether the normals of the <see cref="AxisAlignedBox"/> are pointing inwards or outwards </summary>
        public bool InwardNormals { get; }
        /// <summary> The surface area of the <see cref="AxisAlignedBox"/> </summary>
        public override float SurfaceArea => (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z) * 2f;

        /// <summary> Create an <see cref="AxisAlignedBox"/> </summary>
        /// <param name="corner1">The first corner of the <see cref="AxisAlignedBox"/></param>
        /// <param name="corner2">The corner on the opposite side of the <see cref="AxisAlignedBox"/></param>
        /// <param name="inwardNormals">Whether the normals of the <see cref="AxisAlignedBox"/> are pointing inwards or outwards</param>
        public AxisAlignedBox(Vector3 corner1, Vector3 corner2, bool inwardNormals = false) : base(corner1 + (corner2 - corner1) * 0.5f) {
            Vector3 minCorner = Vector3.ComponentMin(corner1, corner2);
            Vector3 maxCorner = Vector3.ComponentMax(corner1, corner2);
            Bounds = new Vector3[] { minCorner, maxCorner };
            InwardNormals = inwardNormals;
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position of the point on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="AxisAlignedBox"/> at a specified <paramref name="surfacePosition"/> </summary>
        /// <param name="surfacePosition">The surface position to get the normal for</param>
        /// <returns>The normal at the specified <paramref name="surfacePosition"/></returns>
        public override Vector3 GetNormal(Vector3 surfacePosition) {
            Vector3 direction = Vector3.Divide(surfacePosition - Center, Size);
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
        public override float? Intersect(Ray ray) {
            float tmin = (Bounds[ray.Sign.X].X - ray.Origin.Position.X) * ray.DirectionInverted.X;
            float tmax = (Bounds[1 - ray.Sign.X].X - ray.Origin.Position.X) * ray.DirectionInverted.X;

            float tymin = (Bounds[ray.Sign.Y].Y - ray.Origin.Position.Y) * ray.DirectionInverted.Y;
            float tymax = (Bounds[1 - ray.Sign.Y].Y - ray.Origin.Position.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return null;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign.Z].Z - ray.Origin.Position.Z) * ray.DirectionInverted.Z;
            float tzmax = (Bounds[1 - ray.Sign.Z].Z - ray.Origin.Position.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return null;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            if (ray.WithinBounds(tmin)) {
                return tmin;
            } else if (ray.WithinBounds(tmax)) {
                return tmax;
            } else {
                return null;
            }
        }
    }
}
