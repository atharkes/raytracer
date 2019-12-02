using OpenTK;

namespace WhittedRaytracer {
    /// <summary> Extension methods for classes in OpenTK </summary>
    static class ExtensionMethods {
        /// <summary> Get the integer color from a vector3 color </summary>
        /// <param name="color">The vector3 color to get the integer color for</param>
        /// <returns>The integer color</returns>
        public static int ToIntColor(this Vector3 color) {
            color = Vector3.Clamp(color, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            int r = (int)(color.X * 255) << 16;
            int g = (int)(color.Y * 255) << 8;
            int b = (int)(color.Z * 255) << 0;
            return r + g + b;
        }
    }
}
