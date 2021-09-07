namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH {
    /// <summary> An interface for a node in a BVH </summary>
    public interface IBinaryTree {
        /// <summary> The left child node if it has one </summary>
        IBinaryTree? Left { get; }
        /// <summary> The right child node if it has one </summary>
        IBinaryTree? Right { get; }
        /// <summary> Whether this node is a leaf or not </summary>
        bool Leaf { get; }
    }
}
