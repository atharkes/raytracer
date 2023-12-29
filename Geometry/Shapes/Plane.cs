namespace Geometry.Shapes;
/// <summary> A plane is defined using its homogenous equation ax + by + cz + d = 0 </summary>
/// <param name="a"></param>
/// <param name="b"></param>
/// <param name="c"></param>
/// <param name="d"></param>
public class Plane(Number a, Number b, Number c, Number d) {
    public Number E0 { get; } = d;
    public Number E1 { get; } = a;
    public Number E2 { get; } = b;
    public Number E3 { get; } = c;
}
