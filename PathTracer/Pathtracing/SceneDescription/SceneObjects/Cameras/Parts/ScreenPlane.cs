using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras.Parts {
    /// <summary> The screen plane object in the 3d scene </summary>
    public class ScreenPlane : Primitive {
        /// <summary> The rectangle <see cref="IShape"/> of the <see cref="ScreenPlane"/> </summary>
        public Rectangle Rectangle { get; }

        /// <summary> The 2d window the screen plane is linked to </summary>
        public readonly IScreen Screen;
        /// <summary> The accumulator that accumulates light </summary>
        public readonly Accumulator Accumulator;
        /// <summary> The aspect ratio of the screen plane </summary>
        public float AspectRatio => (float)Screen.Width / Screen.Height;

        /// <summary> The top left corner of the screen plane </summary>
        public Vector3 TopLeft => Rectangle.TopLeft;
        /// <summary> The top right corner of the screen plane </summary>
        public Vector3 TopRight => Rectangle.TopRight;
        /// <summary> The bottom left corner of the screen plane </summary>
        public Vector3 BottomLeft => Rectangle.BottomLeft;
        /// <summary> The bottom right corner of the screen plane </summary>
        public Vector3 BottomRight => Rectangle.BottomRight;

        /// <summary> Create a new screen plane linked to a camera </summary>
        /// <param name="camera">The camera to link the screen plane to</param>
        /// <param name="screen">The screen to draw the 2d projection to</param>
        public ScreenPlane(Rectangle rectangle, IScreen screen) : base(rectangle, ) {
            Rectangle = rectangle;
            Screen = screen;
            Accumulator = new Accumulator(screen?.Width ?? 0, screen?.Height ?? 0);
        }

        /// <summary> Get the position of a pixel on the screen plane in worldspace </summary>
        /// <param name="x">The x position of the pixel</param>
        /// <param name="y">The y position of the pixel</param>
        /// <returns>The position of the pixel in worldspace</returns>
        public Vector3 GetPixelPosition(int x, int y) {
            return TopLeft + (float)x / (Screen.Width - 1) * (TopRight - TopLeft) + (float)y / (Screen.Height - 1) * (BottomLeft - TopLeft);
        }
    }
}
