using OpenTK.Mathematics;
using System;
using System.Diagnostics;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    public class AxisAlignedBox : Primitive {
        /// <summary> The bounds of the AABB </summary>
        public override Vector3[] Bounds { get; }
        /// <summary> The minimum bound of the AABB </summary>
        public Vector3 MinCorner { get => Bounds[0]; protected set => Bounds[0] = value; }
        /// <summary> The  </summary>
        public Vector3 MaxCorner { get => Bounds[1]; protected set => Bounds[1] = value; }

        public AxisAlignedBox(Vector3 minCorner, Vector3 maxCorner) : base(minCorner + (maxCorner - minCorner) * 0.5f) {
            Debug.Assert(minCorner == Vector3.ComponentMin(minCorner, maxCorner));
            Debug.Assert(maxCorner == Vector3.ComponentMax(minCorner, maxCorner));
            Bounds = new Vector3[] { minCorner, maxCorner };
        }

        public override Vector3 GetSurfacePoint(Random random) {
            throw new NotImplementedException();
        }

        public override Vector3 GetNormal(Vector3 surfacePosition) {
            Vector3 direction = surfacePosition - Center;
            if (direction.X > direction.Y && direction.X > direction.Z) {
                return Vector3.UnitX;
            } else if (direction.Y > direction.Z) {
                return Vector3.UnitY;
            } else {
                return Vector3.UnitZ;
            }
        }

        public override bool IntersectBool(Ray ray) {
            return Intersect(ray) != null;
        }

        public override Intersection? Intersect(Ray ray) {
            float tmin = (Bounds[ray.Sign.X].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmax = (Bounds[1 - ray.Sign.X].X - ray.Origin.X) * ray.DirectionInverted.X;

            float tymin = (Bounds[ray.Sign.Y].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymax = (Bounds[1 - ray.Sign.Y].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return null;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign.Z].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmax = (Bounds[1 - ray.Sign.Z].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return null;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            if (ray.WithinBounds(tmin)) {
                return new Intersection(ray, tmin, this);
            } else if (ray.WithinBounds(tmax)) {
                return new Intersection(ray, tmax, this);
            } else {
                return null;
            }
        }
    }
}
