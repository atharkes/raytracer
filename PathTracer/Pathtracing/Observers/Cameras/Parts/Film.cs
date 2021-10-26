using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;

namespace PathTracer.Pathtracing.Observers.Cameras.Parts {
    /// <summary> The screen plane object in the 3d scene </summary>
    public class Film : IFilm {
        /// <summary> The <see cref="IShape"/> of the <see cref="Film"/> </summary>
        public IShape Shape => Rectangle;
        /// <summary>  The <see cref="Rectangle"/> of the <see cref="Film"/> </summary>
        public Rectangle Rectangle { get; }
        /// <summary> The event that fires when a sample is registered </summary>
        public event EventHandler<ISample>? SampleRegistered;

        /// <summary> Create a <see cref="Film"/> </summary>
        /// <param name="rectangle">The <see cref="IShape"/> of the <see cref="Film"/></param>
        public Film(Rectangle rectangle) {
            Rectangle = rectangle;
        }

        /// <summary> Register a <paramref name="sample"/> to the <see cref="IFilm"/> </summary>
        /// <param name="sample">The <see cref="ISample"/> to register</param>
        public void RegisterSample(ISample sample) {
            SampleRegistered?.Invoke(this, sample);
        }

        /// <summary> Position the <see cref="IFilm"/> according to the <paramref name="camera"/> </summary>
        /// <param name="camera">The <see cref="ICamera"/> to position for</param>
        public void PositionFilm(object? _, ICamera camera) {
            Position1 distance = 0.5f * Rectangle.Height.Vector / ((float)Math.Tan(camera.FOV / 360 * Math.PI));
            Rectangle.Position = camera.Position + camera.ViewDirection * distance;
            Rectangle.Rotation = camera.Rotation;
        }
    }
}
