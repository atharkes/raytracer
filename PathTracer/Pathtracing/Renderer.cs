using PathTracer.Pathtracing.Integrators;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing {
    /// <summary> An <see cref="IRenderer"/> to render an <see cref="IScene"/> given an <see cref="IIntegrator"/> and an <see cref="IObserver"/> </summary>
    public class Renderer : IRenderer {
        /// <summary> The statistics of the <see cref="Renderer"/> </summary>
        public Statistics Statistics { get; } = new();

        /// <summary> The <see cref="IScene"/> to render </summary>
        public IScene Scene { get; }
        /// <summary> The <see cref="IIntegrator"/> to render the <see cref="IScene"/> with </summary>
        public IIntegrator Integrator { get; }
        /// <summary> The <see cref="IObserver"/> that views the <see cref="IScene"/> </summary>
        public IObserver Observer { get; }

        /// <summary> Create a new <see cref="Renderer"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to render</param>
        /// <param name="integrator">The <see cref="IIntegrator"/> to render the <paramref name="scene"/> with</param>
        /// <param name="observer">The <see cref="IObserver"/> to render the <paramref name="scene"/> to</param>
        public Renderer(IScene scene, IIntegrator integrator, IObserver observer) {
            Scene = scene;
            Integrator = integrator;
            Observer = observer;
        }

        /// <summary> Render the <see cref="Scene"/> </summary>
        /// <param name="renderTime">The <see cref="TimeSpan"/> to spent on rendering</param>
        public void Render(TimeSpan renderTime) {
            Statistics.LogFrameTime();
            Statistics.LogTaskTime(Statistics.OpenTKTime);
            // Integration
            TimeSpan integrationTime = renderTime - Statistics.OpenTKTime.LastTick - Statistics.DrawingTime.LastTick;
            int sampleCount = IntegrationSampleCount(integrationTime);
            Integrator.Integrate(Scene, sampleCount);
            Statistics.SampleCount += sampleCount;
            Statistics.SampleCountLastTick = sampleCount;
            Statistics.LogTaskTime(Statistics.IntegratorTime);
            // Drawing
            if (Observer is IInteractiveObserver interactiveObserver) {
                interactiveObserver.DrawFrame(Statistics);
            }
            Statistics.LogTaskTime(Statistics.DrawingTime);
        }

        int IntegrationSampleCount(TimeSpan integrationTime) {
            int sampleCount = 0;
            if (Statistics.IntegratorTime.LastTick.Ticks > 0) {
                TimeSpan previousTime = Statistics.IntegratorTime.LastTick;
                double sampleCountFactor = 1 + (integrationTime / previousTime - 1) * 0.1;
                sampleCount = (int)(sampleCountFactor * Statistics.SampleCountLastTick);
            }
            return Math.Max(BackwardsSampler.MinimumSampleCount, sampleCount);
        }
    }
}