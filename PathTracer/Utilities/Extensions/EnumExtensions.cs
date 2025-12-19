namespace PathTracer.Utilities.Extensions;

/// <summary> Extension methods for <see cref="Enum"/> </summary>
public static class EnumExtensions {
    /// <summary> Get the next <paramref name="value"/> of a <see cref="Enum"/> </summary>
    /// <typeparam name="T">The type of <see cref="Enum"/></typeparam>
    /// <param name="value">The current value of the <see cref="Enum"/></param>
    /// <returns>The next <paramref name="value"/> of the <see cref="Enum"/></returns>
    public static T Next<T>(this T value) where T : Enum {
        var values = (T[])Enum.GetValues(value.GetType());
        var index = (Array.IndexOf(values, value) + 1) % values.Length;
        return values[index];
    }

    /// <summary> Get the previous <paramref name="value"/> of a <see cref="Enum"/> </summary>
    /// <typeparam name="T">The type of the <see cref="Enum"/></typeparam>
    /// <param name="value">The current value of the <see cref="Enum"/></param>
    /// <returns>The previous <paramref name="value"/> of the <see cref="Enum"/></returns>
    public static T Previous<T>(this T value) where T : Enum {
        var values = (T[])Enum.GetValues(value.GetType());
        var index = (Array.IndexOf(values, value) - 1) % values.Length;
        return values[index];
    }
}
