using PathTracer.Pathtracing.SceneDescription.Shapes;

namespace PathTracer.Pathtracing.SceneDescription {
    public interface IDivisible {
        (IDivisible left, IDivisible right) Divide(AxisAlignedPlane plane);
    }
}
