using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Observers.Cameras.Parts;

namespace PathTracer.Pathtracing.Rays {
    /// <summary> A ray sent from the camera into the scene </summary>
    public class CameraRay : Ray {
        /// <summary> The <see cref="IFilm"/> from which the <see cref="CameraRay"/> originates </summary>
        public IFilm Film { get; }
        /// <summary> How many times a BVH node is intersected </summary>
        public int BVHTraversals { get; set; } = 0;
        /// <summary> Whether the camera ray interacted with something </summary>
        public bool Intersection => Length != float.PositiveInfinity;

        /// <summary> Create a new camera ray </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="film">The <see cref="IFilm"/> from which this ray originates</param>
        public CameraRay(Position3 origin, Normal3 direction, IFilm film, float length = float.PositiveInfinity) : base(origin, direction, length) {
            Film = film;
        }
    }
}
