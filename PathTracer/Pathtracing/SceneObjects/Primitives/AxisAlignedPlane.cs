using OpenTK.Mathematics;
using System;

namespace PathTracer.Pathtracing.SceneObjects.Primitives {
    public class AxisAlignedPlane : Primitive {
        /// <summary> The normal of the <see cref="AxisAlignedPlane"/> </summary>
        public Vector3 Normal { get; }
        /// <summary> The bounds of the <see cref="AxisAlignedPlane"/> </summary>
        public override Vector3[] Bounds => new Vector3[] { Vector3.NegativeInfinity * Normal, Vector3.PositiveInfinity * -Normal };

        /// <summary> Create a new <see cref="AxisAlignedPlane"/> </summary>
        /// <param name="normal">The <see cref="Normal"/> of the <see cref="Plane"/></param>
        /// <param name="position">The position of the <see cref="Plane"/></param>
        public AxisAlignedPlane(Vector3 normal, Vector3 position) : base(position) {
            Normal = normal;
        }

        public override Vector3 GetSurfacePoint(Random random) {
            throw new NotImplementedException();
        }

        public override Vector3 GetNormal(Vector3 surfacePoint) {
            throw new NotImplementedException();
        }

        public override bool IntersectBool(Ray ray) {
            throw new NotImplementedException();
        }

        public override Intersection? Intersect(Ray ray) {
            throw new NotImplementedException();
        }
    }
}
