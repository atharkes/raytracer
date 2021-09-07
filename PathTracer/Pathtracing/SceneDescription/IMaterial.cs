using PathTracer.Pathtracing.Guiding;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> Trace a <paramref name="ray"/> through the <see cref="IMaterial"/> and return a <see cref="ISurfacePoint"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="IMaterial"/></param>
        /// <param name="boundaryPoint">The point from which it enters the <see cref="IMaterial"/></param>
        /// <returns>A <see cref="ISurfacePoint"/> if the ray scatters</returns>
        IPDF<ISurfacePoint> Scatter(IRay ray, IEnumerable<IBoundaryPoint> boundaryPoints);
    }
}
