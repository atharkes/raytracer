using OpenTK.Mathematics;
using PathTracer.Pathtracing.Rays;

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

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns>null</returns>
        public override Intersection? Intersect(Ray ray) {
            if (ray is LightRay lightRay && lightRay.Light == this) {
                return new Intersection(lightRay, this, lightRay.Length);
            } else {
                return null;
            }
        }

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns>false</returns>
        public override bool IntersectBool(Ray ray) {
            if (ray is LightRay lightRay && lightRay.Light == this) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary> A point light doesn't really have a normal </summary>
        /// <param name="intersectionLocation">The intersection point which cannot be on the surface</param>
        /// <returns>A zero vector</returns>
        public override Vector3 GetNormal(Vector3 intersectionLocation) {
            return Vector3.Zero;
        }

        /// <summary> Get the emittance of the pointlight </summary>
        /// <param name="intersection">The intersection at this pointlight</param>
        /// <returns>The emittance of the pointlight at the intersection</returns>
        public override Vector3 GetEmmitance(Intersection intersection) {
            return Material.EmittingLight * DistanceAttenuation(intersection.Distance);
        }

        /// <summary> The distance attenuation of a point light when this is a shadow ray </summary>
        public float DistanceAttenuation(float length) {
            return 1f / (length * length);
        }
    }
}