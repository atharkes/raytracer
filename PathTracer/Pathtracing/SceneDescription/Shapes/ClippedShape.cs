using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
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

        public override bool OnSurface(Position3 position, float epsilon = 0.001F) => OriginalShape.OnSurface(position, epsilon);
        public override Position3 SurfacePosition(Random random) => OriginalShape.SurfacePosition(random);
        public override Normal3 SurfaceNormal(Position3 position) => OriginalShape.SurfaceNormal(position);
        public override IEnumerable<Position1> IntersectDistances(IRay ray) => OriginalShape.IntersectDistances(ray);
    }
}
