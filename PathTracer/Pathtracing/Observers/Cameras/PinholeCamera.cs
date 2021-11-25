using OpenTK.Mathematics;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Observers.Cameras.Parts;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;

namespace PathTracer.Pathtracing.Observers.Cameras {
    /// <summary> An <see cref="ICamera"/> scene object </summary>
    public class PinholeCamera : ICamera {
        /// <summary> The <see cref="IFilm"/> of the <see cref="ICamera"/> </summary>
        IFilm ICamera.Film => Film;
        /// <summary> The <see cref="IFilm"/> of the <see cref="PinholeCamera"/> </summary>
        public Film Film { get; }
        /// <summary> The position of the <see cref="PinholeCamera"/> </summary>
        public Position3 Position { get => position; set => SetPosition(value); }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="PinholeCamera"/> </summary>
        public Quaternion Rotation { get => rotation; set => SetRotation(value); }
        /// <summary> The field of view angle (in degrees) the <see cref="PinholeCamera"/> </summary>
        public float HorizontalFOV { get => fov; set => SetFOV(value); }
        /// <summary> The aspect ratio of the <see cref="PinholeCamera"/> </summary>
        public float AspectRatio { get => aspectRatio; set => SetAspectRatio(value); }
        /// <summary> The viewing direction of the <see cref="PinholeCamera"/> </summary>
        public Normal3 ViewDirection => Rotation * IDirection3.DefaultFront;   

        /// <summary> The event that fires when the <see cref="Camera"/> moved </summary>
        public event EventHandler<ICamera>? OnMoved;

        Position3 position;
        Quaternion rotation;
        float fov;
        float aspectRatio;

        /// <summary> Create a new camera object </summary>
        /// <param name="position">The position of the <see cref="PinholeCamera"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="PinholeCamera"/></param>
        /// <param name="aspectRatio">The aspect ratio of the <see cref="PinholeCamera"/></param>
        /// <param name="fov">The field of view of the <see cref="PinholeCamera"/></param>
        public PinholeCamera(Position3 position, Quaternion rotation, float aspectRatio, float fov) {
            this.position  = position;
            this.rotation = rotation;
            this.fov = fov;
            this.aspectRatio = aspectRatio;
            Rectangle filmRectangle = new(Position, new Position2(aspectRatio, 1), rotation);
            Film = new Film(filmRectangle);
            OnMoved += Film.PositionFilm;
            OnMoved?.Invoke(this, this);
        }

        /// <summary> Move the <see cref="PinholeCamera"/> in a <paramref name="direction"/> </summary>
        /// <param name="direction">The direction in which to move the <see cref="PinholeCamera"/></param>
        public void Move(IDirection3 direction) => SetPosition(position + direction);

        /// <summary> Set the position of the <see cref="PinholeCamera"/> </summary>
        /// <param name="position">The new position of the <see cref="PinholeCamera"/></param>
        public void SetPosition(Position3 position) {
            this.position = position;
            OnMoved?.Invoke(this, this);
        }

        /// <summary> Rotate the <see cref="PinholeCamera"/> around a specified <paramref name="axis"/> </summary>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        public void Rotate(Normal3 axis, float degrees) => SetRotation(Rotation * Quaternion.FromAxisAngle(axis.Vector.Value, degrees));

        public void Rotate(Quaternion quaternion) => SetRotation(Rotation * quaternion);

        /// <summary> Set the rotation <see cref="Quaternion"/> of the <see cref="PinholeCamera"/> </summary>
        /// <param name="rotation">The new rotation <see cref="Quaternion"/></param>
        public void SetRotation(Quaternion rotation) {
            this.rotation = rotation;
            OnMoved?.Invoke(this, this);
        }

        /// <summary> Set the view direction of the <see cref="PinholeCamera"/> </summary>
        /// <param name="direction">The direction the <see cref="PinholeCamera"/> will view towards</param>
        public void SetViewDirection(Normal3 direction) {
            Rotate(new Quaternion(Vector3.Cross(ViewDirection.Vector, direction.Vector), Vector3.Dot(ViewDirection.Vector, direction.Vector) + 1).Normalized());
        }

        /// <summary> Set the field of view of the <see cref="PinholeCamera"/> </summary>
        /// <param name="degrees">The new field of view</param>
        public void SetFOV(float degrees) {
            fov = degrees;
            OnMoved?.Invoke(this, this);
        }

        /// <summary> Set the aspect ratio of the <see cref="PinholeCamera"/> </summary>
        /// <param name="aspectRatio">The new aspect ratio</param>
        public void SetAspectRatio(float aspectRatio) {
            this.aspectRatio = aspectRatio;
            OnMoved?.Invoke(this, this);
        }

        /// <summary> Get a <see cref="CameraRay"/> from the <see cref="PinholeCamera"/> </summary>
        /// <param name="position">The position values between 0 and 1 to decide position on the film</param>
        /// <param name="direction">The direction values between 0 and 1 to decide the direction from the film</param>
        /// <returns>A <see cref="CameraRay"/> corresponding to the <paramref name="position"/> and <paramref name="direction"/></returns>
        public CameraRay GetCameraRay(Position2 position, Direction2 direction) {
            Position3 rayOrigin = Film.Rectangle.SurfacePosition(position);
            Normal3 rayDirection = new(rayOrigin - Position);
            return new CameraRay(rayOrigin, rayDirection, Film);
        }
    }
}