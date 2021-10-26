using PathTracer.Geometry.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Geometry.Positions {
    /// <summary> A dimensionless position <see cref="IVector"/> </summary>
    /// <typeparam name="T">The <see cref="IVector"/> determining the dimension of the <see cref="IPosition{T}"/></typeparam>
    public interface IPosition<T> where T : IVector {
        /// <summary> The <see cref="IVector"/> used for this <see cref="IPosition{T}"/> </summary>
        T Vector { get; }
        /// <summary> The distance from the origin to the <see cref="IPosition{T}"/> </summary>
        Position1 DistanceFromOrigin => Vector.Length;
    }
}
