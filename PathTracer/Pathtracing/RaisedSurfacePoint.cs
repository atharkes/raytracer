using OpenTK.Mathematics;

namespace PathTracer.Pathtracing {
    /// <summary> A <see cref="RaisedSurfacePoint"/> to prevent self intersection when casting <see cref="Ray"/>s </summary>
    public class RaisedSurfacePoint {
        /// <summary> The position of the <see cref="RaisedSurfacePoint"/> </summary>
        public Vector3 Position { get; }
        /// <summary> The original <see cref="Pathtracing.SurfacePoint"/> </summary>
        public SurfacePoint SurfacePoint { get; }
        
        /// <summary>  </summary>
        /// <param name="position"></param>
        /// <param name="surfacePoint"></param>
        public RaisedSurfacePoint(Vector3 position, SurfacePoint surfacePoint) {
            Position = position;
            SurfacePoint = surfacePoint;
        }
    }
}