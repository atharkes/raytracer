using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public class SurfaceMaterial : IMaterial {
        public ISurfacePoint? Scatter(IRay ray, IBoundaryPoint boundaryPoint) {
            throw new NotImplementedException();
        }
    }
}