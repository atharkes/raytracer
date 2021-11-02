using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Shapes {
    /// <summary> A volumetric <see cref="IShape"/> </summary>
    public interface IVolumetricShape : IShape {
        /// <summary> An <see cref="IVolumetricShape"/> has a volume </summary>
        bool IShape.Volumetric => true;

        /// <summary> Check whether a <paramref name="position"/> is inside the <see cref="IVolumetricShape"/> </summary>
        /// <param name="position">The position to check</param>
        /// <returns>Whether the <paramref name="position"/> is inside the <see cref="IVolumetricShape"/></returns>
        bool IShape.Inside(Position3 position) {
            IRay ray = new Ray(position, new Normal3(1, 0, 0));
            return Intersect(ray)?.Inside(0) ?? false;
        }
    }
}
