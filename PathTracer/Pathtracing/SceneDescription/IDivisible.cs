using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;

namespace PathTracer.Pathtracing.SceneDescription;

/// <summary> Defines an object that is <see cref="IDivisible{T}"/> into multiple smaller parts </summary>
/// <typeparam name="T">The type that the <see cref="IDivisible{T}"/> divides into</typeparam>
public interface IDivisible<out T> where T : IDivisible<T> {
    /// <summary> Clip the <see cref="IShape"/> by a <paramref name="plane"/> </summary>
    /// <param name="plane">The <see cref="AxisAlignedPlane"/> to clip the <see cref="IShape"/> with</param>
    /// <returns>The clipped <see cref="IShape"/>s</returns>
    IEnumerable<T> Clip(AxisAlignedPlane plane);
}
