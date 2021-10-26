using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Points {
    /// <summary> A point in space where a boundary is encountered along a traced <see cref="IRay"/> </summary>
    public struct ShapePoint1 : IShapePoint1 {
        /// <summary> The <see cref="IShape"/> of the <see cref="IShapePoint1"/> </summary>
        public IShape Shape { get; }
        /// <summary> The position of the <see cref="IShapePoint1"/> </summary>
        public Position1 Position { get; }
        /// <summary> The outward-pointing normal of the boundary at the <see cref="Position"/> </summary>
        public Normal1 Normal { get; }

        /// <summary> Create a new <see cref="ShapePoint1"/> </summary>
        /// <param name="position">The position of the <see cref="ShapePoint1"/></param>
        /// <param name="normal">The normal of the boundary at the <paramref name="position"/></param>
        public ShapePoint1(IShape shape, Position1 position, Normal1 normal) {
            Shape = shape;
            Position = position;
            Normal = normal;
        }
    }
}
