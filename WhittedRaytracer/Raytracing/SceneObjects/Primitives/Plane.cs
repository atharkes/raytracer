using OpenTK;

namespace WhittedRaytracer.Raytracing.SceneObjects.Primitives {
    /// <summary> A plane for the 3d scene </summary>
    class Plane : Primitive {
        /// <summary> The size of plane objects. Required to fit in an AABB for the BVh </summary>
        public const float Size = 1000;

        /// <summary> Normal vector of the plane </summary>
        public Vector3 Normal { get; set; }
        /// <summary> Distance of the plane from the origin </summary>
        public float Distance { get; set; }

        /// <summary> Returns the AABB bounds of the plane bounded by the size constant </summary>
        public override (Vector3 Min, Vector3 Max) AABBBounds {
            get {
                Vector3 orthogonal = Vector3.One - Normal;
                return (AABBCenter - orthogonal * Size, AABBCenter + orthogonal * Size);
            }
        }

        /// <summary> Create a new plane using a normal and a distance </summary>
        /// <param name="normal">The normal of the plane</param>
        /// <param name="distance">The distance of the plane from the origin</param>
        /// <param name="material">The material of the plane</param>
        public Plane(Vector3 normal, float distance, Material material = null) : base(null, material) {
            Normal = normal.Normalized();
            Distance = distance;
            Position = Normal * Distance;
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
    }
}