using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    public class SurfaceEmitter : ISurfaceEmitter {
        public ISpectrum Color { get; }
        public float Strength { get; }
        public float Roughness => 0f;

        public SurfaceEmitter(ISpectrum color, float strength) {
            Color = color;
            Strength = strength;
        }
    }
}
