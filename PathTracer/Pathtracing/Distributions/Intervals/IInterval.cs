using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using System;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Intervals {
    /// <summary> A generic interval </summary>
    /// <typeparam name="T">The type of interval</typeparam>
    public interface IInterval<T> : IEquatable<IInterval<T>>, IComparable<IInterval<T>> where T : IEquatable<T>, IComparable<T> {
        /// <summary> The entry-point of the <see cref="IInterval{T}"/> </summary>
        T Entry { get; }
        /// <summary> The exit-point of the <see cref="IInterval{T}"/> </summary>
        T Exit { get; }

        /// <summary> The transition points in the <see cref="IInterval{T}"/> </summary>
        IOrderedEnumerable<T> Transitions => new T[] { Entry, Exit }.OrderBy(t => t);
        /// <summary> Whether the <see cref="IInterval{T}"/> is a valid interval </summary>
        bool Valid => Entry.CompareTo(Exit) <= 0;
        /// <summary> Whether the <see cref="IInterval{T}"/> is volumetric </summary>
        bool Volumetric => Entry.CompareTo(Exit) < 0;
        /// <summary> Whether the <see cref="IInterval{T}"/> is planar </summary>
        bool Planar => Entry.CompareTo(Exit) == 0;

        /// <summary> Check whether the specified <paramref name="item"/> falls inside the <see cref="IInterval{T}"/> </summary>
        /// <param name="item">The <see cref="T"/> to check</param>
        /// <returns>Whether the specified <paramref name="item"/> falls inside the <see cref="IInterval{T}"/></returns>
        bool Includes(T item) => Entry.CompareTo(item) <= 0 && item.CompareTo(Exit) <= 0;

        /// <summary> Check whether the specified <paramref name="item"/> falls outside the <see cref="IInterval{T}"/> </summary>
        /// <param name="item">The <see cref="T"/> to check</param>
        /// <returns>Whether the specified <paramref name="item"/> falls outside the <see cref="IInterval{T}"/></returns>
        bool Excludes(T item) => item.CompareTo(Entry) < 0 || Exit.CompareTo(item) < 0;

        /// <summary> Check whether the <see cref="IInterval{T}"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="IInterval{T}"/></param>
        /// <returns>Whether the <paramref name="other"/> is the same</returns>
        bool IEquatable<IInterval<T>>.Equals(IInterval<T>? other) {
            return other is IInterval<T> interval && Entry.Equals(interval.Entry) && Exit.Equals(interval.Exit);
        }

        /// <summary> Compare the <see cref="IInterval{T}"/> to an <paramref name="other"/>"/> </summary>
        /// <param name="other">The other <see cref="IInterval{T}"/></param>
        /// <returns>A comparison between the two <see cref="IInterval{T}"/>s</returns>
        int IComparable<IInterval<T>>.CompareTo(IInterval<T>? other) {
            if (other is IInterval<T> interval) {
                int entry = Entry.CompareTo(interval.Entry);
                int exit = Exit.CompareTo(interval.Exit);
                return entry != 0 ? entry : exit;
            } else {
                return 1;
            }
        }
    }

    /// <summary> An interface for a boundary interval for an <see cref="IRay"/> traced through a (volumetric) <see cref="IShape"/> </summary>
    public interface IInterval : IInterval<Position1>, IEquatable<IInterval>, IComparable<IInterval> {
        /// <summary> The size of the <see cref="IInterval"/> </summary>
        float Size => Exit - Entry;
        /// <summary> The covered area of the <see cref="IInterval"/> </summary>
        float CoveredArea => Size;

        /// <summary> Check whether the <see cref="IInterval"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="IInterval"/></param>
        /// <returns>Whether the <paramref name="other"/> is the same</returns>
        bool IEquatable<IInterval>.Equals(IInterval? other) {
            return other is not null && Entry.Equals(other.Entry) && Exit.Equals(other.Exit);
        }

        /// <summary> Compare the <see cref="IInterval"/> to an <paramref name="other"/>"/> </summary>
        /// <param name="other">The other <see cref="IInterval"/></param>
        /// <returns>A comparison between the two <see cref="IInterval"/>s</returns>
        int IComparable<IInterval>.CompareTo(IInterval? other) {
            if (other is null) {
                return 1;
            } else {
                int entryComparsion = Entry.CompareTo(other.Entry);
                return entryComparsion != 0 ? entryComparsion : Exit.CompareTo(other.Exit);
            }
        }
    }
}
