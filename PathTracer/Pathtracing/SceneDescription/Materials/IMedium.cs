using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> An <see cref="IMaterial"/> through which an <see cref="IRay"/> can travel </summary>
    public interface IMedium : ISurfaceMaterial, IComparable<IMedium>, IEquatable<IMedium> {
        /// <summary> The refractive index of this <see cref="IMedium"/> </summary>
        float RefractiveIndex { get; }
        /// <summary> The priority of this <see cref="IMedium"/> compared to others </summary>
        float Priority { get; }
    }
}
