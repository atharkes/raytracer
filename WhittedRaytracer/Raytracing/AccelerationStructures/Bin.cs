using OpenTK;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures {
    /// <summary> A bin for bvh building </summary>
    class Bin {
        /// <summary> The minimum bound of the AABB of this bin </summary>
        public Vector3 MinBound { get; private set; } = Utils.MaxVector;
        /// <summary> The maximum bound of the AABB of this bin </summary>
        public Vector3 MaxBound { get; private set; } = Utils.MinVector;

        /// <summary> The bounds of the AABB of this bin </summary>
        public Vector3[] Bounds => new Vector3[] { MinBound, MaxBound };
        /// <summary> The size of the AABB of this bin </summary>
        public Vector3 Size => MaxBound - MinBound;
        /// <summary> The surface area of the AABB of this bin </summary>
        public float SurfaceArea => 2 * (Size.X * Size.Y + Size.Y * Size.Z + Size.X * Size.Z);
        /// <summary> The primitives in this bin </summary>
        public ICollection<Primitive> Primitives => primitives;

        readonly List<Primitive> primitives;
        
        /// <summary> Create a new bin with no primitives </summary>
        public Bin() {
            primitives = new List<Primitive>();
        }

        /// <summary> Create a bin with primitives </summary>
        /// <param name="primitives">The primitive to add to the bin</param>
        public Bin(List<Primitive> primitives) {
            this.primitives = primitives;
            foreach (Primitive primitive in primitives) {
                (Vector3 primitiveMin, Vector3 primitiveMax) = primitive.GetBounds();
                MinBound = Vector3.ComponentMin(primitiveMin, MinBound);
                MaxBound = Vector3.ComponentMax(primitiveMax, MaxBound);
            }
        }

        /// <summary> Add a primitive to the bin </summary>
        /// <param name="primitive">The primitive to add to the bin</param>
        public void Add(Primitive primitive) {
            primitives.Add(primitive);
            (Vector3 min, Vector3 max) = primitive.GetBounds();
            MinBound = Vector3.ComponentMin(MinBound, min);
            MaxBound = Vector3.ComponentMax(MaxBound, max);
        }
    }
}
