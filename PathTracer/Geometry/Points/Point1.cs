using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;

namespace PathTracer.Geometry.Points;

public readonly struct Point1 : IPoint1 {
    public Position1 Position { get; }
    public Normal1 Normal { get; }

    public Point1(Position1 position, Normal1 normal) {
        Position = position;
        Normal = normal;
    }

    public IPoint1 NormalFlipped() => new Point1(Position, -Normal);
}
