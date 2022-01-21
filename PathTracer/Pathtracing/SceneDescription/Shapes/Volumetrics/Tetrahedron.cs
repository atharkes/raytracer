using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    public struct Tetrahedron : IVolumetricShape {
        public float Volume => throw new NotImplementedException();
        public float SurfaceArea => throw new NotImplementedException();
        public AxisAlignedBox BoundingBox => new(vertices);

        readonly Position3[] vertices;
        readonly Triangle[] triangles;

        public static Tetrahedron Regular(Position3 position, float size) {
            Position3 v1 = position + new Direction3(size);
            Position3 v2 = position + new Direction3(size, -size, -size);
            Position3 v3 = position + new Direction3(-size, size, -size);
            Position3 v4 = position + new Direction3(-size, -size, size);
            return new Tetrahedron(v1, v2, v3, v4);
        }

        public Tetrahedron(Position3 v1, Position3 v2, Position3 v3, Position3 v4) {
            vertices = new Position3[] { v1, v2, v3, v4 };
            triangles = new Triangle[] {
                new Triangle(v1, v2, v3),
                new Triangle(v1, v4, v2),
                new Triangle(v2, v4, v3),
                new Triangle(v3, v4, v1),
            };
        }

        public IEnumerable<Position1> IntersectDistances(IRay ray) {
            foreach (Triangle triangle in triangles) {
                if (triangle.IntersectDistance(ray) is Position1 distance) {
                    yield return distance;
                }
            }
        }

        public bool OnSurface(Position3 position, float epsilon = 0.001F) => triangles.Any(t => t.OnSurface(position, epsilon));

        public float DistanceToSurface(Position3 position) => triangles.Min(t => t.DistanceToSurface(position));

        public Normal3 OutwardsDirection(Position3 position) {
            Triangle closestTriangle = triangles.MinBy(t => t.DistanceToSurface(position));
            return (closestTriangle as IShape).OutwardsDirection(position);
        }

        public Normal3 SurfaceNormal(Position3 position) => OutwardsDirection(position);

        public Position3 SurfacePosition(Random random) => throw new NotImplementedException();

        public Position2 UVPosition(Position3 position) => throw new NotImplementedException();
    }
}
