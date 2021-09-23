using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> An <see cref="IMaterial"/> through which an <see cref="IRay"/> can travel </summary>
    public interface IMedium : IMaterial, IComparable<IMedium>, IEquatable<IMedium> {
        /// <summary> The refractive index of this <see cref="IMedium"/> </summary>
        double RefractiveIndex { get; }
        /// <summary> The priority of this <see cref="IMedium"/> compared to others </summary>
        double Priority { get; }
    }
}
