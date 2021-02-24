using OpenTK.Mathematics;
using PathTracer.Raytracing.SceneObjects;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Raytracing.AccelerationStructures.BVH {
    /// <summary> An Axis-Aligned Bounding Box </summary>
    public class AABB : IAABB {
        /// <summary> The primitives in the AABB </summary>
        public List<IAABB> Primitives { get; protected set; }
        /// <summary> The bounds of the AABB </summary>
        public Vector3[] Bounds { get; } = new Vector3[] { Utils.MaxVector, Utils.MinVector };
        /// <summary> The minimum bound of the AABB </summary>
        public Vector3 MinBound { get => Bounds[0]; protected set => Bounds[0] = value; }
        /// <summary> The maximum bound of the AABB </summary>
        public Vector3 MaxBound { get => Bounds[1]; protected set => Bounds[1] = value; }
        /// <summary> The size of the AABB </summary>
        public Vector3 Size => MaxBound - MinBound;
        /// <summary> The center of the AABB </summary>
        public Vector3 Center => MinBound + 0.5f * Size;
        /// <summary> The surface area of the AABB </summary>
        public float SurfaceArea => 2 * (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z);
        /// <summary> The cost of having this AABB as a BVHNode </summary>
        public float SurfaceAreaHeuristic => BVHTree.TraversalCost + BVHTree.IntersectionCost * Primitives.Count * SurfaceArea;

        /// <summary> Create a new AABB with no primitives </summary>
        public AABB() {
            Primitives = new List<IAABB>();
        }

        /// <summary> Create a new AABB from two boundable objects </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        public AABB(IAABB left, IAABB right) {
            Primitives = new List<IAABB>() { left, right };
            (MinBound, MaxBound) = CalculateBounds(Primitives);
        }

        /// <summary> Create a new AABB with primitives </summary>
        /// <param name="primitives">The primitives to add to the AABB</param>
        public AABB(IEnumerable<IAABB> primitives) {
            Primitives = primitives.ToList();
            (MinBound, MaxBound) = CalculateBounds(Primitives);
        }

        /// <summary> Add the primitives of another AABB to this AABB </summary>
        /// <param name="other">THe other AABB to add to this one</param>
        /// <returns>This AABB with the added primitives</returns>
        public AABB Add(AABB other) {
            foreach (IAABB primitive in other.Primitives) {
                Add(primitive);
            }
            return this;
        }

        /// <summary> Add a primitive to the AABB </summary>
        /// <param name="primitive">The primitive to add to the AABB</param>
        public void Add(IAABB primitive) {
            Primitives.Add(primitive);
            Vector3[] bounds = primitive.Bounds;
            MinBound = Vector3.ComponentMin(MinBound, bounds[0]);
            MaxBound = Vector3.ComponentMax(MaxBound, bounds[1]);
        }

        /// <summary> Add a list of primitives to the AABB </summary>
        /// <param name="primitives">The primitives to add to the AABB</param>
        public void AddRange(IEnumerable<IAABB> primitives) {
            Primitives.AddRange(primitives);
            foreach (Primitive primitive in primitives) {
                Vector3[] bounds = primitive.Bounds;
                MinBound = Vector3.ComponentMin(MinBound, bounds[0]);
                MaxBound = Vector3.ComponentMax(MaxBound, bounds[1]);
            }
        }

        /// <summary> Remove a primitive from the AABB </summary>
        /// <param name="primitive">The primitive to be removed</param>
        public void Remove(IAABB primitive) {
            Primitives.Remove(primitive);
            (MinBound, MaxBound) = CalculateBounds(Primitives);
        }

        /// <summary> Intersect the AABB (Amy Williams's An Efficient and Robust Ray–Box Intersection Algorithm) </summary>
        /// <param name="ray">The ray to calculate intersection for</param>
        /// <returns>Whether the ray intersects the AABB</returns>
        public bool IntersectBool(Ray ray) {
            float tmin = (Bounds[ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmax = (Bounds[1 - ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;

            float tymin = (Bounds[ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymax = (Bounds[1 - ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return false;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmax = (Bounds[1 - ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return false;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            return tmin > 0 || tmax > 0;
        }

        /// <summary> Intersect the primitives of this AABB </summary>
        /// <param name="ray">The ray to intersect the primitives with</param>
        /// <returns>The intersection with the primitive if there is any</returns>
        public Intersection? IntersectPrimitives(Ray ray) {
            float intersectionDistance = ray.Length;
            Intersection? closestIntersection = null;
            foreach (Primitive primitive in Primitives) {
                Intersection? intersection = primitive.Intersect(ray);
                if (intersection != null && 0 < intersection.Distance && intersection.Distance < intersectionDistance) {
                    closestIntersection = intersection;
                    intersectionDistance = intersection.Distance;
                }
            }
            return closestIntersection;
        }

        /// <summary> Get the distance from a point to the AABB </summary>
        /// <param name="point">To point to get the distance from the AABB to</param>
        /// <returns>The distance from the point to the AABB</returns>
        public float DistanceSquared(Vector3 point) {
            float dx = Math.Max(MinBound.X - point.X, point.X - MaxBound.X);
            float dy = Math.Max(MinBound.Y - point.Y, point.Y - MaxBound.Y);
            float dz = Math.Max(MinBound.Z - point.Z, point.Z - MaxBound.Z);
            return dx * dx + dy * dy + dz * dz;
        }

        /// <summary> Calculate the bounds of a List of Primitives </summary>
        /// <param name="primitives">The primitives the calculate the bounds for</param>
        /// <returns>The bounds of the primitives</returns>
        public static (Vector3 boundMin, Vector3 boundMax) CalculateBounds(ICollection<IAABB> primitives) {
            Vector3 boundMin = Utils.MaxVector;
            Vector3 boundMax = Utils.MinVector;
            foreach (IAABB primitive in primitives) {
                Vector3[] bounds = primitive.Bounds;
                boundMin = Vector3.ComponentMin(bounds[0], boundMin);
                boundMax = Vector3.ComponentMax(bounds[1], boundMax);
            }
            return (boundMin, boundMax);
        }
    }
}
