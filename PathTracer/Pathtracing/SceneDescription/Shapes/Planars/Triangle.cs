using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Triangle"/> <see cref="Shape"/> </summary>
    public struct Triangle : IPlanarShape {
        /// <summary> Epsilon used for the Möller–Trumbore triangle intersection </summary>
        public const float IntersectionEpsilon = 0.0000001f;

        /// <summary> The first point of the <see cref="Triangle"/> </summary>
        public Position3 P1 { get; }
        /// <summary> The second point of the <see cref="Triangle"/> </summary>
        public Position3 P2 { get; }
        /// <summary> The third point of the <see cref="Triangle"/> </summary>
        public Position3 P3 { get; }
        /// <summary> The normal of the <see cref="Triangle"/> </summary>
        public Normal3 Normal { get; }

        /// <summary> The direction vector from <see cref="P1"/> to <see cref="P2"/></summary>
        public Direction3 P1toP2 => P2 - P1;
        /// <summary> The direction vector from <see cref="P1"/> to <see cref="P3"/> </summary>
        public Direction3 P1toP3 => P3 - P1;

        /// <summary> The surface area of the <see cref="Triangle"/> </summary>
        public float SurfaceArea => Vector3.Cross(P2.Vector - P1.Vector, P3.Vector - P1.Vector).Length * 0.5f;
        /// <summary> The <see cref="Plane"/> in which the <see cref="Triangle"/> lies </summary>
        public Plane PlaneOfExistence => new(P1, Normal);
        /// <summary> The bounding box of the <see cref="Triangle"/> </summary>
        public AxisAlignedBox BoundingBox => new(P1, P2, P3);

        /// <summary> Create a new <see cref="Triangle"/></summary>
        /// <param name="p1">The first point of the <see cref="Triangle"/></param>
        /// <param name="p2">The second point of the <see cref="Triangle"/></param>
        /// <param name="p3">The third point of the <see cref="Triangle"/></param>
        /// <param name="normal">The normal of the <see cref="Triangle"/>. Clockwise in order of points is the default.</param>
        /// <param name="material">The <see cref="Material"/> of the <see cref="Triangle"/></param>
        public Triangle(Position3 p1, Position3 p2, Position3 p3, Normal3? normal = null) {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Normal = normal ?? Normal3.Cross((P2 - P1).Normalized(), (P3 - P1).Normalized());
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Triangle"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Triangle"/></returns>
        public Position3 SurfacePosition(Random random) {
            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();
            if (r1 + r2 > 1) {
                r1 = 1 - r1;
                r2 = 1 - r2;
            }
            return P1 + P1toP2 * r1 + P1toP3 * r2;
        }

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        public Position2 UVPosition(Position3 position) => throw new NotImplementedException();

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Triangle"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Triangle"/></returns>
        public bool OnSurface(Position3 position, float epsilon = 0.001F) => throw new NotImplementedException();

        /// <summary> Get the normal of the <see cref="Triangle"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Triangle"/></returns>
        public Normal3 SurfaceNormal(Position3 surfacePoint) => Normal;

        /// <summary> Intersect the <see cref="Triangle"/> with a <paramref name="ray"/>.
        /// Using Möller–Trumbore's triangle intersection. </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Triangle"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> intersects the <see cref="Triangle"/></returns>
        public Position1? IntersectDistance(IRay ray) {
            // Begin calculating determinant - also used to calculate u parameter
            Vector3 P = Vector3.Cross(ray.Direction.Vector, P1toP3.Vector);
            // If determinant is near zero, ray lies in plane of triangle
            float determinant = Vector3.Dot(P1toP2.Vector, P);
            if (determinant > -IntersectionEpsilon && determinant < IntersectionEpsilon) return null;
            float determinantInverted = 1f / determinant;

            // Calculate distance from P1 to ray origin
            Direction3 T = ray.Origin - P1;

            // Calculate u and test bound
            float u = Vector3.Dot(T.Vector, P) * determinantInverted;
            if (u < 0f || u > 1f) return null;

            // Calculate v and test bound
            Vector3 Q = Vector3.Cross(T.Vector, P1toP2.Vector);
            float v = Vector3.Dot(ray.Direction.Vector, Q) * determinantInverted;
            if (v < 0f || u + v > 1f) return null;

            float t = Vector3.Dot(P1toP3.Vector, Q) * determinantInverted;
            return t;
        }

        /// <summary> Clip the <see cref="Triangle"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="triangle"/> with</param>
        /// <returns>The new points of the clipped <see cref="Triangle"/></returns>
        public IEnumerable<Position3> GetClippedPoints(AxisAlignedPlane plane) {
            Direction3[] v = new Direction3[4] { P1 - plane.Position, P2 - plane.Position, P3 - plane.Position, Vector3.Zero };
            const float clipEpsilon = 0.00001f, clipEpsilon2 = 0.01f;
            // Distances to the plane (this is an array parallel to v[], stored as a vec3)
            Vector1[] dist = new Vector1[3] { IDirection3.Dot(v[0], plane.Normal), IDirection3.Dot(v[1], plane.Normal), IDirection3.Dot(v[2], plane.Normal) };
            if (dist[0] < clipEpsilon2 && dist[1] < clipEpsilon2 && dist[2] < clipEpsilon2) {
                // Case 1 (all clipped)
                return Array.Empty<Position3>();
            }
            if (dist[0] > -clipEpsilon && dist[1] > -clipEpsilon && dist[2] > -clipEpsilon) {
                // Case 2 (none clipped)
                return new Position3[] { (Position3)v[0], (Position3)v[1], (Position3)v[2] };
            }
            // There are either 1 or 2 vertices above the clipping plane
            bool above0 = dist[0] >= 0;
            bool above1 = dist[1] >= 0;
            bool above2 = dist[2] >= 0;
            bool nextIsAbove;
            // Find the CCW - most vertex above the plane
            if (above1 && !above0) {
                // Cycle once CCW. Use v3 as a temp
                nextIsAbove = above2;
                v[3] = v[0];
                v[0] = v[1];
                v[1] = v[2];
                v[2] = v[3];
                dist = new Vector1[3] { dist[1], dist[2], dist[0] };
            } else if (above2 && !above1) {
                // Cycle once CW. Use v3 as a temp
                nextIsAbove = above0;
                v[3] = v[2];
                v[2] = v[1];
                v[1] = v[0];
                v[0] = v[3];
                dist = new Vector1[] { dist[2], dist[0], dist[1] };
            } else nextIsAbove = above1;
            // We always need to clip v2 - v0
            v[3] = Direction3.Lerp(v[0], v[2], dist[0] / (dist[0] - dist[2]));
            if (nextIsAbove) {
                // Case 3 (quadrilateral)
                v[2] = Direction3.Lerp(v[1], v[2], dist[1] / (dist[1] - dist[2]));
                return new Position3[] { plane.Position + v[0], plane.Position + v[1], plane.Position + v[2], plane.Position + v[3] };
            } else {
                // Case 4 (triangle)
                v[1] = Direction3.Lerp(v[0], v[1], dist[0] / (dist[0] - dist[1]));
                v[2] = v[3];
                return new Position3[] { plane.Position + v[0], plane.Position + v[1], plane.Position + v[2] };
            }
        }
    }
}