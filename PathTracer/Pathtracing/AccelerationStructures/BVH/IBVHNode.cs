namespace PathTracer.Pathtracing.AccelerationStructures.BVH {
    /// <summary> An interface for a node in a BVH </summary>
    public interface IBVHNode : IAccelerationStructure {
        /// <summary> The AABB of this BVH node </summary>
        AABB AABB { get; }
        /// <summary> The left child node if it has one </summary>
        IBVHNode? Left { get; }
        /// <summary> The right child node if it has one </summary>
        IBVHNode? Right { get; }
        /// <summary> Whether this node is a leaf or not </summary>
        bool Leaf { get; }
    }
}
