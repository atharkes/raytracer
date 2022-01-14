using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> An interface for a boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public interface IShapeInterval : IInterval, IEquatable<IShapeInterval>, IComparable<IShapeInterval>  {
        /// <summary> The <see cref="IShape"/> that produced the <see cref="IShapeInterval"/> </summary>
        IShape Shape { get; }

        /// <summary> Check whether the <see cref="IShapeInterval"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="IShapeInterval"/></param>
        /// <returns>Whether the <paramref name="other"/> is the same</returns>
        bool IEquatable<IShapeInterval>.Equals(IShapeInterval? other) {
            return Shape.Equals(other?.Shape) && (this as IInterval).Equals(other);
        }

        /// <summary> Compare the <see cref="IShapeInterval"/> to an <paramref name="other"/>"/> </summary>
        /// <param name="other">The other <see cref="IShapeInterval"/></param>
        /// <returns>A comparison between the two <see cref="IShapeInterval"/>s</returns>
        int IComparable<IShapeInterval>.CompareTo(IShapeInterval? other) {
            return (this as IInterval).CompareTo(other);
        }
    }
}
