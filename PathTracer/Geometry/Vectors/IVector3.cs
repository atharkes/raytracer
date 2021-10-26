using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 3-dimensional vector </summary>
    public interface IVector3 : IVector {
        /// <summary> The X-component of the <see cref="IVector3"/> </summary>
        Vector1 X { get; }
        /// <summary> The Y-component of the <see cref="IVector3"/> </summary>
        Vector1 Y { get; }
        /// <summary> The Z-component of the <see cref="IVector3"/> </summary>
        Vector1 Z { get; }

        IVector IVector.Normalized() => Normalized();
        new Vector3 Normalized();
    }
}
