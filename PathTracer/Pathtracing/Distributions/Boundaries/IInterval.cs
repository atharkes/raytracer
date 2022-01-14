using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> An interface for a boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public interface IInterval : IEquatable<IInterval>, IComparable<IInterval>  {
        /// <summary> The entry-point of the <see cref="IInterval"/> </summary>
        Position1 Entry { get; }
        /// <summary> The exit-point of the <see cref="IInterval"/> </summary>
        Position1 Exit { get; }

        /// <summary> Whether the <see cref="IInterval"/> is a valid interval </summary>
        bool Valid => Entry <= Exit;
        /// <summary> Whether the <see cref="IInterval"/> is volumetric </summary>
        bool Volumetric => Entry < Exit;
        /// <summary> Whether the <see cref="IInterval"/> is planar </summary>
        bool Planar => Entry == Exit;

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IInterval"/></returns>
        bool Includes(double distance) => Entry <= distance && distance <= Exit;

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IInterval"/></returns>
        bool Excludes(double distance) => distance < Entry || Exit < distance;

        /// <summary> Check whether the <see cref="IInterval"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="IInterval"/></param>
        /// <returns>Whether the <paramref name="other"/> is the same</returns>
        bool IEquatable<IInterval>.Equals(IInterval? other) {
            return Entry.Equals(other?.Entry) && Exit.Equals(other?.Exit);
        }

        /// <summary> Compare the <see cref="IInterval"/> to an <paramref name="other"/>"/> </summary>
        /// <param name="other">The other <see cref="IInterval"/></param>
        /// <returns>A comparison between the two <see cref="IInterval"/>s</returns>
        int IComparable<IInterval>.CompareTo(IInterval? other) {
            return (Entry.CompareTo(other?.Entry) == 0) ? Exit.CompareTo(other?.Exit) : Entry.CompareTo(other?.Entry);
        }
    }
}
