using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.Paths;

/// <summary> An interface for a segment of an <see cref="IPath"/> </summary>
public interface IPathSegment {
    /// <summary> The predecessor of this <see cref="IPathSegment"/> (if it's not a root node) </summary>
    IPathSegment? Predecessor { get; }
    /// <summary> Whether this <see cref="IPathSegment"/> has a <see cref="Predecessor"/> or is a root </summary>
    bool Root => Predecessor is null;
    /// <summary> The successors of this <see cref="IPathSegment"/> (if there are any) </summary>
    IEnumerable<IPathSegment> Successors { get; }
    /// <summary> The neighbours of this <see cref="IPathSegment"/> </summary>
    IEnumerable<IPathSegment> Neighbors { get; }

    /// <summary> The (ir)radiance (estimate) at this <see cref="IPathSegment"/> </summary>
    ISpectrum AccumulatedEmittance { get; }
    /// <summary> The camera importance (estimate) at this <see cref="IPathSegment"/> </summary>
    ISpectrum AccumulatedImportance { get; }

    /// <summary> The emitted (ir)radiance at this <see cref="IPathSegment"/> </summary>
    ISpectrum Emittance { get; }
    /// <summary> The sensor absorption at this <see cref="IPathSegment"/> </summary>
    ISpectrum Importance { get; }
    /// <summary> The absorption occuring at this <see cref="IPathSegment"/> </summary>
    ISpectrum Absorption { get; }
}
