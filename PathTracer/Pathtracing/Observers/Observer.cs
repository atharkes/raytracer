using PathTracer.Pathtracing.Observers.Accumulators;
using PathTracer.Pathtracing.Observers.Cameras;

namespace PathTracer.Pathtracing.Observers;

public class Observer : IObserver {
    /// <summary> The <see cref="ICamera"/> of the <see cref="Observer"/> </summary>
    public ICamera Camera { get; }
    /// <summary> The <see cref="IAccumulator"/> of the <see cref="Observer"/> </summary>
    public IAccumulator Accumulator { get; protected set; }

    public Observer(ICamera camera, int imageWidth, int imageHeight) {
        Camera = camera;
        Accumulator = new Accumulator(imageWidth, imageHeight);
        Camera.AspectRatio = (float)Accumulator.Width / Accumulator.Height;
        Camera.OnMoved += (_, _) => Accumulator.Clear();
        Camera.Film.SampleRegistered += (_, sample) => Accumulator.Add(sample);
    }
}
