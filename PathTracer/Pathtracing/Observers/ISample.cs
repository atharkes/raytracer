using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.Observers {
    /// <summary> A sample of the <see cref="IAccumulator"/> </summary>
    public interface ISample {
        /// <summary> The position of the <see cref="ISample"/> on the <see cref="IAccumulator"/> (0 <= values < 1) </summary>
        Position2 Position { get; }
        /// <summary> The direction of the <see cref="ISample"/> from the <see cref="IAccumulator"/> (0 <= values < 1) </summary>
        Direction2 Direction { get; }
        /// <summary> The (amount of) light contributed by the <see cref="ISample"/> </summary>
        ISpectrum Light { get; }
        /// <summary> The amount of bounding box traversels by the primary ray </summary>
        int PrimaryBVHTraversals { get; }
        /// <summary> Whether the <see cref="ISample"/> intersected anything </summary>
        bool Intersection { get; }
    }

    public struct Sample : ISample {
        public Position2 Position { get; init; }
        public Direction2 Direction { get; init; }
        public ISpectrum Light { get; init; }
        public int PrimaryBVHTraversals { get; init; }
        public bool Intersection { get; init; }
    }
}
