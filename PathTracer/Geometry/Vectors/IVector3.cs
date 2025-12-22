namespace PathTracer.Geometry.Vectors;

/// <summary> A 3-dimensional vector </summary>
public interface IVector3 : IVector {
    /// <summary> The X-component of the <see cref="IVector3"/> </summary>
    float X { get; }
    /// <summary> The Y-component of the <see cref="IVector3"/> </summary>
    float Y { get; }
    /// <summary> The Z-component of the <see cref="IVector3"/> </summary>
    float Z { get; }
}
