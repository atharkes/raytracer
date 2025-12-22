namespace PathTracer.Utilities.Extensions;

/// <summary> Extension methods for <see cref="float"/> </summary>
public static class SingleExtensions {
    /// <summary> The minimum <see cref="int"/> that can represent a <see cref="float"/>-index </summary>
    public static readonly int MinIndex = ToIndex(float.NegativeInfinity);
    /// <summary> The maximum <see cref="int"/> that can represent a <see cref="float"/>-index </summary>
    public static readonly int MaxIndex = ToIndex(float.PositiveInfinity);

    /// <summary> Convert a <paramref name="value"/> to a <see cref="float"/>-index </summary>
    /// <param name="value">the <see cref="float"/> to convert to a <see cref="float"/>-index </param>
    /// <returns>An <see cref="int"/> representing the <see cref="float"/>-index of the <paramref name="value"/></returns>
    /// <exception cref="ArgumentException"><see cref="float.Nan"/> does not have a valid <see cref="float"/>-index representation</exception>
    public static int ToIndex(this float value) {
        if (float.IsNaN(value)) {
            throw new ArgumentException($"{value} does not have a sensical index representation");
        } else {
            var bits = BitConverter.SingleToInt32Bits(value);
            return bits < 0 ? (bits ^ int.MaxValue) + 1 : bits;
        }
    }

    /// <summary> Convert a <see cref="float"/>-<paramref name="index"/> to a <see cref="float"/> </summary>
    /// <param name="index">The <see cref="int"/> representing the <see cref="float"/>-index</param>
    /// <returns>The <see cref="float"/> according to the <see cref="float"/>-<paramref name="index"/></returns>
    /// <exception cref="ArgumentException">Not all <see cref="int"/> values are valid <see cref="float"/>-indices</exception>
    public static float FromIndex(this int index) {
        if (index < MinIndex || index > MaxIndex) {
            throw new ArgumentException($"{index} does not have a sensical floating point representation");
        } else {
            var bits = index < 0 ? (index - 1) ^ int.MaxValue : index;
            return BitConverter.Int32BitsToSingle(bits);
        }
    }

    /// <summary> Get the next <see cref="float"/> above the specified <paramref name="value"/> </summary>
    /// <param name="value">The current value of the <see cref="float"/></param>
    /// <returns>The next <see cref="float"/> above the <paramref name="value"/></returns>
    /// <exception cref="ArgumentException"><see cref="float.PositiveInfinity"/> does not have a next</exception>
    public static float Next(this float value) => float.IsPositiveInfinity(value)
            ? throw new ArgumentException($"{value} does not have a valid next")
            : (value.ToIndex() + 1).FromIndex();

    /// <summary> Increment the <paramref name="value"/> by an <paramref name="amount"/> </summary>
    /// <param name="value">The <see cref="float"/> value to increment</param>
    /// <param name="amount">The amount of steps to increment the <paramref name="value"/></param>
    /// <returns>The <paramref name="value"/> incremented by the <paramref name="amount"/></returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> incremented by <paramref name="amount"/> overflows into <see cref="float.NaN"/></exception>
    public static float Increment(this float value, uint amount) {
        var index = value.ToIndex() + (int)amount;
        return index > MaxIndex
            ? throw new ArgumentException($"{value} incremented by {amount} causes overflow into NaN")
            : index.FromIndex();
    }

    /// <summary> Get the next <see cref="float"/> above the specified <paramref name="value"/> </summary>
    /// <param name="value">The current value of the <see cref="float"/></param>
    /// <returns>The next <see cref="float"/> above the <paramref name="value"/></returns>
    /// <exception cref="ArgumentException"><see cref="float.NegativeInfinity"/> does not have a previous</exception>
    public static float Previous(this float value) => float.IsNegativeInfinity(value)
            ? throw new ArgumentException($"{value} does not have a valid previous")
            : (value.ToIndex() - 1).FromIndex();

    /// <summary> Decrement the <paramref name="value"/> by an <paramref name="amount"/> </summary>
    /// <param name="value">The <see cref="float"/> value to decrement</param>
    /// <param name="amount">The amount of steps to decrement the <paramref name="value"/></param>
    /// <returns>The <paramref name="value"/> decremented by the <paramref name="amount"/></returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> decremented by <paramref name="amount"/> underflows into <see cref="float.NaN"/></exception>
    public static float Decrement(this float value, uint amount) {
        var index = value.ToIndex() - (int)amount;
        return index < MinIndex
            ? throw new ArgumentException($"{value} decremented by {amount} causes underflow into NaN")
            : index.FromIndex();
    }

    /// <summary> Try make the <paramref name="value"/> finite </summary>
    /// <param name="value">The <see cref="float"/> to try make finite</param>
    /// <returns>A finite float</returns>
    /// <exception cref="ArgumentException"><see cref="float.NaN"/> can not be made finite</exception>
    public static float TryMakeFinite(this float value) => float.IsFinite(value)
            ? value
            : float.IsPositiveInfinity(value)
                ? float.MaxValue
                : float.IsNegativeInfinity(value) ? float.MinValue : throw new ArgumentException($"{value} can not be made finite.");
}
