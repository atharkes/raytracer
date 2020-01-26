using OpenTK;

namespace WhittedRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A point light to illuminate your scene </summary>
    class PointLight : Primitive {
        /// <summary> The AABB of the pointlight is just the point </summary>
        public override (Vector3 Min, Vector3 Max) AABBBounds => (Position, Position);

        /// <summary> Create a new point light </summary>
        /// <param name="position">The position of the point light</param>
        /// <param name="color">The color of the point light</param>
        /// <param name="intensity">Intensity of the point light</param>
        public PointLight(Vector3 position, Vector3 color, float intensity) : base(position, new Material(intensity, color)) { }

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns>-1f</returns>
        public override float Intersect(Ray ray) {
            return -1f;
        }

        /// <summary> You cannot intersect a point light </summary>
        /// <param name="ray">The ray that will never intersect the pointlight</param>
        /// <returns>False</returns>
        public override bool IntersectBool(Ray ray) {
            return false;
        }

        /// <summary> A point light doesn't really have a normal </summary>
        /// <param name="intersectionPoint">The intersection point which cannot be on the surface</param>
        /// <returns>A zero vector</returns>
        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return Vector3.Zero;
        }
    }
}