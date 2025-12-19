namespace PathTracer.Pathtracing.SceneDescription.Shapes;

/// <summary> A volumetric <see cref="IShape"/> </summary>
public interface IVolumetricShape : IShape {
    /// <summary> An <see cref="IVolumetricShape"/> has a volume </summary>
    bool IShape.Volumetric => true;
}
