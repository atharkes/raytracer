using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

public readonly struct Point3 : IPoint3 {
    public Position3 Position { get; }
    public Normal3 Normal { get; }

    public Point3(Position3 position, Normal3 normal) {
        Position = position;
        Normal = normal;
    }

    public IPoint3 NormalFlipped() => new Point3(Position, -Normal);
}
