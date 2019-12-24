using OpenTK;
using System;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures {
    /// <summary> An Axis-Aligned Bounding Box </summary>
    class AABB {
        /// <summary> The primitives in this AABB </summary>
        public ICollection<Primitive> Primitives => primitives;
        /// <summary> The bounds of the AABB of this bin </summary>
        public Vector3[] Bounds { get; } = new Vector3[] { Utils.MaxVector, Utils.MinVector };

        /// <summary> The minimum bound of the AABB of this bin </summary>
        public Vector3 MinBound { get => Bounds[0]; private set => Bounds[0] = value; }
        /// <summary> The maximum bound of the AABB of this bin </summary>
        public Vector3 MaxBound { get => Bounds[1]; private set => Bounds[1] = value; }
        /// <summary> The size of the AABB of this bin </summary>
        public Vector3 Size => MaxBound - MinBound;
        /// <summary> The surface area of the AABB of this bin </summary>
        public float SurfaceArea => 2 * (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z);

        readonly List<Primitive> primitives;

        /// <summary> Create a new AABB with no primitives </summary>
        public AABB() {
            primitives = new List<Primitive>();
        }

        /// <summary> Create a new AABB with primitives </summary>
        /// <param name="primitives">The primitive to add to the AABB</param>
        public AABB(List<Primitive> primitives) {
            this.primitives = primitives;
            foreach (Primitive primitive in primitives) {
                (Vector3 primitiveMin, Vector3 primitiveMax) = primitive.GetBounds();
                MinBound = Vector3.ComponentMin(primitiveMin, MinBound);
                MaxBound = Vector3.ComponentMax(primitiveMax, MaxBound);
            }
        }

        /// <summary> Add a primitive to the AABB </summary>
        /// <param name="primitive">The primitive to add to the AABB</param>
        public void Add(Primitive primitive) {
            Primitives.Add(primitive);
            (Vector3 min, Vector3 max) = primitive.GetBounds();
            MinBound = Vector3.ComponentMin(MinBound, min);
            MaxBound = Vector3.ComponentMax(MaxBound, max);
        }

        /// <summary> Add a list of primitives to the AABB </summary>
        /// <param name="primitives">The primitives to add to the AABB</param>
        public void AddRange(IEnumerable<Primitive> primitives) {
            this.primitives.AddRange(primitives);
            foreach (Primitive primitive in primitives) {
                (Vector3 min, Vector3 max) = primitive.GetBounds();
                MinBound = Vector3.ComponentMin(MinBound, min);
                MaxBound = Vector3.ComponentMax(MaxBound, max);
            }
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

        /// <summary> Add (the primitives of) another AABB to this AABB </summary>
        /// <param name="other">THe other AABB to add to this one</param>
        /// <returns>This AABB with the added primitives</returns>
        public AABB Add(AABB other) {
            foreach (Primitive primitive in other.Primitives) {
                Add(primitive);
            }
            return this;
        }

        /// <summary> Calculate the bounds of a List of Primitives </summary>
        /// <param name="primitives">The primitives the calculate the bounds for</param>
        /// <returns>The bounds of the primitives</returns>
        public static (Vector3 boundMin, Vector3 boundMax) CalculateBounds(ICollection<Primitive> primitives) {
            Vector3 boundMin = Utils.MaxVector;
            Vector3 boundMax = Utils.MinVector;
            foreach (Primitive primitive in primitives) {
                (Vector3 primitiveMin, Vector3 primitiveMax) = primitive.GetBounds();
                boundMin = Vector3.ComponentMin(primitiveMin, boundMin);
                boundMax = Vector3.ComponentMax(primitiveMax, boundMax);
            }
            return (boundMin, boundMax);
        }
    }
}
