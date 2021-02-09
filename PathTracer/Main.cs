using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Multithreading;
using PathTracer.Raytracing;
using PathTracer.Raytracing.SceneObjects.CameraParts;
using System;

namespace PathTracer {
    /// <summary> Main class of the raytracer </summary>
    class Main {
        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new Threadpool();
        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public readonly Scene Scene;

        /// <summary> Create a raytracing application </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        public Main(IScreen screen) {
            Scene = Scene.DefaultWithRandomTriangles(screen, 1_000);
        }

        /// <summary> Process a single frame </summary>
        public void Tick() {
            Scene.Camera.Statistics.LogFrameTime();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.OpenTKTime);
            int rayCount = Scene.Camera.RayCountNextTick();
            Scene.Camera.Statistics.LogTickRays(rayCount);
            Action[] tasks = new Action[Threadpool.MultithreadingTaskCount];
            float size = rayCount / Threadpool.MultithreadingTaskCount;
            for (int i = 0; i < Threadpool.MultithreadingTaskCount; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                tasks[i] = () => TraceRays(lowerbound, higherbound);
            }
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.MultithreadingOverhead);
            Threadpool.DoTasks(tasks);
            Threadpool.WaitTillDone();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.TracingTime);
            Scene.Camera.ScreenPlane.Draw();
            Scene.Camera.Statistics.LogTaskTime(Scene.Camera.Statistics.DrawingTime);
        }

        void TraceRays(int from, int to) {
            CameraRay[] rays = Scene.Camera.GetRandomCameraRays(to - from);
            for (int i = 0; i < rays.Length; i++) {
                int x = i % Scene.Camera.ScreenPlane.Screen.Width;
                int y = i / Scene.Camera.ScreenPlane.Screen.Width;
                Vector3 pixelColor = Scene.CastRay(rays[i], 0);
                rays[i].Cavity.AddPhoton(pixelColor, rays[i].BVHTraversals);
            }
        }
    }
}