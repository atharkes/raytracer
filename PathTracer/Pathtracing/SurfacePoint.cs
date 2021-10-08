using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using System;

namespace PathTracer.Pathtracing {
    /// <summary> A point on the surface of a <see cref="SceneDescription.Shape"/>  </summary>
    public class SurfacePoint : ISurfacePoint {
        /// <summary>
        /// Epsilon used to raise the surface point away from the primitive.
        /// Used to avoid the intersection falling behind the primitive by rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.001f;

        /// <summary> The <see cref="SceneDescription.Shape"/> on which the <see cref="SurfacePoint"/> is lying </summary>
        public IPrimitive Primitive { get; }
        /// <summary> The point on the surface of a <see cref="SceneDescription.Shape"/> </summary>
        public Vector3 Position { get; }
        /// <summary> The normal of the <see cref="SceneDescription.Shape"/> at the <see cref="Position"/></summary>
        public Vector3 Normal { get; }
        /// <summary> The amount to raise or lower <see cref="RaisedSurfacePoint"/>s </summary>
        public Vector3 Raise { get; }
        /// <summary> The <see cref="RaisedSurfacePoint"/> above the <see cref="SurfacePoint"/> </summary>
        public RaisedSurfacePoint AboveSurfacePoint { get; }
        /// <summary> The <see cref="RaisedSurfacePoint"/> below the <see cref="SurfacePoint"/> </summary>
        public RaisedSurfacePoint BelowSurfacePoint { get; }

        /// <summary> Create a <see cref="SurfacePoint"/> </summary>
        /// <param name="primitive">The <see cref="SceneDescription.Primitive"/> of the surface at the <paramref name="position"/> </param>
        /// <param name="position">The location of the <see cref="SurfacePoint"/></param>
        /// <param name="normal">The normal of the <paramref name="primitive"/> at the <paramref name="position"/></param>
        public SurfacePoint(IPrimitive primitive, Vector3 position, Vector3 normal) {
            Primitive = primitive;
            Position = position;
            Normal = normal;
            Raise = Normal * RaiseEpsilon;
            AboveSurfacePoint = new(Position + Raise, this);
            BelowSurfacePoint = new(Position - Raise, this);
        }

        /// <summary> Check whether a <paramref name="direction"/> goes into the surface at this <see cref="SurfacePoint"/> </summary>
        /// <param name="direction">The incoming direction</param>
        /// <returns>Whether the <paramref name="direction"/> goes into the surface</returns>
        public bool IntoSurface(Vector3 direction) {
            return Vector3.Dot(direction, Normal) < 0;
        }

        /// <summary> Get the <see cref="RaisedSurfacePoint"/> for a <see cref="Ray"/> leaving in the specified <paramref name="direction"/> </summary>
        /// <param name="direction">The outgoing direction of the <see cref="Ray"/></param>
        /// <returns>The <see cref="RaisedSurfacePoint"/> to use when leaving in the specified <paramref name="direction"/></returns>
        public RaisedSurfacePoint GetRaisedOrigin(Vector3 direction) {
            if (IntoSurface(direction)) {
                return BelowSurfacePoint;
            } else {
                return AboveSurfacePoint;
            }
        }

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
            float n1 = IntoSurface(direction) ? 1f : Primitive.Material.RefractionIndex;
            float n2 = IntoSurface(direction) ? Primitive.Material.RefractionIndex : 1f;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(-Normal, direction);
            float k = 1 - refraction * refraction * (1 - cosThetaInc * cosThetaInc);
            if (k < 0) throw new InvalidOperationException("Material does not refract at this angle");
            return refraction * direction + Normal * (refraction * cosThetaInc - (float)Math.Sqrt(k));
        }

        /// <summary> Get the reflectivity of the <see cref="SurfacePoint"/> under the incoming <paramref name="direction"/> using the Fresnel equations </summary>
        /// <returns>The reflectivity at the <see cref="SurfacePoint"/> under the incoming <paramref name="direction"/></returns>
        public float Reflectivity(Vector3 direction) {
            float n1 = IntoSurface(direction) ? 1f : Primitive.Material.RefractionIndex;
            float n2 = IntoSurface(direction) ? Primitive.Material.RefractionIndex : 1f;
            float refraction = n1 / n2;
            float cosThetaInc = Vector3.Dot(-Normal, direction);
            float k = 1f - refraction * refraction * (1f - cosThetaInc * cosThetaInc);
            if (k < 0f) return 1f; // Full internal reflection
            float cosThetaOut = (float)Math.Sqrt(k);
            float reflectSPolarized = (float)Math.Pow((n1 * cosThetaInc - n2 * cosThetaOut) / (n1 * cosThetaInc + n2 * cosThetaOut), 2);
            float reflectPPolarized = (float)Math.Pow((n1 * cosThetaOut - n2 * cosThetaInc) / (n1 * cosThetaOut + n2 * cosThetaInc), 2);
            return 0.5f * (reflectSPolarized + reflectPPolarized);
        }
    }
}
