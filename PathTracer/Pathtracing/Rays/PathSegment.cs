﻿using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.Shapes;

namespace PathTracer.Pathtracing.Paths {
    /// <summary> A <see cref="Ray"/> used for tracing through the <see cref="Scene"/> </summary>
    public class PathSegment {
        /// <summary> The maximum recursion depth of traced paths </summary>
        public static int MaxRecursionDepth { get; set; } = 1;

        /// <summary> The origin of the <see cref="Ray"/> </summary>
        public RaisedSurfacePoint Origin { get; }
        /// <summary> The normalized direction of the <see cref="Ray"/> </summary>
        public Vector3 Direction { get; }
        /// <summary> The length the <see cref="Ray"/> is travelling </summary>
        public float Length { get; private set; }
        /// <summary> The destination of the <see cref="Ray"/> </summary>
        public RaisedSurfacePoint? Destination { get; private set; }
        /// <summary> The recursion depth of the traced path so far </summary>
        public int RecursionDepth { get; }

        /// <summary> The inverted direction. Used for quick AABB intersection </summary>
        public Vector3 DirectionInverted { get; }
        /// <summary> Whether the individual components of the inverted direction of the ray is negative </summary>
        public Vector3i Sign { get; }

        /// <summary> Create a <see cref="Ray"/> at a specified <paramref name="origin"/> going in a specified <paramref name="direction"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="direction">The direction of the <see cref="Ray"/></param>
        /// <param name="length">The length the <see cref="Ray"/> will try to travel</param>
        /// <param name="recursionDepth">The recursion depth of the <see cref="Ray"/></param>
        public PathSegment(SurfacePoint origin, Vector3 direction, float length = float.PositiveInfinity, int recursionDepth = 0) {
            Origin = origin.GetRaisedOrigin(direction);
            Direction = direction.Normalized();
            Length = length;
            Destination = null;
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / Direction.X, 1 / Direction.Y, 1 / Direction.Z);
            Sign = new Vector3i(DirectionInverted.X < 0 ? 1 : 0, DirectionInverted.Y < 0 ? 1 : 0, DirectionInverted.Z < 0 ? 1 : 0);
        }

        /// <summary> Create a <see cref="Ray"/> at a specified <paramref name="origin"/> going in a specified <paramref name="direction"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="direction">The direction of the <see cref="Ray"/></param>
        /// <param name="recursionDepth">The recursion depth of the <see cref="Ray"/></param>
        public PathSegment(SurfacePoint origin, Vector3 direction, int recursionDepth = 0, float length = float.PositiveInfinity) : this(origin, direction, length, recursionDepth) { }

        /// <summary> Create a <see cref="Ray"/> between a specified <paramref name="origin"/> and <paramref name="destination"/> </summary>
        /// <param name="origin">The origin of the <see cref="Ray"/></param>
        /// <param name="destination">The destination of the <see cref="Ray"/></param>
        /// <param name="recursionDepth">The recursion depth of the <see cref="Ray"/></param>
        public PathSegment(SurfacePoint origin, SurfacePoint destination, int recursionDepth = 0) {
            Vector3 originToDestination = destination.Position - origin.Position;
            Direction = originToDestination.Normalized();
            Origin = origin.GetRaisedOrigin(Direction);
            Length = originToDestination.Length;
            Destination = destination.GetRaisedOrigin(-Direction);
            RecursionDepth = recursionDepth;
            DirectionInverted = new Vector3(1 / Direction.X, 1 / Direction.Y, 1 / Direction.Z);
            Sign = new Vector3i(DirectionInverted.X < 0 ? 1 : 0, DirectionInverted.Y < 0 ? 1 : 0, DirectionInverted.Z < 0 ? 1 : 0);
        }

        /// <summary> Trace the <see cref="Ray"/> through a <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to trace through</param>
        /// <returns>An <see cref="RaySurfaceInteraction"/> if there is one</returns>
        public virtual SurfacePoint? Trace(Scene scene) {
            (Shape, float)? intersection = scene.Trace(this);
            if (intersection.HasValue) {
                (Shape primitive, float distance) = intersection.Value;
                Vector3 position = WalkAlong(distance);
                Vector3 normal = primitive.SurfaceNormal(position);
                SurfacePoint surfacePoint = new(primitive, position, normal);
                Length = distance;
                Destination = surfacePoint.GetRaisedOrigin(-Direction);
                return surfacePoint;
            } else {
                return null;
            }
        }

        public bool Occluded(Scene scene) {
            return scene.IntersectBool(this);
        }

        /// <summary> Walk along the <see cref="Ray"/> for a specified <paramref name="distance"/> </summary>
        /// <param name="distance">The distance to walk along the <see cref="Ray"/></param>
        /// <returns>The position found after walking a specified <paramref name="distance"/> along the <see cref="Ray"/></returns>
        public Vector3 WalkAlong(float distance) {
            return Origin.Position + Direction * distance;
        }

        /// <summary> Check whether the specified <paramref name="distance"/> is within the ray interval </summary>
        /// <param name="distance">The specified distance to check</param>
        /// <returns>Whether the <paramref name="distance"/> falls in the ray interval</returns>
        public bool WithinBounds(float distance) {
            return 0 < distance && distance < Length;
        }
    }
}