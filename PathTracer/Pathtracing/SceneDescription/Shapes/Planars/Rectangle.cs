using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A quad primitive </summary>
    public class Rectangle : PlanarShape {

        public override float SurfaceArea => throw new NotImplementedException();

        public override AxisAlignedBox BoundingBox => throw new NotImplementedException();

        public override Vector3 SurfaceNormal(Vector3 surfacePoint) {
            throw new NotImplementedException();
        }

        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        public override float? IntersectDistance(IRay ray) {
            throw new NotImplementedException();
        }
    }
}
