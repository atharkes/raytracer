using OpenTK.Mathematics;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects {
    /// <summary> A virtual camera object that registers light transport in the <see cref="IScene"/> </summary>
    public interface ICamera {
        /// <summary> The default right vector when no rotation is applied </summary>
        static readonly Vector3 DefaultRight = Vector3.UnitX;
        /// <summary> The default up vector when no rotation is applied </summary>
        static readonly Vector3 DefaultUp = Vector3.UnitY;
        /// <summary> The default front vector when no rotation is applied </summary>
        static readonly Vector3 DefaultFront = Vector3.UnitZ;

        /// <summary> The position of the <see cref="ICamera"/> </summary>
        Vector3 Position { get; set; }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="ICamera"/> </summary>
        Quaternion Rotation { get; }
        /// <summary> The (horizontal) field of view of the <see cref="ICamera"/> (in degrees) </summary>
        float FOV { get; set; }

        /// <summary> The viewing direction of the <see cref="ICamera"/> </summary>
        Vector3 ViewDirection => Front;
        /// <summary> The front vector of the <see cref="ICamera"/> </summary>
        Vector3 Front => Rotation * DefaultFront;
        /// <summary> The back vector of the <see cref="ICamera"/> </summary>
        Vector3 Back => -Front;
        /// <summary> The up vector of the <see cref="ICamera"/> </summary>
        Vector3 Up => Rotation * DefaultUp;
        /// <summary> The down vector of the <see cref="ICamera"/></summary>
        Vector3 Down => -Up;
        /// <summary> The right vector of the <see cref="ICamera"/> </summary>
        Vector3 Right => Rotation * DefaultRight;
        /// <summary> The left vector of the <see cref="ICamera"/> </summary>
        Vector3 Left => -Right;

        /// <summary> Move the <see cref="ICamera"/> in a <paramref name="direction"/> </summary>
        /// <param name="direction">The direction in which to move the <see cref="ICamera"/></param>
        void Move(Vector3 direction);

        /// <summary> Rotate the <see cref="ICamera"/> around a specified <paramref name="axis"/> </summary>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        void Rotate(Vector3 axis, float degrees);

        /// <summary> Set the viewing direction of the <see cref="ICamera"/> </summary>
        /// <param name="direction">The new viewing direction of the <see cref="ICamera"/></param>
        void SetViewDirection(Vector3 direction);
    }
}
