using OpenTK.Mathematics;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.SceneObjects.CameraParts {
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
        public void DrawImage(IScreen screen, bool drawBVH) {
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = Cavities.Length / tasks.Length;
            for (int i = 0; i < tasks.Length; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                if (drawBVH) tasks[i] = () => DrawBVHRange(screen, lowerbound, higherbound);
                else tasks[i] = () => DrawImageRange(screen, lowerbound, higherbound);
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        void DrawImageRange(IScreen screen, int from, int to) {
            for (int i = from; i < to; i++) screen.Plot(i, Cavities[i].AverageLight.ToIntColor());
        }

        void DrawBVHRange(IScreen screen, int from, int to) {
            for (int i = from; i < to; i++) screen.Plot(i, Cavities[i].AverageBVHTraversalColor.ToIntColor());
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
