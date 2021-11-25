using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Directions {
    /// <summary> A 1-dimensional direction vector </summary>
    public interface IDirection1 : IDirection<Vector1>, IComparable<IDirection1>, IEquatable<IDirection1> {
        /// <summary> The default right direction when no rotation is applied </summary>
        static readonly Normal1 DefaultRight = Normal1.One;
        /// <summary> The default left direction when no rotation is applied </summary>
        static readonly Normal1 DefaultLeft = -DefaultRight;
        /// <summary> The direction vector reprsenting no direction </summary>
        static readonly Direction1 Zero = Vector1.Zero;
        /// <summary> The direction vector with 1 in every direction </summary>
        static readonly Direction1 One = Vector1.One;

        Vector1 X => Vector;

        public static bool operator <=(IDirection1 left, IDirection1 right) => left.CompareTo(right) <= 0;
        public static bool operator >=(IDirection1 left, IDirection1 right) => left.CompareTo(right) >= 0;
        public static bool operator <(IDirection1 left, IDirection1 right) => left.CompareTo(right) < 0;
        public static bool operator >(IDirection1 left, IDirection1 right) => left.CompareTo(right) > 0;

        public static Direction1 operator +(IDirection1 left, IDirection1 right) => left.Vector + right.Vector;
        public static Direction1 operator -(IDirection1 direction) => -direction.Vector;
        public static Direction1 operator -(IDirection1 left, IDirection1 right) => left.Vector - right.Vector;
        public static Direction1 operator *(IDirection1 direction, Position1 scale) => direction.Vector * scale.Vector;
        public static Direction1 operator /(IDirection1 direction, Position1 scale) => direction.Vector / scale.Vector;

        public static Position1 operator *(IDirection1 left, IDirection1 right) => left.Vector * right.Vector;
        public static Position1 operator /(IDirection1 left, IDirection1 right) => left.Vector / right.Vector;

        public static Direction1 Dot(IDirection1 left, IDirection1 right) => new(left.Vector * right.Vector);
        public static Direction1 Abs(IDirection1 direction) => Vector1.Abs(direction.Vector);

        bool Equals(object? obj) => obj is IDirection1 direction && Equals(direction);
        bool IEquatable<IDirection1>.Equals(IDirection1? other) => Vector.Equals(other?.Vector);
        int GetHashCode() => Vector.GetHashCode();
        int IComparable<IDirection1>.CompareTo(IDirection1? other) => Vector.CompareTo(other?.Vector);

        bool IDirection<Vector1>.SimilarAs(IDirection<Vector1> other) => SimilarAs(other);
        bool SimilarAs(IDirection1 other) => Dot(this, other) > Zero;
        Normal1 Normalized() => new(Vector);
    }
}
