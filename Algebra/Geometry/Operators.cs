namespace Algebra.Geometry;
public class Operators {
    public static Line Join(Point p1, Point p2)
        => new(
            p1.E021 * p2.E013 - p1.E013 * p2.E021,
            p1.E032 * p2.E021 - p1.E021 * p2.E032,
            p1.E013 * p2.E032 - p1.E032 * p2.E013,
            p1.E021 - p2.E021,
            p1.E013 - p2.E013,
            p1.E032 - p2.E032
        );
}
