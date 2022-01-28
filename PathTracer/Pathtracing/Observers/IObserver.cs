using PathTracer.Pathtracing.Observers.Accumulators;
using PathTracer.Pathtracing.Observers.Cameras;

namespace PathTracer.Pathtracing.Observers {
    /// <summary> An observer of a <see cref="IScene"/> </summary>
    public interface IObserver {
        /// <summary> The virtual <see cref="ICamera"/> object of the <see cref="IObserver"/> </summary>
        ICamera Camera { get; }
        /// <summary> The <see cref="IAccumulator"/> used for storing the samples registered by the <see cref="IObserver"/> </summary>
        IAccumulator Accumulator { get; }
    }
}
