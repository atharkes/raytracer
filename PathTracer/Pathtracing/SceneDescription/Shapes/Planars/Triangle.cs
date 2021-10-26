using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Triangle"/> <see cref="Shape"/> </summary>
    public class Triangle : PlanarShape {
        /// <summary> Epsilon used for the Möller–Trumbore triangle intersection </summary>
        public const float IntersectionEpsilon = 0.0000001f;

        /// <summary> The first point of the <see cref="Triangle"/> </summary>
        public Vector3 P1 { get; }
        /// <summary> The second point of the <see cref="Triangle"/> </summary>
        public Vector3 P2 { get; }
        /// <summary> The third point of the <see cref="Triangle"/> </summary>
        public Vector3 P3 { get; }
        /// <summary> The normal of the <see cref="Triangle"/> </summary>
        public Vector3 Normal { get; }

        /// <summary> The surface area of the <see cref="Triangle"/> </summary>
        public override float SurfaceArea => Vector3.Cross(P2 - P1, P3 - P1).Length * 0.5f;
        /// <summary> The <see cref="Plane"/> in which the <see cref="Triangle"/> lies </summary>
        public override Plane PlaneOfExistence => new(P1, Normal);
        /// <summary> The bounding box of the <see cref="Triangle"/> </summary>
        public override AxisAlignedBox BoundingBox => new(P1, P2, P3);

        /// <summary> Create a new <see cref="Triangle"/></summary>
        /// <param name="p1">The first point of the <see cref="Triangle"/></param>
        /// <param name="p2">The second point of the <see cref="Triangle"/></param>
        /// <param name="p3">The third point of the <see cref="Triangle"/></param>
        /// <param name="normal">The normal of the <see cref="Triangle"/>. Clockwise in order of points is the default.</param>
        /// <param name="material">The <see cref="Material"/> of the <see cref="Triangle"/></param>
        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3? normal = null) {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Normal = normal?.Normalized() ?? Vector3.Cross(p2 - p1, p3 - p1).Normalized();
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Triangle"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Triangle"/></returns>
        public override Vector3 SurfacePosition(Random random) {
            Vector3 P1toP2 = P2 - P1;
            Vector3 P1toP3 = P3 - P1;
            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();
            if (r1 + r2 > 1) {
                r1 = 1 - r1;
                r2 = 1 - r2;
            }
            return P1 + P1toP2 * r1 + P1toP3 * r2;
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Triangle"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Triangle"/></returns>
        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Triangle"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="Triangle"/></returns>
        public override Vector3 SurfaceNormal(Vector3 surfacePoint) {
            return Normal;
        }

        /// <summary> Intersect the <see cref="Triangle"/> with a <paramref name="ray"/>.
        /// Using Möller–Trumbore's triangle intersection. </summary>
        /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="Triangle"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> intersects the <see cref="Triangle"/></returns>
        public override float? IntersectDistance(IRay ray) {
            // Get vectors for two edges sharing V1
            Vector3 P1toP2 = P2 - P1;
            Vector3 P1toP3 = P3 - P1;

            // Begin calculating determinant - also used to calculate u parameter
            Vector3 P = Vector3.Cross(ray.Direction, P1toP3);
            // If determinant is near zero, ray lies in plane of triangle
            float determinant = Vector3.Dot(P1toP2, P);
            if (determinant > -IntersectionEpsilon && determinant < IntersectionEpsilon) return null;
            float determinantInverted = 1f / determinant;

            // Calculate distance from P1 to ray origin
            Vector3 T = ray.Origin - P1;

            // Calculate u and test bound
            float u = Vector3.Dot(T, P) * determinantInverted;
            if (u < 0f || u > 1f) return null;

            // Calculate v and test bound
            Vector3 Q = Vector3.Cross(T, P1toP2);
            float v = Vector3.Dot(ray.Direction, Q) * determinantInverted;
            if (v < 0f || u + v > 1f) return null;

            float t = Vector3.Dot(P1toP3, Q) * determinantInverted;
            return t;
        }

        /// <summary> Clip the <see cref="Triangle"/> by a <paramref name="plane"/> </summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="triangle"/> with</param>
        /// <returns>The new points of the clipped <see cref="Triangle"/></returns>
        public IEnumerable<Vector3> GetClippedPoints(AxisAlignedPlane plane) {
            Vector3 v0 = P1 - plane.Position, v1 = P2 - plane.Position, v2 = P3 - plane.Position, v3;
            const float clipEpsilon = 0.00001f, clipEpsilon2 = 0.01f;
            // Distances to the plane (this is an array parallel to v[], stored as a vec3)
            Vector3 dist = new(Vector3.Dot(v0, plane.Normal), Vector3.Dot(v1, plane.Normal), Vector3.Dot(v2, plane.Normal));
            if (dist.X < clipEpsilon2 && dist.Y < clipEpsilon2 && dist.Z < clipEpsilon2) {
                // Case 1 (all clipped)
                return Array.Empty<Vector3>();
            }
            if (dist.X > -clipEpsilon && dist.Y > -clipEpsilon && dist.Z > -clipEpsilon) {
                // Case 2 (none clipped)
                return new Vector3[] { v0, v1, v2 };
            }
            // There are either 1 or 2 vertices above the clipping plane
            bool above0 = dist.X >= 0;
            bool above1 = dist.Y >= 0;
            bool above2 = dist.Z >= 0;
            bool nextIsAbove;
            // Find the CCW - most vertex above the plane
            if (above1 && !above0) {
                // Cycle once CCW. Use v3 as a temp
                nextIsAbove = above2;
                v3 = v0; v0 = v1; v1 = v2; v2 = v3;
                dist = new Vector3(dist.Y, dist.Z, dist.X);
            } else if (above2 && !above1) {
                // Cycle once CW. Use v3 as a temp
                nextIsAbove = above0;
                v3 = v2; v2 = v1; v1 = v0; v0 = v3;
                dist = new Vector3(dist.Z, dist.X, dist.Y);
            } else nextIsAbove = above1;
            // We always need to clip v2 - v0
            v3 = Vector3.Lerp(v0, v2, dist[0] / (dist[0] - dist[2]));
            if (nextIsAbove) {
                // Case 3 (quadrilateral)
                v2 = Vector3.Lerp(v1, v2, dist[1] / (dist[1] - dist[2]));
                return new Vector3[] { v0, v1, v2, v3 };
            } else {
                // Case 4 (triangle)
                v1 = Vector3.Lerp(v0, v1, dist[0] / (dist[0] - dist[1]));
                v2 = v3;
                return new Vector3[] { v0, v1, v2 };
            }
        }
    }
}