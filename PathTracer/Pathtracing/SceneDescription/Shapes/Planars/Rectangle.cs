using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A quad primitive </summary>
    public class Rectangle : PlanarShape {

        public override Plane PlaneOfExistence { get; }

        public Vector3 Middle { get; }
        public Vector3 TopLeft { get; } 
        public Vector3 TopRight { get; }
        public Vector3 BottomLeft { get; }
        public Vector3 BottomRight { get; }

        /// <summary> The width of the <see cref="Rectangle"/> </summary>
        public float Width { get; }
        /// <summary> The height of the <see cref="Rectangle"/> </summary>
        public float Height { get; }
        /// <summary> The aspect ratio of the <see cref="Rectangle"/> </summary>
        public float AspectRatio => Width / Height;
        /// <summary> The surface area of the <see cref="Rectangle"/> </summary>
        public override float SurfaceArea => Width * Height;

        /// <summary> The bounding box of the <see cref="Rectangle"/> </summary>
        public override AxisAlignedBox BoundingBox => throw new NotImplementedException();

        public Rectangle(Plane plane, Vector2 topLeft, Vector2 top, float height) {
            PlaneOfExistence = plane;
        }

        public Rectangle(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft) {
            PlaneOfExistence = new Plane();
        }

        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        public override Vector3 SurfaceNormal(Vector3 surfacePoint) {
            return PlaneOfExistence.Normal;
        }

        public override float? IntersectDistance(IRay ray) {
            throw new NotImplementedException();
        }
    }
}
