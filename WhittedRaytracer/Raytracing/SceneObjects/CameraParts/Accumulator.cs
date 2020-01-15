using OpenTK;
using System;

namespace WhittedRaytracer.Raytracing.SceneObjects.CameraParts {
    /// <summary> An accumulator that accumulates the light of photons </summary>
    class Accumulator {
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
        public void DrawImage(IScreen screen) {
            Action[] tasks = new Action[Main.MultithreadingTaskCount];
            float size = Cavities.Length / tasks.Length;
            for (int i = 0; i < tasks.Length; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                tasks[i] = () => DrawImageRange(screen, lowerbound, higherbound);
            }
            Main.Threadpool.DoTasks(tasks);
            Main.Threadpool.WaitTillDone();
        }

        void DrawImageRange(IScreen screen, int from, int to) {
            for (int i = from; i < to; i++) {
                screen.Plot(i, Cavities[i].AverageLight.ToIntColor());
            }
        }

        /// <summary> Clear the light in the accumulator </summary>
        public void Clear() {
            foreach (Cavity cavity in Cavities) {
                cavity.Clear();
            }
        }

        /// <summary> Add a photon to a cavity in the accumulator </summary>
        /// <param name="x">The x coordinate of the cavity</param>
        /// <param name="y">The y coordinate of the cavity</param>
        /// <param name="photon">The photon to add to a cavity in the accumulator</param>
        public void AddPhoton(int x, int y, Vector3 photon) {
            Cavities[x + y * Width].AddPhoton(photon);
        }
    }
}
