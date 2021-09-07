using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;

namespace PathTracer.Pathtracing.Paths {
    /// <summary> A ray sent from the camera into the scene </summary>
    public class CameraRay : Ray {
        /// <summary> The cavity from which this ray originates </summary>
        public Cavity Cavity { get; }
        /// <summary> How many times a BVH node is intersected </summary>
        public int BVHTraversals { get; set; } = 0;
        /// <summary> Whether the camera ray interacted with something </summary>
        public bool Intersection => Length != float.PositiveInfinity;

        /// <summary> Create a new camera ray </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="cavity">The cavity from which this ray originates</param>
        public CameraRay(SurfacePoint origin, Vector3 direction, Cavity cavity, float length = float.PositiveInfinity) : base(origin, direction, length, 0) {
            Cavity = cavity;
        }
    }
}
