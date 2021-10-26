using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Geometry.Vectors {
    /// <summary> A dimensionless vector </summary>
    public interface IVector {
        /// <summary> The length of the <see cref="IVector"/> </summary>
        Vector1 Length { get; }
        /// <summary> The squared length of the <see cref="IVector"/> </summary>
        Vector1 LengthSquared { get; }

        /// <summary> Normalize the <see cref="IVector"/> </summary>
        /// <returns>The normalized <see cref="IVector"/></returns>
        IVector Normalized();
    }
}
