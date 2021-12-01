using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 2-dimensional vector </summary>
    public interface IVector2 : IVector {
        /// <summary> The X-component of the <see cref="IVector2"/> </summary>
        float X { get; }
        /// <summary> The Y-component of the <see cref="IVector2"/> </summary>
        float Y { get; }
    }
}
