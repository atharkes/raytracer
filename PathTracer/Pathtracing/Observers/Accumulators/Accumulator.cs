using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.Observers.Accumulators {
    /// <summary> An accumulator that accumulates the light of photons </summary>
    public class Accumulator : IAccumulator {
        /// <summary> The width (in pixels) of the <see cref="Accumulator"/> </summary>
        public int Width { get; }
        /// <summary> The height (in pixels) of the <see cref="Accumulator"/> </summary>
        public int Height { get; }
        /// <summary> The amount of samples accumulated by the <see cref="Accumulator"/> </summary>
        public int SampleCount { get; protected set; }
        /// <summary> The average accumulated light in the <see cref="Accumulator"/> </summary>
        public ISpectrum AccumulatedLight {
            get {
                ISpectrum averageLight = ISpectrum.Black;
                foreach (Cavity cavity in cavities) {
                    if (!cavity.AverageLight.IsBlack) {
                        averageLight += cavity.AverageLight;
                    }
                }
                return averageLight;
            }
        }

        readonly Cavity[] cavities;

        /// <summary> Create a new accumulator </summary>
        /// <param name="pixelCount">The amount of cavities in the accumulator</param>
        public Accumulator(int width, int height) {
            Width = width;
            Height = height;
            cavities = new Cavity[width * height];
            for (int i = 0; i < cavities.Length; i++) {
                cavities[i] = new Cavity();
            }
        }

        /// <summary> Add a <paramref name="sample"/> to the <see cref="Accumulator"/> </summary>
        /// <param name="sample">The <see cref="ISample"/> to add</param>
        public void Add(ISample sample) {
            int x = (int)(sample.Position.X * Width);
            int y = (int)(sample.Position.Y * Height);
            cavities[y * Width + x].AddSample(sample.Light, sample.PrimaryBVHTraversals, sample.Intersection);
            SampleCount++;
        }

        /// <summary> Draws the samples to the <paramref name="screen"/> </summary>
        /// <param name="screen">The <see cref="IScreen"/> to draw to</param>
        /// <param name="drawingMode">The <see cref="DrawingMode"/></param>
        public void DrawImage(IScreen screen, DrawingMode drawingMode) {
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = cavities.Length / tasks.Length;
            for (int i = 0; i < tasks.Length; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                switch (drawingMode) {
                    case DrawingMode.Light:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].AverageLight.ToRGBInt()); };
                        break;
                    case DrawingMode.BVHNodeTraversals:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].AverageBVHTraversalColor.ToRGBInt()); };
                        break;
                    case DrawingMode.Intersections:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, cavities[i].IntersectionChanceColor.ToRGBInt()); };
                        break;
                }
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        /// <summary> Clear the samples in the <see cref="Accumulator"/> </summary>
        public void Clear() {
            SampleCount = 0;
            foreach (Cavity cavity in cavities) {
                cavity.Clear();
            }
        }
    }
}
