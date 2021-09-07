using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    public class ClippedShape : Shape {
        IShape OriginalShape { get; }
        public override bool Volumetric => OriginalShape.Volumetric;
        public override float Volume => OriginalShape.Volume;
        public override float SurfaceArea => OriginalShape.SurfaceArea;
        public override AxisAlignedBox BoundingBox { get; }

        public ClippedShape(IShape original, AxisAlignedBox clippedBoundingBox) {
            OriginalShape = original;
            BoundingBox = clippedBoundingBox;
        }

        public override bool OnSurface(Vector3 position, float epsilon = 0.001F) => OriginalShape.OnSurface(position, epsilon);
        public override Vector3 PointOnSurface(Random random) => OriginalShape.PointOnSurface(random);
        public override Vector3 SurfaceNormal(Vector3 position) => OriginalShape.SurfaceNormal(position);
        public override IEnumerable<float> IntersectDistances(IRay ray) => OriginalShape.IntersectDistances(ray);
    }
}
