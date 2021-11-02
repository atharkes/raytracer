using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Rays;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> A perfect sphere <see cref="Shape"/> </summary>
    public struct Sphere : IVolumetricShape {
        /// <summary> Position of the <see cref="Sphere"/> </summary>
        public Position3 Position { get; set; }
        /// <summary> The radius of the <see cref="Sphere"/> </summary>
        public Vector1 Radius { get; set; }

        /// <summary> The volume of the <see cref="Sphere"/> </summary>
        public float Volume => 4f / 3f * (float)Math.PI * Radius * Radius * Radius;
        /// <summary> The surface area of the <see cref="Sphere"/> </summary>
        public float SurfaceArea => 4f * (float)Math.PI * Radius * Radius;
        /// <summary> The bounding box of the <see cref="Sphere"/> </summary>
        public AxisAlignedBox BoundingBox => new(Position - new Direction3(Radius), Position + new Direction3(Radius));

        /// <summary> Create a new <see cref="Sphere"/> </summary>
        /// <param name="position">The position of the <see cref="Sphere"/></param>
        /// <param name="radius">The radius of the <see cref="Sphere"/></param>
        /// <param name="material">The material of the <see cref="Sphere"/></param>
        public Sphere(Position3 position, float radius = 1) {
            Position = position;
            Radius = radius;
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Sphere"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Sphere"/></returns>
        public Position3 SurfacePosition(Random random) {
            double z = 1 - 2 * random.NextDouble();
            double r = Math.Sqrt(Math.Max(0, 1 - z * z));
            double phi = 2 * Math.PI * random.NextDouble();
            Direction3 direction = new Normal3((float)(Math.Cos(phi) * r), (float)(Math.Sin(phi) * r), (float)z);
            return Position + direction * Radius;
        }

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        public Position2 UVPosition(Position3 position) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Sphere"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Sphere"/></returns>
        public bool OnSurface(Position3 position, float epsilon = 0.001F) => (position - Position).Length.Equals(Radius, epsilon);

        /// <summary> Get the normal of the <see cref="Sphere"/> at a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface point to get the normal for</param>
        /// <returns>The normal at the <paramref name="position"/></returns>
        public Normal3 SurfaceNormal(Position3 position) => OutwardsDirection(position);

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        public Normal3 OutwardsDirection(Position3 position) => (position - Position).Normalized();

        /// <summary> Intersect the <see cref="Sphere"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Sphere"/> with</param>
        /// <returns>Whether and when the <see cref="Ray"/>intersects the <see cref="Sphere"/></returns>
        public IEnumerable<Position1> IntersectDistances(IRay ray) {
            Direction3 rayOriginToSpherePosition = Position - ray.Origin;
            float sphereInDirectionOfRay = IDirection3.Dot(rayOriginToSpherePosition, ray.Direction);
            float rayNormalDistance = rayOriginToSpherePosition.LengthSquared - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) {
                yield break;
            }
            float raySphereDistance = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            yield return sphereInDirectionOfRay - raySphereDistance;
            yield return sphereInDirectionOfRay + raySphereDistance;
        }
    }
}