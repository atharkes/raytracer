﻿namespace Geometry.Shapes;
/// <summary> A point is just a homogeneous point, Euclidean coordinates plus the origin </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="z"></param>
public class Point(Number x, Number y, Number z) {
    public Number E123 { get; } = 1;
    public Number E032 { get; } = x;
    public Number E013 { get; } = y;
    public Number E021 { get; } = z;
}