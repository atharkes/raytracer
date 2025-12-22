using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.Observers.Cameras.Parts;

/// <summary> The film of the <see cref="ICamera"/> that registers the samples </summary>
public interface IFilm {
    /// <summary> The <see cref="IShape"/> used for the <see cref="IFilm"/> </summary>
    IShape Shape { get; }
    /// <summary> The event that fires when a sample is registered </summary>
    event EventHandler<Sample>? SampleRegistered;

    /// <summary> Register a <paramref name="sample"/> to the <see cref="IFilm"/> </summary>
    /// <param name="sample">The <see cref="ISample"/> to register</param>
    void RegisterSample(Sample sample);
}
