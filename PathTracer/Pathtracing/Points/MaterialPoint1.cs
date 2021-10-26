using OpenTK.Mathematics;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Points;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A point on the surface of a <see cref="SceneDescription.Shape"/>  </summary>
    public class MaterialPoint1 : IMaterialPoint1 {
        /// <summary> The <see cref="IPrimitive"/> on which the <see cref="MaterialPoint1"/> is lying </summary>
        public IMaterial Material { get; }
        /// <summary> The point on the surface of an <see cref="IPrimitive"/> </summary>
        public Position1 Position { get; }
        /// <summary> The surface normal at the <see cref="Position"/></summary>
        public Normal1 Normal { get; }

        /// <summary> Create a <see cref="MaterialPoint1"/> </summary>
        /// <param name="material">The <see cref="IMaterial"/> of the surface at the <paramref name="position"/> </param>
        /// <param name="position">The location of the <see cref="MaterialPoint1"/></param>
        /// <param name="normal">The normal of the <paramref name="material"/> at the <paramref name="position"/></param>
        public MaterialPoint1(IMaterial material, Position1 position, Normal1 normal) {
            Material = material;
            Position = position;
            Normal = normal;
        }

        /// <summary> Check whether a <paramref name="direction"/> goes into the surface at this <see cref="MaterialPoint1"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes into the surface</returns>
        public bool IsTowards(IDirection1 direction) => (this as IPoint1).IsTowards(direction);

        /// <summary> Check whether a <paramref name="direction"/> goes away from the surface at this <see cref="MaterialPoint1"/> </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes away from the surface</returns>
        public bool IsFrom(IDirection1 direction) => (this as IPoint1).IsFrom(direction);

        #region Too be (re)moved
        /// <summary> Get the reflected <paramref name="direction"/> at this <see cref="MaterialPoint1"/> </summary>
        /// <param name="direction">The incoming direction</param>
        /// <returns>The reflected <paramref name="direction"/></returns>
        public Vector3 Reflect(Vector3 direction) {
            return direction - 2 * Vector3.Dot(direction, Normal) * Normal;
        }

        /// <summary> Get the refracted <paramref name="direction"/> at this <see cref="MaterialPoint1"/> </summary>
        /// <param name="direction">The incoming direction</param>
        /// <returns>The refracted <paramref name="direction"/></returns>
        /// <exception cref="InvalidOperationException">When the <see cref="MaterialPoint1"/> doesn't refract at the angle of the incoming <paramref name="direction"/></exception>
        public Vector3 Refract(Vector3 direction) {
            float n1 = IsTowards(direction) ? 1f : Primitive.Material.RefractionIndex;
            float n2 = IsTowards(direction) ? Primitive.Material.RefractionIndex : 1f;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(-Normal, direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) throw new InvalidOperationException("Material does not refract at this angle");
            return refraction * direction + Normal * (refraction * cosThetaInc - (float)Math.Sqrt(k));
        }

        /// <summary> Get the reflectivity of the <see cref="MaterialPoint1"/> under the incoming <paramref name="direction"/> using the Fresnel equations </summary>
        /// <returns>The reflectivity at the <see cref="MaterialPoint1"/> under the incoming <paramref name="direction"/></returns>
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
