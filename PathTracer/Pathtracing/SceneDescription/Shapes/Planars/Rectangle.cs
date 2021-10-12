using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A quad primitive </summary>
    public class Rectangle : PlanarShape {
        /// <summary> The position of the <see cref="Rectangle"/> </summary>
        public Vector3 Position { get; set; }
        /// <summary> The size of the <see cref="Rectangle"/> </summary>
        public Vector2 Size { get; set; }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/> </summary>
        public Quaternion Rotation { get; set; }

        /// <summary> The normal vector of the <see cref="Rectangle"/> </summary>
        public Vector3 Normal => Rotation * Camera.DefaultFront;
        /// <summary> The vector going right along the width of the <see cref="Rectangle"/> </summary>
        public Vector3 Right => Rotation * Camera.DefaultRight;
        /// <summary> The vector going up along the height of the <see cref="Rectangle"/> </summary>
        public Vector3 Up => Rotation * Camera.DefaultUp;

        /// <summary> The vector that goes from the left of the <see cref="Rectangle"/> to the right </summary>
        public Vector3 LeftToRight => Right * Width;
        /// <summary> The vector that goes from the bottom of the <see cref="Rectangle"/> to the top </summary>
        public Vector3 BottomToTop => Up * Height;

        /// <summary> The center of the <see cref="Rectangle"/> </summary>
        public Vector3 Center => Position;
        /// <summary> The bottom left corner of the <see cref="Rectangle"/> </summary>
        public Vector3 BottomLeft => Center - LeftToRight * 0.5f - BottomToTop * 0.5f;
        /// <summary> The bottom right corner of the <see cref="Rectangle"/> </summary>
        public Vector3 BottomRight => Center + LeftToRight * 0.5f - BottomToTop * 0.5f;
        /// <summary> The top left corner of the <see cref="Rectangle"/> </summary>
        public Vector3 TopLeft => Center - LeftToRight * 0.5f + BottomToTop * 0.5f;
        /// <summary> The top right corner of the <see cref="Rectangle"/> </summary>
        public Vector3 TopRight => Center + LeftToRight * 0.5f + BottomToTop * 0.5f;
        
        /// <summary> The width of the <see cref="Rectangle"/> </summary>
        public float Width => Size.X;
        /// <summary> The height of the <see cref="Rectangle"/> </summary>
        public float Height => Size.Y;
        /// <summary> The aspect ratio of the <see cref="Rectangle"/> </summary>
        public float AspectRatio => Width / Height;

        /// <summary> The surface area of the <see cref="Rectangle"/> </summary>
        public override float SurfaceArea => Width * Height;
        /// <summary> The <see cref="Plane"/> in which the <see cref="Rectangle"/> lies </summary>
        public override Plane PlaneOfExistence => new(Position, Normal);
        /// <summary> The bounding box of the <see cref="Rectangle"/> </summary>
        public override AxisAlignedBox BoundingBox => new(BottomLeft, BottomRight, TopLeft, TopRight);

        /// <summary> Create a <see cref="Rectangle"/> </summary>
        /// <param name="position">The (center) position of the <see cref="Rectangle"/></param>
        /// <param name="size">The size of the <see cref="Rectangle"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/></param>
        public Rectangle(Vector3 position, Vector2 size, Quaternion rotation) {
            Position = position;
            Size = size;
            Rotation = rotation;
        }

        /// <summary> Create a <see cref="Rectangle"/> </summary>
        /// <param name="position">The (center) position of the <see cref="Rectangle"/></param>
        /// <param name="width">The width of the <see cref="Rectangle"/></param>
        /// <param name="height">The height of the <see cref="Rectangle"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/></param>
        public Rectangle(Vector3 position, float width, float height, Quaternion rotation) {
            Position = position;
            Size = new Vector2(width, height);
            Rotation = rotation;
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Rectangle"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Rectangle"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Rectangle"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Rectangle"/></returns>
        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Rectangle"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Rectangle"/></returns>
        public override Vector3 SurfaceNormal(Vector3 surfacePoint) {
            return Normal;
        }

        /// <summary> Intersect the <see cref="Rectangle"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Rectangle"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> intersects the <see cref="Rectangle"/></returns>
        public override float? IntersectDistance(IRay ray) {
            float? planeDistance = PlaneOfExistence.IntersectDistance(ray);
            if (!planeDistance.HasValue) {
                return null;
            } else {
                Vector3 planeIntersection = ray.Origin + ray.Direction * planeDistance.Value;
                Vector3 relativeIntersection = planeIntersection - Position;
                float u = Vector3.Dot(LeftToRight, relativeIntersection) / LeftToRight.LengthSquared;
                float v = Vector3.Dot(BottomToTop, relativeIntersection) / BottomToTop.LengthSquared;
                if (0f <= u && u <= 1f && 0f <= v && v <= 1f) {
                    return planeDistance.Value;
                } else {
                    return null;
                }
            }
        }
    }
}
