﻿using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Geometry.Directions {
    /// <summary> A 3-dimensional direction vector </summary>
    public interface IDirection3 : IDirection<Vector3>, IEquatable<IDirection3> {
        /// <summary> The default right direction when no rotation is applied </summary>
        static readonly Normal3 DefaultRight = Normal3.UnitX;
        /// <summary> The default left direction when no rotation is applied </summary>
        static readonly Normal3 DefaultLeft = -DefaultRight;
        /// <summary> The default up vector when no rotation is applied </summary>
        static readonly Normal3 DefaultUp = Normal3.UnitY;
        /// <summary> The default down vector when no rotation is applied </summary>
        static readonly Normal3 DefaultDown = -DefaultUp;
        /// <summary> The default front vector when no rotation is applied </summary>
        static readonly Normal3 DefaultFront = Normal3.UnitZ;
        /// <summary> The default back vector when no rotation is applied </summary>
        static readonly Normal3 DefaultBack = -DefaultFront;
        /// <summary> The direction vector reprsenting no direction </summary>
        static readonly Direction3 Zero = Vector3.Zero;
        /// <summary> The direction vector with 1 in all directions </summary>
        static readonly Direction3 One = Vector3.One;

        /// <summary> The X-coordinate of the <see cref="IDirection3"/> </summary>
        Direction1 X => Vector.X;
        /// <summary> The Y-coordinate of the <see cref="IDirection3"/> </summary>
        Direction1 Y => Vector.Y;
        /// <summary> The Z-coordinate of the <see cref="IDirection3"/> </summary>
        Direction1 Z => Vector.Z;

        public static Direction3 operator +(IDirection3 left, IDirection3 right) => left.Vector + right.Vector;
        public static Direction3 operator -(IDirection3 direction) => -direction.Vector;
        public static Direction3 operator -(IDirection3 left, IDirection3 right) => left.Vector - right.Vector;
        public static Direction3 operator *(IDirection3 direction, Position1 scale) => direction.Vector * scale.Vector;
        public static Direction3 operator *(IDirection3 direction, Position3 scale) => direction.Vector * scale.Vector;
        public static Direction3 operator /(IDirection3 direction, Position1 scale) => direction.Vector / scale.Vector;
        public static Direction3 operator /(Position1 scale, IDirection3 direction) => scale.Vector / direction.Vector;
        public static Direction3 operator /(IDirection3 direction, Position3 scale) => direction.Vector / scale.Vector;

        public static Vector1 Dot(IDirection3 left, IDirection3 right) => Vector3.Dot(left.Vector, right.Vector);
        public static bool InClosedHemisphere(IDirection3 left, IDirection3 right) => Dot(left, right) >= 0;
        public static bool InOpenHemisphere(IDirection3 left, IDirection3 right) => Dot(left, right) > 0;
        public static bool Opposing(IDirection3 left, IDirection3 right) => Dot(left, right) < 0;
        public static Direction3 Abs(IDirection3 direction) => Vector3.Abs(direction.Vector);

        bool Equals(object? obj) => obj is IDirection3 direction && Equals(direction);
        bool IEquatable<IDirection3>.Equals(IDirection3? other) => Vector.Equals(other?.Vector);
        int GetHashCode() => Vector.GetHashCode();

        bool IDirection<Vector3>.SimilarAs(IDirection<Vector3> other) => SimilarAs(other);
        bool Similar(IDirection3 other) => InClosedHemisphere(this, other);
        bool Opposing(IDirection3 other) => Opposing(this, other);
        Normal3 Normalized() => new(Vector);
    }
}
