using OpenTK;
using WhittedRaytracer.Raytracing.SceneObjects.CameraParts;

namespace WhittedRaytracer.Raytracing {
    /// <summary> A ray sent from the camera into the scene </summary>
    class CameraRay : Ray {
        /// <summary> The cavity from which this ray originates </summary>
        public readonly Cavity Cavity;
        /// <summary> How many times a BVH node is intersected </summary>
        public int BVHTraversals { get; set; } = 0;

        /// <summary> Create a new camera ray </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="cavity">The cavity from which this ray originates</param>
        public CameraRay(Vector3 origin, Vector3 direction, Cavity cavity) : base(origin, direction, float.MaxValue, 0) {
            Cavity = cavity;
        }
    }
}
