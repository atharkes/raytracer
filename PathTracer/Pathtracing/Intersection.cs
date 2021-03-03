using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using System;

namespace PathTracer.Pathtracing {
    /// <summary> An interaction between a <see cref="Pathtracing.Ray"/> and a <see cref="SceneObjects.Primitive"/> </summary>
    public class Intersection {
        /// <summary>
        /// Epsilon used to raise the intersection away from the primitive.
        /// Used to avoid the intersection falling behind the primitive by rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.001f;

        /// <summary> The ray that intersects the primitive </summary>
        public Ray Ray { get; }
        /// <summary> The primitive that is intersected by the ray </summary>
        public Primitive Primitive { get; }
        /// <summary> The distance from the origin of the ray to the intersection </summary>
        public float Distance { get; }
        /// <summary> The position of the intersection </summary>
        public Vector3 Position { get; }

        /// <summary> Whether the ray is entering the primitive or exiting it </summary>
        public bool IntoPrimitive { get; }
        /// <summary> The normal of the primitive at the intersection </summary>
        public Vector3 Normal { get; }
        /// <summary> The conversion from irradiance to radiance </summary>
        public float NdotL { get; }

        /// <summary> Create a new intersection between a ray and a primitive </summary>
        /// <param name="ray">The ray that intersects the primitive</param>
        /// <param name="primitive">The primitive that is intersected by the ray</param>
        /// <param name="distance">The distance travelled along the ray</param>
        public Intersection(Ray ray, float distance, Primitive primitive) {
            Ray = ray;
            Primitive = primitive;
            Distance = distance;
            Position = Ray.Origin + Ray.Direction * distance;
            Vector3 normal = primitive.GetNormal(Position);
            IntoPrimitive = Vector3.Dot(Ray.Direction, normal) < 0;
            Normal = IntoPrimitive ? normal : -normal;
            Position = Ray.Origin + Ray.Direction * distance + Normal * RaiseEpsilon;
            NdotL = Vector3.Dot(Ray.Direction, -Normal);
        }

        /// <summary> Get a reflection of the incoming ray at this intersection </summary>
        /// <returns>The reflected ray</returns>
        public Ray GetReflectedRay() {
            Vector3 reflectedDirection = Ray.Direction - 2 * Vector3.Dot(Ray.Direction, Normal) * Normal;
            return new Ray(Position, reflectedDirection, float.MaxValue, Ray.RecursionDepth + 1);
        }

        /// <summary> Get a refracted ray of the incoming ray at this intersection </summary>
        /// <returns>The refracted ray</returns>
        public Ray? GetRefractedRay() {
            float n1 = IntoPrimitive ? 1 : Primitive.Material.RefractionIndex;
            float n2 = IntoPrimitive ? Primitive.Material.RefractionIndex : 1;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(Normal, -Ray.Direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) return null;
            Vector3 refractedDirection = refraction * Ray.Direction + Normal * (refraction * cosThetaInc - (float)Math.Sqrt(k));
            return new Ray(Position, refractedDirection, float.MaxValue, Ray.RecursionDepth + 1);
        }

        /// <summary> Get the reflectivity of the surface of the dielectric </summary>
        /// <returns>The reflectivity</returns>
        public float GetReflectivity() {
            float n1 = IntoPrimitive ? 1 : Primitive.Material.RefractionIndex;
            float n2 = IntoPrimitive ? Primitive.Material.RefractionIndex : 1;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(Normal, -Ray.Direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) return 1f;
            float cosThetaOut = (float)Math.Sqrt(k);
            float reflectSPolarized = (float)Math.Pow((n1 * cosThetaInc - n2 * cosThetaOut) / (n1 * cosThetaInc + n2 * cosThetaOut), 2);
            float reflectPPolarized = (float)Math.Pow((n1 * cosThetaOut - n2 * cosThetaInc) / (n1 * cosThetaOut + n2 * cosThetaInc), 2);
            return 0.5f * (reflectSPolarized + reflectPPolarized);
        }
    }
}