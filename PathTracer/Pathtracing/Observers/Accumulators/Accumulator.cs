using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.Observers.Accumulators;

/// <summary> An accumulator that accumulates the light of photons </summary>
public class Accumulator : IAccumulator {
    /// <summary> The width (in pixels) of the <see cref="Accumulator"/> </summary>
    public int Width { get; }
    /// <summary> The height (in pixels) of the <see cref="Accumulator"/> </summary>
    public int Height { get; }
    /// <summary> The amount of samples accumulated by the <see cref="Accumulator"/> </summary>
    public int SampleCount { get; protected set; }
    /// <summary> The average accumulated light in the <see cref="Accumulator"/> </summary>
    public RGBSpectrum AccumulatedRGB {
        get {
            var result = RGBColors.Black;
            foreach (var cavity in cavities) {
                result += cavity.AverageLight;
            }
            return result;
        }
    }

    private readonly Cavity[] cavities;

    /// <summary> Create a new accumulator </summary>
    /// <param name="pixelCount">The amount of cavities in the accumulator</param>
    public Accumulator(int width, int height) {
        Width = width;
        Height = height;
        cavities = new Cavity[width * height];
        for (var i = 0; i < cavities.Length; i++) {
            cavities[i] = new Cavity();
        }
    }

    /// <summary> Get the <see cref="Cavity"/> at the specified coordinates </summary>
    /// <param name="x">The x coordinate of the <see cref="Cavity"/></param>
    /// <param name="y">The y coordinate of the <see cref="Cavity"/></param>
    /// <returns>The <see cref="Cavity"/> at the specified coordinates</returns>
    public Cavity Get(int x, int y) => cavities[x + y * Width];

    /// <summary> Add a <paramref name="sample"/> to the <see cref="Accumulator"/> </summary>
    /// <param name="sample">The <see cref="Sample"/> to add.</param>
    public void Add(Sample sample) {
        var x = (int)(sample.Position.X * Width);
        var y = (int)(sample.Position.Y * Height);
        var index = y * Width + x;
        try {
            cavities[index].AddSample(sample.Light, sample.PrimaryBVHTraversals, sample.Intersection);
            SampleCount++;
        } catch (IndexOutOfRangeException) {
            Console.WriteLine($"Exception when adding a sample to the Accumulator. Has the screen be resized?");
        }
    }

    /// <summary> Draws the samples to the <paramref name="screen"/> </summary>
    /// <param name="screen">The <see cref="IScreen"/> to draw to</param>
    /// <param name="drawingMode">The <see cref="DrawingMode"/></param>
    public void DrawImage(IScreen screen, DrawingMode drawingMode) {
        var tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
        var size = (float)cavities.Length / tasks.Length;
        for (var i = 0; i < tasks.Length; i++) {
            var lowerbound = (int)(i * size);
            var higherbound = (int)((i + 1) * size);
            switch (drawingMode) {
                case DrawingMode.Light:
                    tasks[i] = () => { for (var i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].AverageLight.ToRGBInt()); };
                    break;
                case DrawingMode.BVHNodeTraversals:
                    tasks[i] = () => { for (var i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].AverageBVHTraversalColor.ToRGBInt()); };
                    break;
                case DrawingMode.Intersections:
                    tasks[i] = () => { for (var i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].IntersectionChanceColor.ToRGBInt()); };
                    break;
            }
        }
        Program.Threadpool.DoTasks(tasks);
        Program.Threadpool.WaitTillDone();
    }

    /// <summary> Clear the samples in the <see cref="Accumulator"/> </summary>
    public void Clear() {
        SampleCount = 0;
        foreach (var cavity in cavities) {
            cavity.Clear();
        }
    }

    public void Add(ISample sample) => throw new NotImplementedException();
}
