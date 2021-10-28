using PathTracer.Pathtracing.SceneDescription;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Boundaries {
    /// <summary> The boundary collection of a <see cref="IShape"/>-<see cref="IRay"/> intersection </summary>
    public interface IBoundaryCollection : IEnumerable<IShapeInterval> {
        /// <summary> The <see cref="IShapeInterval"/>s making up the <see cref="IBoundaryCollection"/> </summary>
        IEnumerable<IShapeInterval> BoundaryIntervals { get; }

        /// <summary> Whether the <see cref="IBoundaryCollection"/> is volumetric </summary>
        bool Volumetric => BoundaryIntervals.Any(i => i.Volumetric);
        /// <summary> Whether the <see cref="IBoundaryCollection"/> is planar </summary>
        bool Planar => BoundaryIntervals.All(i => i.Planar);
        /// <summary> The amount of times the <see cref="IRay"/> entered the <see cref="IBoundaryCollection"/> </summary>
        int Passthroughs => BoundaryIntervals.Count();

        /// <summary> Add two <see cref="IBoundaryCollection"/>s together </summary>
        /// <param name="left">The left <see cref="IBoundaryCollection"/></param>
        /// <param name="right">The right <see cref="IBoundaryCollection"/></param>
        /// <returns>An <see cref="IBoundaryCollection"/> containing the combined <see cref="IShapeInterval"/>s of <paramref name="left"/> and <paramref name="right"/></returns>
        public static IBoundaryCollection? operator +(IBoundaryCollection? left, IBoundaryCollection? right) {
            if (right is null) {
                return left;
            } else if (left is null) {
                return right;
            } else {
                left.AddRange(right);
                return left;
            }
        }

        /// <summary> Get the <see cref="IEnumerator{T}"/> of the <see cref="IBoundaryCollection"/> </summary>
        /// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="IBoundaryCollection"/></returns>
        new IEnumerator<IShapeInterval> GetEnumerator() => BoundaryIntervals.GetEnumerator();

        /// <summary> Get the <see cref="IEnumerator{T}"/> of the <see cref="IBoundaryCollection"/> </summary>
        /// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="IBoundaryCollection"/></returns>
        IEnumerator<IShapeInterval> IEnumerable<IShapeInterval>.GetEnumerator() => GetEnumerator();

        /// <summary> Get the <see cref="IEnumerator"/> of the <see cref="IBoundaryCollection"/> </summary>
        /// <returns>The <see cref="IEnumerator"/> of the <see cref="IBoundaryCollection"/></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary> Add another <see cref="IBoundaryCollection"/> to this <see cref="IBoundaryCollection"/> </summary>
        /// <param name="other">The other <see cref="IBoundaryCollection"/></param>
        void AddRange(IBoundaryCollection other);

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryCollection"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryCollection"/></returns>
        bool Inside(double distance) => BoundaryIntervals.Any(i => i.Includes(distance));

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryCollection"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryCollection"/></returns>
        bool Outside(double distance) => !Inside(distance);
    }
}
