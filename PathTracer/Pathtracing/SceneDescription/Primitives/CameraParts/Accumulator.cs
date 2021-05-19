using OpenTK.Mathematics;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.SceneDescription.CameraParts {
    /// <summary> The drawing mode of the accumulator </summary>
    public enum DrawingMode {
        Light,
        BVHNodeTraversals,
        Intersections,
    }

    /// <summary> An accumulator that accumulates the light of photons </summary>
    public class Accumulator {
        /// <summary> The cavities in which the light is accumulated </summary>
        public Cavity[] Cavities { get; }
        /// <summary> The width of the accumulator </summary>
        public int Width { get; }
        /// <summary> The height of the accumulator </summary>
        public int Height { get; }

        /// <summary> Create a new accumulator </summary>
        /// <param name="width">The width of the accumulator</param>
        /// <param name="height">The height of the accumulator</param>
        public Accumulator(int width, int height) {
            Width = width;
            Height = height;
            Cavities = new Cavity[width * height];
            for (int i = 0; i < Cavities.Length; i++) {
                Cavities[i] = new Cavity();
            }
        }

        /// <summary> Draws an image from the photons in the accumulator to a screen </summary>
        /// <param name="screen">The screen to draw the image to</param>
        public void DrawImage(IScreen screen, DrawingMode drawingMode) {
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = Cavities.Length / tasks.Length;
            for (int i = 0; i < tasks.Length; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                switch (drawingMode) {
                    case DrawingMode.Light:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, Cavities[i].AverageLight.ToIntColor()); };
                        break;
                    case DrawingMode.BVHNodeTraversals:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, Cavities[i].AverageBVHTraversalColor.ToIntColor()); };
                        break;
                    case DrawingMode.Intersections:
                        tasks[i] = () => { for (int i = lowerbound; i < higherbound; i++) screen.Plot(i, Cavities[i].IntersectionChanceColor.ToIntColor()); };
                        break;
                }
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        /// <summary> Get the total average light in the accumulator </summary>
        /// <returns>The total average light in the accumulator</returns>
        public Vector3 AverageLight() {
            Vector3 averageLight = Vector3.Zero;
            foreach (Cavity cavity in Cavities) {
                averageLight += cavity.AverageLight;
            }
            return averageLight;
        }

        /// <summary> Clear the light in the accumulator </summary>
        public void Clear() {
            foreach (Cavity cavity in Cavities) {
                cavity.Clear();
            }
        }
    }
}
