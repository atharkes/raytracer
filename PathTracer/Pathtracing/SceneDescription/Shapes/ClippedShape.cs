using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    public class ClippedShape : IShape {
        IShape OriginalShape { get; }
        public bool Volumetric => OriginalShape.Volumetric;
        public float Volume => OriginalShape.Volume;
        public float SurfaceArea => OriginalShape.SurfaceArea;
        public AxisAlignedBox BoundingBox { get; }

        public ClippedShape(IShape original, AxisAlignedBox clippedBoundingBox) {
            OriginalShape = original;
            BoundingBox = clippedBoundingBox;
        }

        public IEnumerable<Position1> IntersectDistances(IRay ray) {
            return (BoundingBox as IIntersectable).Intersects(ray) ? OriginalShape.IntersectDistances(ray) : Enumerable.Empty<Position1>();
        }

        public float DistanceToSurface(Position3 position) => OriginalShape.DistanceToSurface(position);
        public bool OnSurface(Position3 position, float epsilon = 0.0001F) => OriginalShape.OnSurface(position, epsilon);
        public Position3 SurfacePosition(Random random) => OriginalShape.SurfacePosition(random);
        public Position2 UVPosition(Position3 position) => OriginalShape.UVPosition(position);
        public Normal3 SurfaceNormal(Position3 position) => OriginalShape.SurfaceNormal(position);
        public Normal3 OutwardsDirection(Position3 position) => OriginalShape.OutwardsDirection(position);
    }
}
