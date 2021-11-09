using System;

namespace PathTracer.Utilities {
    /// <summary> Extension methods for classes in OpenTK </summary>
    public static class ExtensionMethods {
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
