using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> A <see cref="Sphere"/> <see cref="Shape"/> to represent objects in the <see cref="Scene"/> </summary>
    public class Sphere : VolumetricShape {
        /// <summary> Position of the <see cref="Sphere"/> </summary>
        public Position3 Position { get; set; }
        /// <summary> The radius of the <see cref="Sphere"/> </summary>
        public Direction1 Radius { get; set; }

        /// <summary> The volume of the <see cref="Sphere"/> </summary>
        public override float Volume => 4f / 3f * (float)Math.PI * Radius * Radius * Radius;
        /// <summary> The surface area of the <see cref="Sphere"/> </summary>
        public override float SurfaceArea => 4f * (float)Math.PI * Radius * Radius;
        /// <summary> The bounding box of the <see cref="Sphere"/> </summary>
        public override AxisAlignedBox BoundingBox => new(Position - new Direction3(Radius), Position + new Direction3(Radius));

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
        public override Position3 SurfacePosition(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface position for which to get the UV-position</param>
        /// <returns>The UV-position for the <paramref name="position"/></returns>
        public override Position2 UVPosition(Position3 position) {
            throw new NotImplementedException();
        }

        /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="Sphere"/> </summary>
        /// <param name="position">The position to check</param>
        /// <param name="epsilon">The epsilon to specify the precision</param>
        /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="Sphere"/></returns>
        public override bool OnSurface(Position3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Sphere"/> at a specified <paramref name="position"/> </summary>
        /// <param name="position">The surface point to get the normal for</param>
        /// <returns>The normal at the <paramref name="position"/></returns>
        public override Normal3 SurfaceNormal(Position3 position) {
            return (position - Position).Normalized();
        }

        /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
        /// <param name="position">The position to get the outwards direction from</param>
        /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
        public override Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);

        /// <summary> Intersect the <see cref="Sphere"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Sphere"/> with</param>
        /// <returns>Whether and when the <see cref="Ray"/>intersects the <see cref="Sphere"/></returns>
        public override IEnumerable<Position1> IntersectDistances(IRay ray) {
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