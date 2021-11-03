using OpenTK.Mathematics;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A quad primitive </summary>
    public struct Rectangle : IPlanarShape {
        /// <summary> The position of the <see cref="Rectangle"/> </summary>
        public Position3 Position { get; set; }
        /// <summary> The size of the <see cref="Rectangle"/> </summary>
        public Position2 Size { get; set; }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/> </summary>
        public Quaternion Rotation { get; set; }

        /// <summary> The normal vector of the <see cref="Rectangle"/> </summary>
        public Normal3 Normal => Rotation * IDirection3.DefaultFront;
        /// <summary> The vector going right along the width of the <see cref="Rectangle"/> </summary>
        public Normal3 Right => Rotation * IDirection3.DefaultRight;
        /// <summary> The vector going up along the height of the <see cref="Rectangle"/> </summary>
        public Normal3 Up => Rotation * IDirection3.DefaultUp;

        /// <summary> The vector that goes from the left of the <see cref="Rectangle"/> to the right </summary>
        public Direction3 LeftToRight => Right * Width;
        /// <summary> The vector that goes from the right of the <see cref="Rectangle"/> to the left </summary>
        public Direction3 RightToLeft => -Right * Width;
        /// <summary> The vector that goes from the bottom of the <see cref="Rectangle"/> to the top </summary>
        public Direction3 BottomToTop => Up * Height;
        /// <summary> The vector that goes from the top of the <see cref="Rectangle"/> to the bottom </summary>
        public Direction3 TopToBottom => -Up * Height;

        /// <summary> The center of the <see cref="Rectangle"/> </summary>
        public Position3 Center { get => Position; set => Position = value; }
        /// <summary> The bottom left corner of the <see cref="Rectangle"/> </summary>
        public Position3 BottomLeft => Center - LeftToRight * 0.5f - BottomToTop * 0.5f;
        /// <summary> The bottom right corner of the <see cref="Rectangle"/> </summary>
        public Position3 BottomRight => Center + LeftToRight * 0.5f - BottomToTop * 0.5f;
        /// <summary> The top left corner of the <see cref="Rectangle"/> </summary>
        public Position3 TopLeft => Center - LeftToRight * 0.5f + BottomToTop * 0.5f;
        /// <summary> The top right corner of the <see cref="Rectangle"/> </summary>
        public Position3 TopRight => Center + LeftToRight * 0.5f + BottomToTop * 0.5f;
        
        /// <summary> The width of the <see cref="Rectangle"/> </summary>
        public Position1 Width => Size.X;
        /// <summary> The height of the <see cref="Rectangle"/> </summary>
        public Position1 Height => Size.Y;
        /// <summary> The aspect ratio of the <see cref="Rectangle"/> </summary>
        public float AspectRatio => Width / Height;

        /// <summary> The surface area of the <see cref="Rectangle"/> </summary>
        public float SurfaceArea => Width * Height;
        /// <summary> The <see cref="Plane"/> in which the <see cref="Rectangle"/> lies </summary>
        public Plane PlaneOfExistence => new(Position, Normal);
        /// <summary> The bounding box of the <see cref="Rectangle"/> </summary>
        public AxisAlignedBox BoundingBox => new(BottomLeft, BottomRight, TopLeft, TopRight);

        /// <summary> Create a <see cref="Rectangle"/> </summary>
        /// <param name="position">The (center) position of the <see cref="Rectangle"/></param>
        /// <param name="size">The size of the <see cref="Rectangle"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/></param>
        public Rectangle(Position3 position, Position2 size, Quaternion rotation) {
            Position = position;
            Size = size;
            Rotation = rotation;
        }

        /// <summary> Create a <see cref="Rectangle"/> </summary>
        /// <param name="position">The (center) position of the <see cref="Rectangle"/></param>
        /// <param name="width">The width of the <see cref="Rectangle"/></param>
        /// <param name="height">The height of the <see cref="Rectangle"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="Rectangle"/></param>
        public Rectangle(Position3 position, Position1 width, Position1 height, Quaternion rotation) {
            Position = position;
            Size = new Position2(width, height);
            Rotation = rotation;
        }

        public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);
        public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Position.GetHashCode(), Size.GetHashCode(), Rotation.GetHashCode());
        public override bool Equals(object? obj) => obj is Rectangle rectangle && Equals(rectangle);
        public bool Equals(Rectangle other) => Position.Equals(other.Position) && Size.Equals(other.Size) && Rotation.Equals(other.Rotation);

        public Position3 SurfacePosition(Position2 uvPosition) => TopLeft + LeftToRight * uvPosition.X + TopToBottom * uvPosition.Y;

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Rectangle"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Rectangle"/></returns>
        public Position3 SurfacePosition(Random random) => throw new NotImplementedException();

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        public Position2 UVPosition(Position3 position) {
            Direction3 relativePosition = position - TopLeft;
            return new Position2(IDirection3.Dot(LeftToRight, relativePosition), IDirection3.Dot(TopToBottom, relativePosition));
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Rectangle"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Rectangle"/></returns>
        public bool OnSurface(Position3 position, float epsilon = 0.001F) => throw new NotImplementedException();

        /// <summary> Get the normal of the <see cref="Rectangle"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Rectangle"/></returns>
        public Normal3 SurfaceNormal(Position3 surfacePoint) => Normal;

        /// <summary> Intersect the <see cref="Rectangle"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Rectangle"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> intersects the <see cref="Rectangle"/></returns>
        public Position1? IntersectDistance(IRay ray) {
            Position1? planeDistance = PlaneOfExistence.IntersectDistance(ray);
            if (!planeDistance.HasValue) {
                return null;
            } else {
                Position3 planeIntersection = ray.Origin + ray.Direction * planeDistance.Value;
                IDirection3 relativeIntersection = planeIntersection - Position;
                Position1 u = (Position1)IDirection3.Dot(LeftToRight, relativeIntersection) / LeftToRight.LengthSquared;
                Position1 v = (Position1)IDirection3.Dot(BottomToTop, relativeIntersection) / BottomToTop.LengthSquared;
                if (0f <= u && u <= 1f && 0f <= v && v <= 1f) {
                    return planeDistance.Value;
                } else {
                    return null;
                }
            }
        }
    }
}
