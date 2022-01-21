using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;

namespace PathTracer.Pathtracing.Observers {
    /// <summary> An interface for a 2d screen used for displaying the 3d scene using raytracing </summary>
    public interface IScreen {
        /// <summary> The bounds of the <see cref="IScreen"/> </summary>
        Box2i Bounds => new(Location.X, Location.Y, Size.X, Size.Y);
        /// <summary> The position of the <see cref="IScreen"/> on the display </summary>
        Vector2i Location { get; }
        /// <summary> The size of the <see cref="IScreen"/> </summary>
        Vector2i Size { get; }
        /// <summary> The width of the <see cref="IScreen"/> </summary>
        int Width => Size.X;
        /// <summary> The height of the <see cref="IScreen"/> </summary>
        int Height => Size.Y;

        /// <summary> An event that fires when the <see cref="IScreen"/> is resized </summary>
        event Action<ResizeEventArgs> Resize;

        /// <summary> Clear the screen </summary>
        /// <param name="color">The color to clear the screen width</param>
        void Clear(int color = 0x000000);

        /// <summary> Plot a pixel on the screen </summary>
        /// <param name="x">The x position to plot</param>
        /// <param name="y">The y position to plot</param>
        /// <param name="color">The color to plot at the position</param>
        void Plot(int x, int y, int color = 0xffffff);

        /// <summary> Plot a pixel on the screen </summary>
        /// <param name="i">The position of the pixel in the pixel array</param>
        /// <param name="color">The color to plot at the position</param>
        void Plot(int i, int color = 0xffffff);

        /// <summary> Draw a line on the screen </summary>
        /// <param name="x1">The x position of the first point</param>
        /// <param name="y1">The y position of the first point</param>
        /// <param name="x2">The x position of the second point</param>
        /// <param name="y2">The y position of the second point</param>
        /// <param name="color">The color of the line</param>
        void Line(int x1, int y1, int x2, int y2, int color = 0xffffff);

        /// <summary> Draw a box on the screen </summary>
        /// <param name="x1">The x position of the first corner</param>
        /// <param name="y1">The y position of the first corner</param>
        /// <param name="x2">The x position of the second corner</param>
        /// <param name="y2">The y position of the second corner</param>
        /// <param name="color">The color of the box</param>
        void Box(int x1, int y1, int x2, int y2, int color = 0xffffff);

        /// <summary> Print text on the screen </summary>
        /// <param name="text">The text to print on the screen</param>
        /// <param name="x">The x position of the topleft of the text</param>
        /// <param name="y">The y position of the topleft of the text</param>
        /// <param name="color">The color of the text</param>
        void Print(string text, int x, int y, int color = 0xffffff);
    }
}
