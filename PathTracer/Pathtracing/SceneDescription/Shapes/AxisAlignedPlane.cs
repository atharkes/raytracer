using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    public class AxisAlignedPlane : Shape {
        /// <summary> The normal of the <see cref="AxisAlignedPlane"/> </summary>
        public Vector3 Normal { get; }
        /// <summary> The bounds of the <see cref="AxisAlignedPlane"/> </summary>
        public override Vector3[] Bounds => new Vector3[] { Vector3.NegativeInfinity * Normal, Vector3.PositiveInfinity * -Normal };
        /// <summary> The surface area of the <see cref="AxisAlignedPlane"/> </summary>
        public override float SurfaceArea => float.PositiveInfinity;

        /// <summary> Create a new <see cref="AxisAlignedPlane"/> </summary>
        /// <param name="normal">The normal of the <see cref="Plane"/></param>
        /// <param name="position">The position of the <see cref="Plane"/></param>
        public AxisAlignedPlane(Vector3 normal, Vector3 position) : base(position) {
            Normal = normal;
        }

        public override Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException();
        }

        public override Vector3 GetNormal(Vector3 surfacePoint) {
            throw new NotImplementedException();
        }

        public override float? Intersect(Ray ray) {
            throw new NotImplementedException();
        }
    }
}
