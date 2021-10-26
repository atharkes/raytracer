using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A 1-dimensional vector </summary>
    public interface IVector1 : IVector {
        /// <summary> The X-component of the <see cref="IVector1"/> </summary>
        Vector1 X { get; }

        IVector IVector.Normalized() => Normalized();
        new Vector1 Normalized();
    }
}
