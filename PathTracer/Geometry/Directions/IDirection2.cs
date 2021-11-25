using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Directions {
    /// <summary> A 2-dimensional direction vector </summary>
    public interface IDirection2 : IDirection<Vector2>, IEquatable<IDirection2> {
        /// <summary> The default right direction when no rotation is applied </summary>
        static readonly Normal2 DefaultRight = Normal2.UnitX;
        /// <summary> The default left direction when no rotation is applied </summary>
        static readonly Normal2 DefaultLeft = -DefaultRight;
        /// <summary> The default up vector when no rotation is applied </summary>
        static readonly Normal2 DefaultUp = Normal2.UnitY;
        /// <summary> The default down vector when no rotation is applied </summary>
        static readonly Normal2 DefaultDown = -DefaultUp;
        /// <summary> The direction vector reprsenting no direction </summary>
        static readonly Direction2 Zero = Vector2.Zero;
        /// <summary> The direction vector 1 in all directions </summary>
        static readonly Direction2 One = Vector2.One;

        /// <summary> The X-coordinate of the <see cref="IDirection2"/> </summary>
        Direction1 X => Vector.X;
        /// <summary> The Y-coordinate of the <see cref="IDirection2"/> </summary>
        Direction1 Y => Vector.Y;

        public static Direction2 operator +(IDirection2 left, IDirection2 right) => left.Vector + right.Vector;
        public static Direction2 operator -(IDirection2 direction) => -direction.Vector;
        public static Direction2 operator -(IDirection2 left, IDirection2 right) => left.Vector - right.Vector;
        public static Direction2 operator *(IDirection2 direction, Position1 scale) => direction.Vector * scale.Vector;
        public static Direction2 operator *(IDirection2 direction, Position2 scale) => direction.Vector * scale.Vector;
        public static Direction2 operator /(IDirection2 direction, Position1 scale) => direction.Vector / scale.Vector;
        public static Direction2 operator /(Position1 scale, IDirection2 direction) => scale.Vector / direction.Vector;
        public static Direction2 operator /(IDirection2 direction, Position2 scale) => direction.Vector / scale.Vector;

        public static Direction1 Dot(IDirection2 left, IDirection2 right) => Vector2.Dot(left.Vector, right.Vector);
        public static Direction2 Abs(IDirection2 direction) => Vector2.Abs(direction.Vector);

        bool Equals(object? obj) => obj is IDirection2 direction && Equals(direction);
        bool IEquatable<IDirection2>.Equals(IDirection2? other) => Vector.Equals(other?.Vector);
        int GetHashCode() => Vector.GetHashCode();

        bool IDirection<Vector2>.SimilarAs(IDirection<Vector2> other) => SimilarAs(other);
        bool SimilarAs(IDirection2 other) => Dot(this, other) > IDirection1.Zero;
        Normal2 Normalized() => new(Vector);
    }
}
