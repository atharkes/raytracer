using OpenTK.Mathematics;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.Paths;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;

namespace PathTracer {
    /// <summary> Main class of the raytracer </summary>
    public class Renderer : IRenderer {
        /// <summary> The <see cref="IScene"/> to render </summary>
        public IScene Scene { get; }
        /// <summary> The <see cref="IIntegrator"/> to render the <see cref="IScene"/> with </summary>
        public IIntegrator Integrator { get; }
        /// <summary> The <see cref="IObserver"/> that views the <see cref="IScene"/> </summary>
        public IObserver Observer { get; }
        /// <summary> The statistics of the <see cref="Renderer"/> </summary>
        public Statistics Statistics { get; } = new();

        public Renderer(IScene scene, IIntegrator integrator, IObserver observer) {
            Scene = scene;
            Integrator = integrator;
            Observer = observer;
        }

        public void Render(IScene scene) {
            Statistics.LogFrameTime();
            Statistics.LogTaskTime(Statistics.OpenTKTime);
            int rayCount = scene.Camera.RayCountNextTick();
            Statistics.LogTickRays(rayCount);
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            float size = rayCount / Program.Threadpool.MultithreadingTaskCount;
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                int lowerbound = (int)(i * size);
                int higherbound = (int)((i + 1) * size);
                tasks[i] = () => TraceRays(scene, lowerbound, higherbound);
            }
            Statistics.LogTaskTime(Statistics.MultithreadingOverhead);
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
            Statistics.LogTaskTime(Statistics.TracingTime);
            Observer.UpdateWindow();
            Statistics.LogTaskTime(Statistics.DrawingTime);
        }

        void TraceRays(IScene scene, int from, int to) {
            ICollection<Ray> rays = Guider.Samples(scene, to - from, Utils.Random);
            foreach(Ray ray in rays) {
                Vector3 pixelColor = Sample(scene, ray);
                if (ray is CameraRay cameraRay) {
                    cameraRay.Cavity.AddSample(pixelColor, cameraRay.BVHTraversals, cameraRay.Intersection);
                }
            }
        }
    }
}