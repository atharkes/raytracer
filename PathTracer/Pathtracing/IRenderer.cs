using PathTracer.Pathtracing.Integrators;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;

namespace PathTracer.Pathtracing;

/// <summary> A renderer that uses an <see cref="IIntegrator"/> to render a <see cref=""/>. </summary>
public interface IRenderer {
    /// <summary> The <see cref="IScene"/> to be rendered </summary>
    IScene Scene { get; }
    /// <summary> The <see cref="IIntegrator"/> to render the scene </summary>
    IIntegrator Integrator { get; }
    /// <summary> The <see cref="IObserver"/> that views the scene </summary>
    IObserver Observer { get; }

    /// <summary> Render the <see cref="Scene"/> </summary>
    /// <param name="renderTime">The <see cref="TimeSpan"/> to spent on rendering</param>
    void Render(TimeSpan renderTime);
}
