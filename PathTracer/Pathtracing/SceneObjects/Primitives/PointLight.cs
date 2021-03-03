using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    /// <summary> A point light to illuminate your scene </summary>
    public class PointLight : Primitive {
        /// <summary> The AABB of the pointlight is just the point </summary>
        public override Vector3[] Bounds => new Vector3[] { Position, Position };

        /// <summary> Create a new point light </summary>
        /// <param name="position">The position of the point light</param>
        /// <param name="color">The color of the point light</param>
        /// <param name="intensity">Intensity of the point light</param>
        public PointLight(Vector3 position, Vector3 color, float intensity) : base(position, new Material(intensity, color)) { }

        public override Vector3 GetSurfacePoint(Random random) {
            return Position;
        }

        /// <summary> A point light doesn't really have a normal </summary>
        /// <param name="surfacePoint">The intersection point which cannot be on the surface</param>
        /// <returns>A zero vector</returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return (surfacePoint - Position).Normalized();
        }

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns>false</returns>
        public override bool IntersectBool(Ray ray) {
            return false;
        }

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns></returns>
        public override float? Intersect(Ray ray) {
            return null;
        }

        /// <summary> Get the radiance emitted along a specified <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to emit radiance along</param>
        /// <returns>The radiance emitted from the <see cref="PointLight"/> along the <paramref name="ray"/></returns>
        public override Vector3 GetEmmitance(Ray ray) {
            return Material.EmittingLight / (ray.Length * ray.Length);
        }
    }
}