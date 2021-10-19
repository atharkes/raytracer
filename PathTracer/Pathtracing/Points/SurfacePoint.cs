using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A point on the surface of a <see cref="SceneDescription.Shape"/>  </summary>
    public class SurfacePoint : ISurfacePoint {
        /// <summary> The <see cref="IPrimitive"/> on which the <see cref="SurfacePoint"/> is lying </summary>
        public IPrimitive Primitive { get; }
        /// <summary> The point on the surface of an <see cref="IPrimitive"/> </summary>
        public Vector3 Position { get; }
        /// <summary> The surface normal at the <see cref="Position"/></summary>
        public Vector3 Normal { get; }

        /// <summary> Create a <see cref="SurfacePoint"/> </summary>
        /// <param name="primitive">The <see cref="IPrimitive"/> of the surface at the <paramref name="position"/> </param>
        /// <param name="position">The location of the <see cref="SurfacePoint"/></param>
        /// <param name="normal">The normal of the <paramref name="primitive"/> at the <paramref name="position"/></param>
        public SurfacePoint(IPrimitive primitive, Vector3 position, Vector3 normal) {
            Primitive = primitive;
            Position = position;
            Normal = normal;
        }

        /// <summary> Check whether a <paramref name="direction"/> goes into the surface at this <see cref="SurfacePoint"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes into the surface</returns>
        public bool IsTowards(Vector3 direction) => (this as IPoint).IsTowards(direction);

        /// <summary> Check whether a <paramref name="direction"/> goes away from the surface at this <see cref="SurfacePoint"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes away from the surface</returns>
        public bool IsFrom(Vector3 direction) => (this as IPoint).IsFrom(direction);

        #region Too be (re)moved
        /// <summary> Get the reflected <paramref name="direction"/> at this <see cref="SurfacePoint"/> </summary>
        /// <param name="direction">The incoming direction</param>
        /// <returns>The reflected <paramref name="direction"/></returns>
        public Vector3 Reflect(Vector3 direction) {
            return direction - 2 * Vector3.Dot(direction, Normal) * Normal;
        }

        /// <summary> Get the refracted <paramref name="direction"/> at this <see cref="SurfacePoint"/> </summary>
        /// <param name="direction">The incoming direction</param>
        /// <returns>The refracted <paramref name="direction"/></returns>
        /// <exception cref="InvalidOperationException">When the <see cref="SurfacePoint"/> doesn't refract at the angle of the incoming <paramref name="direction"/></exception>
        public Vector3 Refract(Vector3 direction) {
            float n1 = IsTowards(direction) ? 1f : Primitive.Material.RefractionIndex;
            float n2 = IsTowards(direction) ? Primitive.Material.RefractionIndex : 1f;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(-Normal, direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) throw new InvalidOperationException("Material does not refract at this angle");
            return refraction * direction + Normal * (refraction * cosThetaInc - (float)Math.Sqrt(k));
        }

        /// <summary> Get the reflectivity of the <see cref="SurfacePoint"/> under the incoming <paramref name="direction"/> using the Fresnel equations </summary>
        /// <returns>The reflectivity at the <see cref="SurfacePoint"/> under the incoming <paramref name="direction"/></returns>
        public float Reflectivity(Vector3 direction) {
            float n1 = IsTowards(direction) ? 1f : Primitive.Material.RefractionIndex;
            float n2 = IsTowards(direction) ? Primitive.Material.RefractionIndex : 1f;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(-Normal, direction);
            float k = 1f - refraction * refraction * (1f - cosThetaInc * cosThetaInc);
            if (k < 0f) return 1f; // Full internal reflection
            float cosThetaOut = (float)Math.Sqrt(k);
            float reflectSPolarized = (float)Math.Pow((n1 * cosThetaInc - n2 * cosThetaOut) / (n1 * cosThetaInc + n2 * cosThetaOut), 2);
            float reflectPPolarized = (float)Math.Pow((n1 * cosThetaOut - n2 * cosThetaInc) / (n1 * cosThetaOut + n2 * cosThetaInc), 2);
            return 0.5f * (reflectSPolarized + reflectPPolarized);
        }
        #endregion
    }
}
