using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Observers.Cameras.Parts;

namespace PathTracer.Pathtracing.Rays;

/// <summary> A ray sent from the camera into the scene </summary>
public sealed class CameraRay : IRay {
    /// <summary> The <see cref="Ray"/> </summary>
    public Ray Ray { get; }

    public Position3 Origin => Ray.Origin;

    public Normal3 Direction => Ray.Direction;

    public Position1 Length => Ray.Length;

    /// <summary> The <see cref="IFilm"/> from which the <see cref="CameraRay"/> originates </summary>
    public IFilm Film { get; }
    /// <summary> How many BVH node bounding boxes the ray is tested against </summary>
    public int BVHTraversals { get; set; } = 0;
    /// <summary> Whether the camera ray interacted with something </summary>
    public int Intersections { get; set; } = 0;

    public bool Intersection => Intersections != 0;

    /// <summary> Create a new camera ray </summary>
    /// <param name="origin">The origin of the ray</param>
    /// <param name="direction">The direction of the ray</param>
    /// <param name="film">The <see cref="IFilm"/> from which this ray originates</param>
    public CameraRay(Position3 origin, Normal3 direction, IFilm film, float length = float.PositiveInfinity) {
        Ray = new(origin, direction, length);
        Film = film;
    }
}
