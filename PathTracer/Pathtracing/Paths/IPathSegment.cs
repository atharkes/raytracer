using PathTracer.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.Paths {
    public interface IPathSegment {
        /// <summary> The predecessor of this <see cref="IPathSegment"/> (if it's not a root node) </summary>
        IPathSegment? Predecessor { get; }
        /// <summary> Whether this <see cref="IPathSegment"/> has a <see cref="Predecessor"/> or is a root </summary>
        bool Root => Predecessor is null;
        /// <summary> The successors of this <see cref="IPathSegment"/> (if there are any) </summary>
        IEnumerable<IPathSegment> Successors { get; }
        
        ISpectrum Radiance { get; }
        ISpectrum Importance { get; }
        ISpectrum Absorption { get; }
    }
}
