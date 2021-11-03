using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    public struct InfinityPlane : IShape {
        public bool Volumetric => false;
        public float Volume => 0;
        public float SurfaceArea => float.PositiveInfinity;
        public AxisAlignedBox BoundingBox => new(Position3.MinValue, Position3.MaxValue);

        public IEnumerable<Position1> IntersectDistances(IRay ray) {
            yield return Position1.MinValue;
            yield return Position1.MaxValue;
        }

        public bool OnSurface(Position3 position, float epsilon = 0.001F) {
            throw new NotImplementedException();
        }

        public Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);

        public Normal3 SurfaceNormal(Position3 position) {
            return new Normal3(-(Direction3)position);
        }

        public Position3 SurfacePosition(Random random) {
            throw new NotImplementedException();
        }

        public Position2 UVPosition(Position3 position) {
            throw new NotImplementedException();
        }
    }
}
