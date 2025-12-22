using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Utilities;

/// <summary> Just some usefull static methods </summary>
public static class Utils {
    /// <summary> A deterministic <see cref="Random"/> to generate random scenes which are always the same </summary>
    public static Random DeterministicRandom { get; } = new Random(0);
    /// <summary> The <see cref="Random"/> to use per thread </summary>
    public static Random ThreadRandom => Random.Shared;

    /// <summary> Get the minimum of two <see cref="IComparable{T}"/> </summary>
    /// <typeparam name="T">The type of the objects</typeparam>
    /// <param name="first">The first <see cref="IComparable{T}"/></param>
    /// <param name="second">The second <see cref="IComparable{T}"/></param>
    /// <returns>The minimum <see cref="IComparable{T}"/></returns>
    public static T Min<T>(T first, T second) where T : IComparable<T> => first.CompareTo(second) <= 0 ? first : second;

    /// <summary> Get the maximum of two <see cref="IComparable{T}"/> </summary>
    /// <typeparam name="T">The type of the objects</typeparam>
    /// <param name="first">The first <see cref="IComparable{T}"/></param>
    /// <param name="second">The second <see cref="IComparable{T}"/></param>
    /// <returns>The maximum <see cref="IComparable{T}"/></returns>
    public static T Max<T>(T first, T second) where T : IComparable<T> => first.CompareTo(second) >= 0 ? first : second;

    /// <summary> Get a color scale for a value from black to green to yellow to red </summary>
    /// <param name="value">The value to get the color for</param>
    /// <param name="min">The minimum value of the scale</param>
    /// <param name="max">The maximum value of the scale</param>
    /// <returns>A color of the color scale</returns>
    public static RGBSpectrum ColorScaleBlackGreenYellowRed(float value, float min, float max) {
        const int transitions = 3;
        var range = max - min;
        var transition1 = range / transitions;
        var green = value < transition1 ? value / transition1 : transitions - value / transition1;
        var red = (value - transition1) / transition1;
        return new RGBSpectrum(red, green, 0);
    }
}
