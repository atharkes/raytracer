using OpenTK;

namespace WhittedRaytracer.Raytracing.AccelerationStructures.SBVH {
    /// <summary> A plane to split a primitive </summary>
    public class SplitPlane {
        /// <summary> The normal of the clipping plane on the left </summary>
        public Vector3 Normal { get; }
        /// <summary> The position of the clipping plane on the left </summary>
        public Vector3 Position { get; }

        /// <summary> Create a split plane </summary>
        /// <param name="normal">The normal of the split plane</param>
        /// <param name="position">The position of the split plane</param>
        public SplitPlane(Vector3 normal, Vector3 position) {
            Normal = normal;
            Position = position;
        }
    }
}
