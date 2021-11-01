using PathTracer.Geometry.Vectors;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Paths {
    /// <summary> An <see cref="IPathSegment"/> for an <see cref="ISurfacePoint"/> </summary>
    public interface IPointSegment : IPathSegment {
        /// <summary> The predecessing <see cref="IRaySegment"/> of this <see cref="IPointSegment"/> (if it's not a root node) </summary>
        new IRaySegment? Predecessor { get; }
        IPathSegment? IPathSegment.Predecessor => Predecessor;

        /// <summary> The direction samples at this <see cref="IPointSegment"/> </summary>
        IDictionary<Vector3, IRaySegment> Samples { get; }

        /// <summary> Add a new direction sample to the <see cref="IPointSegment"/> </summary>
        /// <param name="direction">The direction of the sample</param>
        /// <returns>The <see cref="IRaySegment"/> created for the sample</returns>
        IRaySegment AddSample(Vector3 direction);
    }
}
