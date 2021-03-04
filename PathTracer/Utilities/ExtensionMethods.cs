using OpenTK.Mathematics;
using System;

namespace PathTracer.Utilities {
    /// <summary> Extension methods for classes in OpenTK </summary>
    public static class ExtensionMethods {
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

        /// <summary> Get the next value of a <paramref name="current"/> <see cref="Enum"/> </summary>
        /// <typeparam name="T">The type of the <paramref name="current"/> <see cref="Enum"/></typeparam>
        /// <param name="current">The value of the <see cref="Enum"/> to get the next for</param>
        /// <returns>The next value of the <paramref name="current"/> <see cref="Enum"/></returns>
        public static T Next<T>(this T current) where T : Enum {
            T[] values = (T[])Enum.GetValues(current.GetType());
            int index = Array.IndexOf(values, current) + 1;
            return (values.Length == index) ? values[0] : values[index];
        }
    }
}
