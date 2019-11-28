using OpenTK;
using System.Collections.Generic;

namespace WhittedStyleRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A plane for the 3d scene </summary>
    class Plane : Primitive {
        /// <summary> Normal vector of the plane </summary>
        public Vector3 Normal;
        /// <summary> Distance of the plane from the origin </summary>
        public float Distance;
        /// <summary> The size of the plane. Required for bvh </summary>
        public const float Size = 1000;

        /// <summary> Create a new plane using a normal and a distance </summary>
        /// <param name="normal">The normal of the plane</param>
        /// <param name="distance">The distance of the plane from the origin</param>
        /// <param name="color">The color of the plane</param>
        /// <param name="specularity">The specularity of the plane</param>
        /// <param name="glossyness">The glossyness of the plane</param>
        /// <param name="glossSpecularity">The specularity of the glossyness of the plane</param>
        public Plane(Vector3 normal, float distance, Vector3? color = null, float specularity = 0, float glossyness = 0, float glossSpecularity = 0)
            : base (null, color, specularity, glossyness, glossSpecularity) {
            Normal = normal.Normalized();
            Distance = distance;
        }

        /// <summary> Returns the distance of the intersection </summary>
        /// <param name="ray">The ray to intersect this plane with</param>
        /// <returns>The distance of the intersection</returns>
        public override float Intersect(Ray ray) {
            return -((Vector3.Dot(ray.Origin, Normal) - Distance) / Vector3.Dot(ray.Direction, Normal));
        }

        /// <summary> Intersect the plane with a ray </summary>
        /// <param name="ray">The ray to intersect the plane with</param>
        /// <returns>Whether the ray intersects the plane</returns>
        public override bool IntersectBool(Ray ray) {
            float intersectDistance = Intersect(ray);
            if (intersectDistance > 0 && intersectDistance < ray.Length) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary> Get the normal of the plane </summary>
        /// <param name="intersectionPoint">The point to get the normal at</param>
        /// <returns>The normal of the plane</returns>
        public override Vector3 GetNormal(Vector3 intersectionPoint) {
            return Normal;
        }

        /// <summary> The center of the plane, which is the point closest to the origin </summary>
        /// <returns>The point closest to the origin</returns>
        public override Vector3 GetCenter() {
            return Normal * Distance;
        }

        /// <summary> Returns some arbitrary bounds of the plane </summary>
        /// <returns>The bounds of the plane</returns>
        public override List<Vector3> GetBounds() {
            List<Vector3> bounds = new List<Vector3>(2)
            {
                GetCenter() - (Vector3.One - Normal) * Size,
                GetCenter() + (Vector3.One - Normal) * Size
            };
            return bounds;
        }
    }
}