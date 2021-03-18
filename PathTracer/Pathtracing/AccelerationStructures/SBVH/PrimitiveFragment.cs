using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneObjects;
using System;

namespace PathTracer.Pathtracing.AccelerationStructures.SBVH {
    /// <summary> A fragment of a <see cref="Primitive"/> used for spatial splits in the <see cref="SBVHTree"/> </summary>
    public class PrimitiveFragment : Primitive {
        /// <summary> The original <see cref="Primitive"/> of the <see cref="PrimitiveFragment"/> </summary>
        public Primitive Original { get; }
        /// <summary> The bounds of the <see cref="PrimitiveFragment"/> </summary>
        public override Vector3[] Bounds { get; }
        /// <summary> The surface area of the <see cref="PrimitiveFragment"/> </summary>
        public override float SurfaceArea => Original.SurfaceArea;

        /// <summary> Create a new <see cref="PrimitiveFragment"/> </summary>
        /// <param name="original">The original <see cref="Primitive"/></param>
        /// <param name="bounds">The bounds of the <see cref="PrimitiveFragment"/></param>
        public PrimitiveFragment(Primitive original, Vector3[] bounds) {
            Original = original;
            Bounds = bounds;
            Position = bounds[0] + 0.5f * (bounds[1] - bounds[0]);
        }

        /// <summary> Intersect the <see cref="PrimitiveFragment"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="PrimitiveFragment"/> with</param>
        /// <returns>Whether and when the <paramref name="ray"/> intersects the <see cref="PrimitiveFragment"/></returns>
        public override float? Intersect(Ray ray) {
            return Original.Intersect(ray);
        }

        /// <summary> Intersect the <see cref="PrimitiveFragment"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="PrimitiveFragment"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="PrimitiveFragment"/></returns>
        public override bool Intersects(Ray ray) {
            return Original.Intersects(ray);
        }

        /// <summary> Get a <paramref name="random"/> surface point on the <see cref="PrimitiveFragment"/> </summary>
        /// <param name="random">The <see cref="Random"/> to determine the position on the surface</param>
        /// <returns>A <paramref name="random"/> surface point on the <see cref="PrimitiveFragment"/></returns>
        public override Vector3 PointOnSurface(Random random) {
            return Original.PointOnSurface(random);
        }

        /// <summary> Get the normal of the <see cref="PrimitiveFragment"/> at a specified <paramref name="surfacePoint"/> </summary>
        /// <param name="surfacePoint">The surface point to get the normal for</param>
        /// <returns>The normal of the <see cref="PrimitiveFragment"/> at the specified <paramref name="surfacePoint"/> </returns>
        public override Vector3 GetNormal(Vector3 surfacePoint) {
            return Original.GetNormal(surfacePoint);
        }

        public override Vector3 GetEmmitance(Ray ray) {
            return Original.GetEmmitance(ray);
        }

        /// <summary> Clip the <see cref="PrimitiveFragment"/> by a <paramref name="plane"/></summary>
        /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="PrimitiveFragment"/> with</param>
        /// <returns>A new <see cref="PrimitiveFragment"/> with clipped bounds</returns>
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
