using OpenTK.Mathematics;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Observers.Cameras.Parts;
using PathTracer.Pathtracing.Rays;
using System;

namespace PathTracer.Pathtracing.Observers.Cameras {
    /// <summary> A virtual camera object that registers light transport in the <see cref="IScene"/> </summary>
    public interface ICamera {
        /// <summary> The <see cref="IFilm"/> of the <see cref="ICamera"/> </summary>
        IFilm Film { get; }
        /// <summary> The position of the <see cref="ICamera"/> </summary>
        Position3 Position { get; set; }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="ICamera"/> </summary>
        Quaternion Rotation { get; }
        /// <summary> The (horizontal) field of view of the <see cref="ICamera"/> (in degrees) </summary>
        float FOV { get; set; }

        /// <summary> The viewing direction of the <see cref="ICamera"/> </summary>
        Normal3 ViewDirection => Front;
        /// <summary> The front vector of the <see cref="ICamera"/> </summary>
        Normal3 Front => Rotation * IDirection3.DefaultFront;
        /// <summary> The back vector of the <see cref="ICamera"/> </summary>
        Normal3 Back => -Front;
        /// <summary> The up vector of the <see cref="ICamera"/> </summary>
        Normal3 Up => Rotation * IDirection3.DefaultUp;
        /// <summary> The down vector of the <see cref="ICamera"/></summary>
        Normal3 Down => -Up;
        /// <summary> The right vector of the <see cref="ICamera"/> </summary>
        Normal3 Right => Rotation * IDirection3.DefaultRight;
        /// <summary> The left vector of the <see cref="ICamera"/> </summary>
        Normal3 Left => -Right;

        /// <summary> The event that fires when the <see cref="ICamera"/> moved </summary>
        event EventHandler<ICamera>? OnMoved;

        /// <summary> Move the <see cref="ICamera"/> in a <paramref name="direction"/> </summary>
        /// <param name="direction">The direction in which to move the <see cref="ICamera"/></param>
        void Move(IDirection3 direction);

        /// <summary> Rotate the <see cref="ICamera"/> around a specified <paramref name="axis"/> </summary>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        void Rotate(Normal3 axis, float degrees);

        /// <summary> Set the viewing direction of the <see cref="ICamera"/> </summary>
        /// <param name="direction">The new viewing direction of the <see cref="ICamera"/></param>
        void SetViewDirection(Normal3 direction);

        /// <summary> Get a <see cref="CameraRay"/> from the <see cref="ICamera"/> </summary>
        /// <param name="position">The position values between 0 and 1 to decide position on the film</param>
        /// <param name="direction">The direction values between 0 and 1 to decide the direction from the film</param>
        /// <returns>A <see cref="CameraRay"/> corresponding to the <paramref name="position"/> and <paramref name="direction"/></returns>
        CameraRay GetCameraRay(Position2 position, Direction2 direction);
    }
}
