using PathTracer.Pathtracing.SceneDescription;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Boundaries {
    /// <summary> The boundary of a <see cref="IShape"/>-<see cref="IRay"/>-intersection </summary>
    public interface IBoundary {
        /// <summary> The <see cref="IBoundaryPoint"/>s making up th <see cref="IBoundary"/> </summary>
        public IEnumerable<IBoundaryPoint> BoundaryPoints { get; }
        /// <summary> Whether the <see cref="IBoundary"/> is volumetric </summary>
        public bool Volumetric { get; }
        /// <summary> Whether the <see cref="IBoundary"/> is planar </summary>
        public bool Planar => !Volumetric;
        /// <summary> The amount of times the <see cref="IRay"/> entered the <see cref="IBoundary"/> </summary>
        public int Passthroughs { get; }

        public static IBoundary operator +(IBoundary left, IBoundary right) {

        }

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundary"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundary"/></returns>
        public bool Inside(double distance);

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundary"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundary"/></returns>
        public bool Outside(double distance) => !Inside(distance);

        public IBoundaryPoint FirstEntry(double start, double end);

        public IEnumerable<(IBoundaryPoint Entry, IBoundaryPoint Exit)> PassthroughIntervals();

        public IBoundaryPoint Entry(int index);

        public IBoundaryPoint Exit(int index);

        
    }
}
