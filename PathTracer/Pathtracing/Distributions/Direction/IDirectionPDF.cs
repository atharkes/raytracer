using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Materials;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public interface IDirectionMedium {
        Vector3 Direction { get; }
        IMedium Medium { get; }
    }
    public interface IDirectionDistribution : IPDF<IDirectionMedium> { }
}
