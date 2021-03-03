using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using System;

namespace PathTracer.Pathtracing.AccelerationStructures.SBVH {
    /// <summary> A fragment of a primitive for the SBVH construction </summary>
    public class PrimitiveFragment : Primitive {
        /// <summary> The original primitive of the fragment </summary>
        public Primitive Original { get; }
        /// <summary> The bounds of the fragment </summary>
        public override Vector3[] Bounds { get; }

        /// <summary> Create a new fragment of a primitive </summary>
        /// <param name="original">The original primitive</param>
        /// <param name="bounds">The bounds of the fragment</param>
        public PrimitiveFragment(Primitive original, Vector3[] bounds) {
            Original = original;
            Bounds = bounds;
            Position = bounds[0] + 0.5f * (bounds[1] - bounds[0]);
        }

        /// <summary> Intersect the fragment with a ray by intersecting its original primitive </summary>
        /// <param name="ray">The ray to intersect the fragment with</param>
        /// <returns>The intersection with the original primitive if there is any</returns>
        public override float? Intersect(Ray ray) {
            return Original.Intersect(ray);
        }

        /// <summary> Intersect the fragment with a ray by intersecting its original primitive </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the original primitive</returns>
        public override bool IntersectBool(Ray ray) {
            return Original.IntersectBool(ray);
        }

        /// <summary> Get a <paramref name="random"/> surface point on the <see cref="PrimitiveFragment"/> </summary>
        /// <param name="random">The <see cref="Random"/> to determine the location of the surface point</param>
        /// <returns>A <paramref name="random"/> surface point on the <see cref="Original"/></returns>
        public override Vector3 GetSurfacePoint(Random random) {
            return Original.GetSurfacePoint(random);
        }

        /// <summary> Get the normal of the fragment at an intersect location using the original primitive </summary>
        /// <param name="surfacePoint">The intersection point to get the normal at</param>
        /// <returns>The normal of the original primitive at the intersection point</returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return Original.GetNormal(surfacePoint);
        }

        /// <summary> Clipping a fragment doesn't create a fragment of a fragment </summary>
        /// <param name="plane">The plane to clip the fragment with</param>
        /// <returns>Creates a smaller fragment of the original primitive</returns>
        public override PrimitiveFragment? Clip(AxisAlignedPlane plane) {
            PrimitiveFragment? newFragment = Original.Clip(plane);
            if (newFragment == null) {
                return null;
            } else {
                Vector3[] newBounds = { Vector3.ComponentMin(Bounds[0], newFragment.Bounds[0]), Vector3.ComponentMin(Bounds[1], newFragment.Bounds[1]) };
                return new PrimitiveFragment(Original, newBounds);
            }
        }
    }
}
