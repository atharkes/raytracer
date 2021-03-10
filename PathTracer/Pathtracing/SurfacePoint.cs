using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;

namespace PathTracer.Pathtracing {
    /// <summary> A <see cref="Point3"/> on the surface of a <see cref="SceneObjects.Primitive"/>  </summary>
    public struct SurfacePoint {
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public Primitive Primitive { get; }
    }
}
