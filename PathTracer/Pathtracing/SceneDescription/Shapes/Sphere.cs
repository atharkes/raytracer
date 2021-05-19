using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> A <see cref="Sphere"/> <see cref="Shape"/> to represent objects in the <see cref="Scene"/> </summary>
    public class Sphere : Shape {
        /// <summary> The radius of the <see cref="Sphere"/> </summary>
        public float Radius { get; set; }
        /// <summary> The bounds of the <see cref="Sphere"/> </summary>
        public override Vector3[] Bounds => new Vector3[] { Position - Vector3.One * Radius, Position + Vector3.One * Radius };
        /// <summary> The surface area of the <see cref="Sphere"/> </summary>
        public override float SurfaceArea => 4f * (float)Math.PI * Radius * Radius;

        /// <summary> Create a new <see cref="Sphere"/> </summary>
        /// <param name="position">The position of the <see cref="Sphere"/></param>
        /// <param name="radius">The radius of the <see cref="Sphere"/></param>
        /// <param name="material">The material of the <see cref="Sphere"/></param>
        public Sphere(Vector3 position, float radius = 1, Material? material = null) : base(position, material) {
            Radius = radius;
        }

        /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="Sphere"/> </summary>
        /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
        /// <returns>A <paramref name="random"/> point on the surface of the <see cref="Sphere"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        /// <summary> Get the normal of the <see cref="Sphere"/> at a specified <paramref name="surfacePoint"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal at the <paramref name="surfacePoint"/></returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return (surfacePoint - Position).Normalized();
        }

        /// <summary> Intersect the <see cref="Sphere"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Sphere"/> with</param>
        /// <returns>Whether and when the <see cref="Ray"/>intersects the <see cref="Sphere"/></returns>
        public override float? Intersect(Ray ray) {
            Vector3 rayOriginToSpherePosition = Position - ray.Origin.Position;
            float sphereInDirectionOfRay = Vector3.Dot(rayOriginToSpherePosition, ray.Direction);
            float rayNormalDistance = rayOriginToSpherePosition.LengthSquared - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) {
                return null;
            }
            float raySphereDistance = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            float intersection1 = sphereInDirectionOfRay - raySphereDistance;
            float intersection2 = sphereInDirectionOfRay + raySphereDistance;
            if (ray.WithinBounds(intersection1)) {
                return intersection1;
            } else if (ray.WithinBounds(intersection2)) {
                return intersection2;
            } else {
                return null;
            }
        }
    }
}