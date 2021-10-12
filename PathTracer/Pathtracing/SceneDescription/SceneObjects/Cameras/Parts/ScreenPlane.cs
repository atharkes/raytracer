using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras.Parts {
    /// <summary> The screen plane object in the 3d scene </summary>
    public class ScreenPlane : Primitive {
        /// <summary> The rectangle <see cref="IShape"/> of the <see cref="ScreenPlane"/> </summary>
        public Rectangle Rectangle { get; }
        /// <summary> The accumulator that accumulates light </summary>
        public Accumulator Accumulator { get; }

        /// <summary> Create a new screen plane linked to a camera </summary>
        /// <param name="camera">The camera to link the screen plane to</param>
        /// <param name="screen">The screen to draw the 2d projection to</param>
        public ScreenPlane(Rectangle rectangle, int pixelCount) : base(rectangle, ) {
            Rectangle = rectangle;
            Accumulator = new Accumulator(pixelCount);
        }

        /// <summary> Get a specific position on the screenplane </summary>
        /// <param name="x">The x value between 0 and 1</param>
        /// <param name="y">The y value between 0 and 1</param>
        /// <returns>The position on the screenplane in worldspace</returns>
        public Vector3 GetPixelPosition(float x, float y) {
            return Rectangle.BottomLeft + x * Rectangle.LeftToRight + y * Rectangle.BottomToTop;
        }
    }
}
