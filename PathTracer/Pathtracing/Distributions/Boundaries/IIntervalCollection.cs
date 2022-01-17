using PathTracer.Geometry.Positions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> A collection of <see cref="IInterval"/>s </summary>
    public interface IIntervalCollection : IInterval, ICollection<IInterval> {
        /// <summary> The <see cref="IInterval"/>s in the <see cref="IIntervalCollection"/> </summary>
        ICollection<IInterval> Intervals { get; }

        /// <summary> The first entry of the <see cref="IIntervalCollection"/> </summary>
        Position1 IInterval<Position1>.Entry => Intervals.Min(i => i.Entry);
        /// <summary> The alst exit of the <see cref="IIntervalCollection"/> </summary>
        Position1 IInterval<Position1>.Exit => Intervals.Max(i => i.Exit);
        /// <summary> The covered area by the <see cref="IIntervalCollection"/> </summary>
        float CoveredArea => throw new NotImplementedException("Necessary when perfect importance sampling is not possible.");

        /// <summary> Whether the <see cref="IIntervalCollection"/> is volumetric </summary>
        bool IInterval<Position1>.Volumetric => Intervals.Any(i => i.Volumetric);
        /// <summary> Whether the <see cref="IIntervalCollection"/> is planar </summary>
        bool IInterval<Position1>.Planar => Intervals.All(i => i.Planar);

        /// <summary> The amount of items in the <see cref="IIntervalCollection"/> </summary>
        int ICollection<IInterval>.Count => Intervals.Count;
        /// <summary> Whether the <see cref="IIntervalCollection"/> is Read-Only </summary>
        bool ICollection<IInterval>.IsReadOnly => Intervals.IsReadOnly;

        /// <summary> Add two <see cref="IBoundaryCollection"/>s together </summary>
        /// <param name="left">The left <see cref="IBoundaryCollection"/></param>
        /// <param name="right">The right <see cref="IBoundaryCollection"/></param>
        /// <returns>An <see cref="IBoundaryCollection"/> containing the combined <see cref="IShapeInterval"/>s of <paramref name="left"/> and <paramref name="right"/></returns>
        public static IIntervalCollection? operator +(IIntervalCollection? left, IIntervalCollection? right) {
            if (right is null) {
                return left;
            } else if (left is null) {
                return right;
            } else {
                left.Add(right);
                return left;
            }
        }

        /// <summary> Add another <see cref="IIntervalCollection"/> to this <see cref="IIntervalCollection"/> </summary>
        /// <param name="other">The other <see cref="IIntervalCollection"/></param>
        void Add(IIntervalCollection other) {
            foreach (IInterval interval in other) {
                Intervals.Add(interval);
            }
        }

        /// <summary> Check whether the <paramref name="item"/> falls inside the <see cref="IIntervalCollection"/> </summary>
        /// <param name="item">The <see cref="Position1"/> to check</param>
        /// <returns>Whether the <paramref name="item"/> falls inside the <see cref="IIntervalCollection"/></returns>
        bool IInterval<Position1>.Includes(Position1 item) => Intervals.Any(i => i.Includes(item));

        /// <summary> Check whether the <paramref name="item"/> falls outside the <see cref="IIntervalCollection"/> </summary>
        /// <param name="item">The <see cref="Position1"/> to check</param>
        /// <returns>Whether the <paramref name="item"/> falls outside the <see cref="IIntervalCollection"/></returns>
        bool IInterval<Position1>.Excludes(Position1 item) => Intervals.All(i => i.Excludes(item));

        /// <summary> Add an <paramref name="item"/> to the <see cref="IIntervalCollection"/> </summary>
        /// <param name="item">The <see cref="IInterval"/> to add to the <see cref="IIntervalCollection"/></param>
        void ICollection<IInterval>.Add(IInterval item) => Intervals.Add(item);

        /// <summary> Clear the items from the <see cref="IIntervalCollection"/> </summary>
        void ICollection<IInterval>.Clear() => Intervals.Clear();

        /// <summary> Check whether the <see cref="IIntervalCollection"/> contains an <paramref name="item"/> </summary>
        /// <param name="item">The <see cref="IInterval"/> to check</param>
        /// <returns>Whether the <see cref="IIntervalCollection"/> contains the <paramref name="item"/></returns>
        bool ICollection<IInterval>.Contains(IInterval item) => Intervals.Contains(item);

        /// <summary> Copy the items from the <see cref="IIntervalCollection"/> to an <paramref name="array"/></summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The index in the <paramref name="array"/> to copy it to</param>
        void ICollection<IInterval>.CopyTo(IInterval[] array, int arrayIndex) => Intervals.CopyTo(array, arrayIndex);

        /// <summary> Remove an <paramref name="item"/> from the <see cref="IIntervalCollection"/> </summary>
        /// <param name="item">The <see cref="IInterval"/> to remove from the <see cref="IIntervalCollection"/></param>
        /// <returns>Whether the <paramref name="item"/> was present in the <see cref="IIntervalCollection"/></returns>
        bool ICollection<IInterval>.Remove(IInterval item) => Intervals.Remove(item);

        /// <summary> Get the <see cref="IEnumerator{T}"/> of the <see cref="IIntervalCollection"/> </summary>
        /// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="IIntervalCollection"/></returns>
        IEnumerator<IInterval> IEnumerable<IInterval>.GetEnumerator() => Intervals.GetEnumerator();

        /// <summary> Get the <see cref="IEnumerator"/> of the <see cref="IIntervalCollection"/> </summary>
        /// <returns>The <see cref="IEnumerator"/> of the <see cref="IIntervalCollection"/></returns>
        IEnumerator IEnumerable.GetEnumerator() => Intervals.GetEnumerator();
    }
}
