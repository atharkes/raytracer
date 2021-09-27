namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> Defines a 3-dimensional <see cref="Shape"/> that encompasses a volume. </summary>
    public abstract class VolumetricShape : Shape {
        /// <summary> Whether the <see cref="VolumetricShape"/> has a volume </summary>
        public override bool Volumetric { get; } = true;
    }
}
