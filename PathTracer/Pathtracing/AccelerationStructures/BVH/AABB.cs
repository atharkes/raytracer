using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.AccelerationStructures.BVH {
    /// <summary> An Axis-Aligned Bounding Box </summary>
    public class AABB : IBoundable {
        /// <summary> The <see cref="IBoundable"/> objects in the <see cref="AABB"/> </summary>
        public List<IBoundable> SceneObjects { get; protected set; }
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
        public float SurfaceAreaHeuristic => BVHTree.TraversalCost + BVHTree.IntersectionCost * SceneObjects.Count * SurfaceArea;

        /// <summary> Create a new AABB with no primitives </summary>
        public AABB() {
            SceneObjects = new List<IBoundable>();
        }

        /// <summary> Create a new AABB from two boundable objects </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        public AABB(IBoundable left, IBoundable right) {
            SceneObjects = new List<IBoundable>() { left, right };
            (MinBound, MaxBound) = CalculateBounds(SceneObjects);
        }

        /// <summary> Create a new AABB with primitives </summary>
        /// <param name="primitives">The primitives to add to the AABB</param>
        public AABB(IEnumerable<IBoundable> primitives) {
            SceneObjects = primitives.ToList();
            (MinBound, MaxBound) = CalculateBounds(SceneObjects);
        }

        /// <summary> Add the primitives of another AABB to this AABB </summary>
        /// <param name="other">THe other AABB to add to this one</param>
        /// <returns>This AABB with the added primitives</returns>
        public AABB Add(AABB other) {
            foreach (IBoundable primitive in other.SceneObjects) {
                Add(primitive);
            }
            return this;
        }

        /// <summary> Add a primitive to the AABB </summary>
        /// <param name="primitive">The primitive to add to the AABB</param>
        public void Add(IBoundable primitive) {
            SceneObjects.Add(primitive);
            Vector3[] bounds = primitive.Bounds;
            MinBound = Vector3.ComponentMin(MinBound, bounds[0]);
            MaxBound = Vector3.ComponentMax(MaxBound, bounds[1]);
        }

        /// <summary> Add a list of primitives to the AABB </summary>
        /// <param name="primitives">The primitives to add to the AABB</param>
        public void AddRange(IEnumerable<IBoundable> primitives) {
            SceneObjects.AddRange(primitives);
            foreach (Primitive primitive in primitives) {
                Vector3[] bounds = primitive.Bounds;
                MinBound = Vector3.ComponentMin(MinBound, bounds[0]);
                MaxBound = Vector3.ComponentMax(MaxBound, bounds[1]);
            }
        }

        /// <summary> Remove a primitive from the AABB </summary>
        /// <param name="primitive">The primitive to be removed</param>
        public void Remove(IBoundable primitive) {
            SceneObjects.Remove(primitive);
            (MinBound, MaxBound) = CalculateBounds(SceneObjects);
        }

        /// <summary> Intersect the AABB (Amy Williams's An Efficient and Robust Ray–Box Intersection Algorithm) </summary>
        /// <param name="ray">The ray to calculate intersection for</param>
        /// <returns>Whether the ray intersects the AABB</returns>
        public bool IntersectBool(Ray ray) {
            float tmin = (Bounds[ray.Sign.X].X - ray.Origin.Position.X) * ray.DirectionInverted.X;
            float tmax = (Bounds[1 - ray.Sign.X].X - ray.Origin.Position.X) * ray.DirectionInverted.X;

            float tymin = (Bounds[ray.Sign.Y].Y - ray.Origin.Position.Y) * ray.DirectionInverted.Y;
            float tymax = (Bounds[1 - ray.Sign.Y].Y - ray.Origin.Position.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return false;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign.Z].Z - ray.Origin.Position.Z) * ray.DirectionInverted.Z;
            float tzmax = (Bounds[1 - ray.Sign.Z].Z - ray.Origin.Position.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return false;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            return tmin < ray.Length && tmax > 0;
        }

        /// <summary> Intersect the primitives of this AABB </summary>
        /// <param name="ray">The ray to intersect the primitives with</param>
        /// <returns>The intersection with the primitive if there is any</returns>
        public (Primitive primitive, float distance)? IntersectPrimitives(Ray ray) {
            float closestDistance = ray.Length;
            Primitive? closestPrimitive = null;
            foreach (Primitive primitive in SceneObjects) {
                float? current = primitive.Intersect(ray);
                if (current.HasValue && current.Value < closestDistance) {
                    Debug.Assert(0f < current.Value && current.Value < ray.Length);
                    closestDistance = current.Value;
                    closestPrimitive = primitive;
                }
            }
            if (closestPrimitive == null) {
                return null;
            } else {
                return (closestPrimitive, closestDistance);
            }
        }

        /// <summary> Calculate the bounds of a List of Primitives </summary>
        /// <param name="primitives">The primitives the calculate the bounds for</param>
        /// <returns>The bounds of the primitives</returns>
        public static (Vector3 boundMin, Vector3 boundMax) CalculateBounds(ICollection<IBoundable> primitives) {
            Vector3 boundMin = Utils.MaxVector;
            Vector3 boundMax = Utils.MinVector;
            foreach (IBoundable primitive in primitives) {
                Vector3[] bounds = primitive.Bounds;
                boundMin = Vector3.ComponentMin(bounds[0], boundMin);
                boundMax = Vector3.ComponentMax(bounds[1], boundMax);
            }
            return (boundMin, boundMax);
        }
    }
}
