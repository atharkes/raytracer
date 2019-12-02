using OpenTK;
using System;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing {
    /// <summary> A data structure to store an intersection between a ray and a primitive </summary>
    class Intersection {
        /// <summary> The ray that intersects the primitive </summary>
        public readonly Ray Ray;
        /// <summary> The primitive that is intersected by the ray </summary>
        public readonly Primitive Primitive;
        /// <summary> The distance from the origin of the ray to the intersection </summary>
        public readonly float Distance;
        /// <summary> Whether the ray is entering the primitive or exiting it </summary>
        public readonly bool IntoPrimitive;

        /// <summary> The position of the intersection </summary>
        public readonly Vector3 Position;
        /// <summary> The normal of the primitive at the intersection </summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// Epsilon used to raise the intersection away from the primitive.
        /// Used to avoid the intersection falling behind the primitive by rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.00001f;

        /// <summary> Create a new intersection between a ray and a primitive </summary>
        /// <param name="ray">The ray that intersects the primitive</param>
        /// <param name="primitive">The primitive that is intersected by the ray</param>
        /// <param name="position">The position of the intersection</param>
        public Intersection(Ray ray, Primitive primitive, float distance) {
            Ray = ray;
            Ray.Length = distance;
            Primitive = primitive;
            Distance = distance;
            Vector3 position = Ray.Origin + Ray.Direction * distance;
            Vector3 normal = primitive.GetNormal(position);
            IntoPrimitive = Vector3.Dot(Ray.Direction, normal) < 0;
            Normal = IntoPrimitive ? normal : -normal;
            Position = position + Normal * RaiseEpsilon;
        }

        /// <summary> Get a shadow ray from this intersection to a lightsource </summary>
        /// <param name="lightsource">The lightsource to send the shadow ray to</param>
        /// <returns>A shadow ray from the intersection to the lightsource</returns>
        public Ray GetShadowRay(Lightsource lightsource) {
            Vector3 direction = lightsource.Position - Position;
            return new Ray(Position, direction, direction.Length);
        }

        /// <summary> Get a reflection of the incoming ray at this intersection </summary>
        /// <returns>The reflected ray</returns>
        public Ray GetReflectedRay() {
            Vector3 reflectedDirection = Ray.Direction - 2 * Vector3.Dot(Ray.Direction, Normal) * Normal;
            return new Ray(Position, reflectedDirection);
        }

        /// <summary> Get a refracted ray of the incoming ray at this intersection </summary>
        /// <returns>The refracted ray</returns>
        public Ray GetRefractedRay() {
            float n1 = IntoPrimitive ? Primitive.RefractionIndex : 1;
            float n2 = IntoPrimitive ? 1 : Primitive.RefractionIndex;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(Normal, -Ray.Direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) return null;
            Vector3 refractedDirection = refraction * Ray.Direction + Normal * (refraction * cosThetaInc - (float)Math.Sqrt(k));
            return new Ray(Position + refractedDirection * RaiseEpsilon * 2, refractedDirection);
        }

        /// <summary> Get the reflectivity of the surface of the dielectric </summary>
        /// <returns>The reflectivity</returns>
        public float GetReflectivity() {
            float n1 = IntoPrimitive ? Primitive.RefractionIndex : 1;
            float n2 = IntoPrimitive ? 1 : Primitive.RefractionIndex;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(Normal, -Ray.Direction);
            float cosThetaOut = (float)Math.Sqrt(1 - (refraction * refraction * cosThetaInc * cosThetaInc));
            float reflectSPolarized = (float)Math.Pow((n1 * cosThetaInc - n2 * cosThetaOut) / (n1 * cosThetaInc + n2 * cosThetaOut), 2);
            float reflectPPolarized = (float)Math.Pow((n1 * cosThetaOut - n2 * cosThetaInc) / (n1 * cosThetaOut + n2 * cosThetaInc), 2);
            return 0.5f * (reflectSPolarized + reflectPPolarized);
        }
    }
}