using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using System;

namespace PathTracer.Pathtracing {
    /// <summary> An interaction between a <see cref="Ray"/> and a <see cref="Primitive"/> at a <see cref="Pathtracing.SurfacePoint"/> </summary>
    public class RaySurfaceInteraction {
        /// <summary> The <see cref="Ray"/> that interacts with a <see cref="Pathtracing.SurfacePoint"/> </summary>
        public Ray IncomingRay { get; }
        /// <summary> The interaction point at a surface of a <see cref="Primitive"/> </summary>
        public SurfacePoint SurfacePoint { get; }

        /// <summary> The distance from the origin of the ray to the intersection </summary>
        public float Distance => (SurfacePoint.Position - IncomingRay.Origin.Position).Length;
        /// <summary> Whether the ray is entering the primitive or exiting it </summary>
        public bool IntoSurface => SurfacePoint.IntoSurface(IncomingRay.Direction);
        /// <summary> The conversion factor when converting irradiance to radiance </summary>
        public float NdotL => Math.Abs(Vector3.Dot(IncomingRay.Direction, SurfacePoint.Normal));

        /// <summary> Create a new <see cref="RaySurfaceInteraction"/> between a <see cref="Ray"/> and a <see cref="Pathtracing.SurfacePoint"/> </summary>
        /// <param name="incomingRay">The <see cref="Ray"/> that interacts with the <paramref name="surfacePoint"/></param>
        /// <param name="surfacePoint">The <see cref="Pathtracing.SurfacePoint"/> of a <see cref="Primitive"/></param>
        public RaySurfaceInteraction(Ray incomingRay, SurfacePoint surfacePoint) {
            IncomingRay = incomingRay;
            SurfacePoint = surfacePoint;
        }

        /// <summary> Get the reflectivity of the <see cref="SurfacePoint"/> under the angle of the <see cref="IncomingRay"/> using the Fresnel equations </summary>
        /// <returns>The reflectivity at the <see cref="SurfacePoint"/></returns>
        public float Reflectivity() => SurfacePoint.Reflectivity(IncomingRay.Direction);

        /// <summary> Get the reflected <see cref="Ray"/> at this <see cref="RaySurfaceInteraction"/> </summary>
        /// <returns>The reflected <see cref="Ray"/></returns>
        public Ray Reflect() {
            Vector3 reflectedDirection = SurfacePoint.Reflect(IncomingRay.Direction);
            return new Ray(SurfacePoint, reflectedDirection, IncomingRay.RecursionDepth + 1);
        }

        /// <summary> Get the refracted <see cref="Ray"/> at this <see cref="RaySurfaceInteraction"/> </summary>
        /// <returns>The refracted <see cref="Ray"/></returns>
        /// <exception cref="InvalidOperationException">When the <see cref="SurfacePoint"/> doesn't refract at the angle of the <see cref="IncomingRay"/></exception>
        public Ray Refract() {
            Vector3 refractedDirection = SurfacePoint.Refract(IncomingRay.Direction);
            return new Ray(SurfacePoint, refractedDirection, IncomingRay.RecursionDepth + 1);
        }
    }
}