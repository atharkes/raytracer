using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.Observers.Accumulators {
    /// <summary> The part of the <see cref="ICamera"/> that accumulates the light samples </summary>
    public interface IAccumulator {
        /// <summary> The average accumulated light in the <see cref="IAccumulator"/> </summary>
        ISpectrum AccumulatedLight { get; }
        /// <summary> The amount of samples in the <see cref="IAccumulator"/> </summary>
        int SampleCount { get; }

        /// <summary> Add a <paramref name="sample"/> to the <see cref="IAccumulator"/></summary>
        /// <param name="sample">The <see cref="ISample"/> to add</param>
        void Add(ISample sample);

        /// <summary> Clear the samples in the <see cref="IAccumulator"/> </summary>
        void Clear();

        /// <summary> Draw the samples to the <paramref name="screen"/> </summary>
        /// <param name="screen">The <see cref="IScreen"/> to draw to</param>
        /// <param name="drawingMode">The <see cref="DrawingMode"/></param>
        void DrawImage(IScreen screen, DrawingMode drawingMode);
    }
}
