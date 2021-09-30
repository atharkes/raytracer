using System.Collections.Generic;

namespace PathTracer.Pathtracing.Paths {
    /// <summary> An <see cref="IPathSegment"/> for an <see cref="IRay"/> </summary>
    public interface IRaySegment : IPathSegment {
        /// <summary> The predecessing <see cref="IPointSegment"/> of this <see cref="IRaySegment"/> (if it's not a root node) </summary>
        new IPointSegment? Predecessor { get; }
        IPathSegment? IPathSegment.Predecessor => Predecessor;

        /// <summary> The distance samples for this <see cref="IRaySegment"/> </summary>
        IDictionary<double, IPointSegment> Samples { get; }

        /// <summary> Add a new distance sample to the <see cref="IRaySegment"/> </summary>
        /// <param name="distance">The distance of the sample</param>
        /// <returns>The <see cref="IPointSegment"/> created for the sample</returns>
        IPointSegment AddSample(double distance);
    }
}
