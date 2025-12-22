using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

public readonly struct Point2 : IPoint2 {
    public Position2 Position { get; }
    public Normal2 Normal { get; }

    public Point2(Position2 position, Normal2 normal) {
        Position = position;
        Normal = normal;
    }

    public IPoint2 NormalFlipped() => new Point2(Position, -Normal);
}
