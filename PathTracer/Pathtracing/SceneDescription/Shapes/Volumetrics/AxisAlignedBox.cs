using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> An axis aligned box primitive </summary>
    public class AxisAlignedBox : VolumetricShape {
        /// <summary> The bounds of the <see cref="AxisAlignedBox"/> </summary>
        public Position3[] Bounds { get; }
        /// <summary> Whether the normals of the <see cref="AxisAlignedBox"/> are pointing inwards or outwards </summary>
        public bool InwardNormals { get; set; } = false;

        /// <summary> The minimum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Position3 MinCorner { get => Bounds[0]; set => Bounds[0] = value; }
        /// <summary> The maximum bound of the <see cref="AxisAlignedBox"/> </summary>
        public Position3 MaxCorner { get => Bounds[1]; set => Bounds[1] = value; }
        /// <summary> The size of the <see cref="AxisAlignedBox"/> </summary>
        public IDirection3 Size => MaxCorner - MinCorner;
        /// <summary> The center of the <see cref="AxisAlignedBox"/> </summary>
        public Position3 Center => MinCorner + Size / 2;

        /// <summary> The volume of the <see cref="AxisAlignedBox"/> </summary>
        public override float Volume => Size.X * Size.Y * Size.Z;
        /// <summary> The surface area of the <see cref="AxisAlignedBox"/> </summary>
        public override float SurfaceArea => (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z) * 2;
        /// <summary> The bounding box of the <see cref="AxisAlignedBox"/> </summary>
        public override AxisAlignedBox BoundingBox => this;

        /// <summary> Create an <see cref="AxisAlignedBox"/> using two corners </summary>
        /// <param name="corner1">The first corner of the <see cref="AxisAlignedBox"/></param>
        /// <param name="corner2">The corner on the opposite side of the <see cref="AxisAlignedBox"/></param>
        public AxisAlignedBox(Position3 corner1, Position3 corner2) {
            Position3 minCorner = Position3.ComponentMin(corner1, corner2);
            Position3 maxCorner = Position3.ComponentMax(corner1, corner2);
            Bounds = new Position3[] { minCorner, maxCorner };
        }

        /// <summary> Create an <see cref="AxisAlignedBox"/> that encompasses <paramref name="positions"/> </summary>
        /// <param name="positions">The positions that the <see cref="AxisAlignedBox"/> should encompass</param>
        public AxisAlignedBox(params Position3[] positions) {
            Position3 minCorner = Position3.PositiveInfinity;
            Position3 maxCorner = Position3.NegativeInfinity;
            foreach (Position3 position in positions) {
                minCorner = Position3.ComponentMin(minCorner, position);
                maxCorner = Position3.ComponentMax(maxCorner, position);
            }
            Bounds = new Position3[] { minCorner, maxCorner };
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position of the point on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="AxisAlignedBox"/></returns>
        public override Position3 SurfacePosition(Random random) {
            throw new NotImplementedException();
        }


        public override Position2 UVPosition(Position3 position) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="AxisAlignedBox"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="AxisAlignedBox"/></returns>
        public override bool OnSurface(Position3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="AxisAlignedBox"/> at a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position to get the normal for</param>
        /// <returns>The normal at the specified <paramref name="position"/></returns>
        public override Normal3 SurfaceNormal(Position3 position) {
            IDirection3 direction = (position - Center) / (Position3.Origin + Size);
            IDirection1 x = IDirection1.Abs(direction.X);
            IDirection1 y = IDirection1.Abs(direction.Y);
            IDirection1 z = IDirection1.Abs(direction.Z);
            Normal3 normal;
            if (x > y && x > z) {
                normal = direction.X > 0f ? Normal3.UnitX : -Normal3.UnitX;
            } else if (y > z) {
                normal = direction.Y > 0f ? Normal3.UnitY : -Normal3.UnitY;
            } else {
                normal = direction.Z > 0f ? Normal3.UnitZ : -Normal3.UnitZ;
            }
            return InwardNormals ? -normal : normal;
        }

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        public override Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);

        /// <summary> Intersect the <see cref="AxisAlignedBox"/> by a <paramref name="ray"/>.
        /// Using Amy Williams's "An Efficient and Robust Ray–Box Intersection" Algorithm </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="AxisAlignedBox"/> with</param>
        /// <returns>Whether and when the <see cref="Ray"/> intersects the <see cref="AxisAlignedBox"/></returns>
        public override IEnumerable<Position1> IntersectDistances(IRay ray) {
            Position1 tmin = (Bounds[ray.Sign.X].X - ray.Origin.X) * ray.InvDirection.X;
            Position1 tmax = (Bounds[1 - ray.Sign.X].X - ray.Origin.X) * ray.InvDirection.X;

            Position1 tymin = (Bounds[ray.Sign.Y].Y - ray.Origin.Y) * ray.InvDirection.Y;
            Position1 tymax = (Bounds[1 - ray.Sign.Y].Y - ray.Origin.Y) * ray.InvDirection.Y;
            if ((tmin > tymax) || (tmax < tymin)) yield break;
            tmin = Position1.ComponentMax(tmin, tymin);
            tmax = Position1.ComponentMin(tmax, tymax);

            Position1 tzmin = (Bounds[ray.Sign.Z].Z - ray.Origin.Z) * ray.InvDirection.Z;
            Position1 tzmax = (Bounds[1 - ray.Sign.Z].Z - ray.Origin.Z) * ray.InvDirection.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) yield break;
            yield return Position1.ComponentMax(tmin, tzmin);
            yield return Position1.ComponentMin(tmax, tzmax);
        }

        /// <summary> Clip the <see cref="AxisAlignedBox"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="AxisAlignedBox"/> with</param>
        /// <returns>A new clipped <see cref="AxisAlignedBox"/> if it's not clipped entirely</returns>
        public override IEnumerable<AxisAlignedBox> Clip(AxisAlignedPlane plane) {
            Position3 minCorner = MinCorner;
            Position3 maxCorner = MaxCorner;

            if (plane.Normal == Normal3.UnitX) {
                minCorner = new(Position1.ComponentMax(MinCorner.X, plane.Position.X), MinCorner.Y, MinCorner.Z);
            } else if (plane.Normal == -Normal3.UnitX) {
                maxCorner = new(Position1.ComponentMin(MinCorner.X, plane.Position.X), MaxCorner.Y, MaxCorner.Z);
            } else if (plane.Normal == Normal3.UnitY) {
                minCorner = new(MinCorner.X, Position1.ComponentMax(MinCorner.Y, plane.Position.Y), MinCorner.Z);
            } else if (plane.Normal == -Normal3.UnitY) {
                maxCorner = new(MaxCorner.X, Position1.ComponentMin(MaxCorner.Y, plane.Position.Y), MaxCorner.Z);
            } else if (plane.Normal == Normal3.UnitZ) {
                minCorner = new(MinCorner.X, MinCorner.Y, Position1.ComponentMax(MinCorner.Z, plane.Position.Z));
            } else if (plane.Normal == -Normal3.UnitZ) {
                maxCorner = new(MaxCorner.X, MaxCorner.Y, Position1.ComponentMin(MaxCorner.Z, plane.Position.Z));
            }
            if (minCorner == MinCorner && maxCorner == MaxCorner) {
                yield return this;
            } else if (minCorner.X < maxCorner.X && minCorner.Y < maxCorner.Y && minCorner.Z < maxCorner.Z) {
                yield return new AxisAlignedBox(minCorner, maxCorner);
            }
        }
    }
}
