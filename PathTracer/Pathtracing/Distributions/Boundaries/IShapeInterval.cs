using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> An interface for a boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public interface IShapeInterval : IEquatable<IShapeInterval>, IComparable<IShapeInterval>  {
        /// <summary> The <see cref="IShape"/> that produced the <see cref="IShapeInterval"/> </summary>
        IShape Shape { get; }
        /// <summary> The entry-point of the <see cref="IShapeInterval"/> </summary>
        Position1 Entry { get; }
        /// <summary> The exit-point of the <see cref="IShapeInterval"/> </summary>
        Position1 Exit { get; }

        /// <summary> Whether the <see cref="IShapeInterval"/> is a valid interval </summary>
        bool Valid => Entry <= Exit;
        /// <summary> Whether the <see cref="IShapeInterval"/> is volumetric </summary>
        bool Volumetric => Entry < Exit;
        /// <summary> Whether the <see cref="IShapeInterval"/> is planar </summary>
        bool Planar => Entry == Exit;

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IShapeInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IShapeInterval"/></returns>
        bool Includes(double distance) => Entry <= distance && distance <= Exit;

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IShapeInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IShapeInterval"/></returns>
        bool Excludes(double distance) => distance < Entry || Exit < distance;

        /// <summary> Check whether the <see cref="IShapeInterval"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="IShapeInterval"/></param>
        /// <returns>Whether the <paramref name="other"/> is the same</returns>
        bool IEquatable<IShapeInterval>.Equals(IShapeInterval? other) {
            return Shape.Equals(other?.Shape) && Entry.Equals(other?.Entry) && Exit.Equals(other?.Exit);
        }

        /// <summary> Compare the <see cref="IShapeInterval"/> to an <paramref name="other"/>"/> </summary>
        /// <param name="other">The other <see cref="IShapeInterval"/></param>
        /// <returns>A comparison between the two <see cref="IShapeInterval"/>s</returns>
        int IComparable<IShapeInterval>.CompareTo(IShapeInterval? other) {
            return (Entry.CompareTo(other?.Entry) == 0) ? Exit.CompareTo(other?.Exit) : Entry.CompareTo(other?.Entry);
        }
    }
}
