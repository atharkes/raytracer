using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Pathtracing.Paths {
    /// <summary> A path </summary>
    public interface IPath {
        /// <summary> The origin of the <see cref="IPath"/> </summary>
        IPathSegment Origin { get; }
    }
}
