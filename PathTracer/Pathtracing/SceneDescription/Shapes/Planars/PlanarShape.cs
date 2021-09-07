namespace PathTracer.Pathtracing.SceneDescription.Shapes.Planars {
    /// <summary> A <see cref="Shape"/> that has no volume </summary>
    public abstract class PlanarShape : Shape, IPlanarShape {
        /// <summary> Whether the <see cref="PlanarShape"/> has a volume </summary>
        public override bool Volumetric { get; } = false;
        /// <summary> The volume of the <see cref="PlanarShape"/> </summary>
        public override float Volume { get; } = 0F;
    }
}
