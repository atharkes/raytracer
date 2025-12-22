namespace PathTracer.Pathtracing.Paths;

/// <summary> A path </summary>
public interface IPath {
    /// <summary> The origin of the <see cref="IPath"/> </summary>
    IPathSegment Origin { get; }
}
