namespace Algebra.Geometry;
public class Line(Number mx, Number my, Number mz, Number vx, Number vy, Number vz) {
    public Number E01 { get; } = vx;
    public Number E02 { get; } = vy;
    public Number E03 { get; } = vz;
    public Number E12 { get; } = mx;
    public Number E31 { get; } = my;
    public Number E23 { get; } = mz;
}
