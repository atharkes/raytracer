﻿using OpenTK;
using System;
using WhittedRaytracer.Utilities;

namespace WhittedRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A sphere primitive for the 3d scene </summary>
    public class Sphere : Primitive {
        /// <summary> The radius of the sphere </summary>
        public float Radius { get; set; }
        /// <summary> Get the AABB bounds of the sphere </summary>
        public override (Vector3 Min, Vector3 Max) AABBBounds => (Position - Vector3.One * Radius, Position + Vector3.One * Radius);
        /// <summary> Create a random Sphere </summary>
        public static new Sphere Random => new Sphere(Utils.RandomVector * 100f, Utils.Random.Next(0, 100));

        /// <summary> Create a new sphere object for the 3d scene </summary>
        /// <param name="position">The position of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="material">The material of the sphere</param>
        public Sphere(Vector3 position, float radius = 1, Material material = null) : base(position, material) {
            Radius = radius;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>The distance at which the ray intersects the sphere</returns>
        public override float Intersect(Ray ray) {
            Vector3 sphereFromRayOrigin = Position - ray.Origin;
            float sphereInDirectionOfRay = Vector3.Dot(sphereFromRayOrigin, ray.Direction);
            float rayNormalDistance = Vector3.Dot(sphereFromRayOrigin, sphereFromRayOrigin) - sphereInDirectionOfRay * sphereInDirectionOfRay;
            if (rayNormalDistance > Radius * Radius) return -1f;
            float raySphereDist = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
            float intersection1 = sphereInDirectionOfRay - raySphereDist;
            float intersection2 = sphereInDirectionOfRay + raySphereDist;
            if (intersection1 > 0 && intersection1 < ray.Length) return intersection1;
            if (intersection2 > 0 && intersection2 < ray.Length) return intersection2;
            return -1f;
        }

        /// <summary> Intersect the sphere with a ray </summary>
        /// <param name="ray">The ray to intersect the sphere with</param>
        /// <returns>Whether the ray intersects the sphere</returns>
        public override bool IntersectBool(Ray ray) {
            return Intersect(ray) > 0;
        }

        /// <summary> Get the normal of the sphere at a point of intersection </summary>
        /// <param name="intersectionPoint">The intersection point to get the normal at</param>
        /// <returns>The normal at the point of intersection</returns>
        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return (intersectionPoint - Position).Normalized();
        }
    }
}