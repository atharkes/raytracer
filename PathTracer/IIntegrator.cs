using OpenTK.Mathematics;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.PDFs;

namespace PathTracer {
    /// <summary> An integrator to render a <see cref="IScene"/> </summary>
    public interface IIntegrator {
        //IPDF<Vector3> OriginGuider { get; }
        //IPDF<Vector3, Vector3> DirectionGuider { get; }
        //IPDF<IRay, float> DistanceGuider { get; }

        /// <summary> Integrate the <paramref name="scene"/> </summary>
        /// <param name="scene">The <see cref="IScene"/> to integrate</param>
        void Integrate(IScene scene);
    }
}
