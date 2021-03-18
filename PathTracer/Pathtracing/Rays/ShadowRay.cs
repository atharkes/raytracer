namespace PathTracer.Pathtracing.Rays {
    public class ShadowRay : Ray {
        public ShadowRay(SurfacePoint origin, SurfacePoint destination, int recursionDepth) : base(origin, destination, recursionDepth) { }

        /// <summary> Trace the <see cref="Ray"/> through a <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="Scene"/> to trace through</param>
        /// <returns>An <see cref="RaySurfaceInteraction"/> if there is one</returns>
        public override SurfacePoint? Trace(Scene scene) {
            if (!scene.IntersectBool(this)) {
                return Destination!.SurfacePoint;
            } else {
                return null;
            }
        }
    }
}
