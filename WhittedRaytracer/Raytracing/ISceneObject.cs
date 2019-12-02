using OpenTK;

namespace WhittedRaytracer.Raytracing {
    /// <summary> An interface for an object in the 3d scene </summary>
    interface ISceneObject {
        /// <summary> The position of the scene object </summary>
        Vector3 Position { get; set; }
    }
}
