namespace PathTracer.Pathtracing.Spectra;

/// <summary> Default colors in <see cref="RGBSpectrum"/> form </summary>
public static class RGBColors {
    /// <summary> The <see cref="RGBSpectrum"/> that represents no light </summary>
    public static readonly RGBSpectrum Black = new(0f);
    /// <summary> The <see cref="RGBSpectrum"/> that represents light of all wavelengths </summary>
    public static readonly RGBSpectrum White = new(1f);

    /// <summary> The color representing off-white </summary>
    public static readonly RGBSpectrum OffWhite = new(0.9f);
    /// <summary> The color representing gray </summary>
    public static readonly RGBSpectrum LightGray = new(0.8f);
    /// <summary> The color representing dark gray </summary>
    public static readonly RGBSpectrum Gray = new(0.7f);
    /// <summary> The color representing dark gray </summary>
    public static readonly RGBSpectrum DarkGray = new(0.4f);
    /// <summary> The color representing red </summary>
    public static readonly RGBSpectrum Red = new(0.8f, 0.2f, 0.2f);
    /// <summary> The color representing green </summary>
    public static readonly RGBSpectrum Green = new(0.2f, 0.8f, 0.2f);
    /// <summary> The color representing blue </summary>
    public static readonly RGBSpectrum Blue = new(0.2f, 0.2f, 0.8f);
    /// <summary> The color representing yellow </summary>
    public static readonly RGBSpectrum Yellow = new(0.8f, 0.8f, 0.2f);
    /// <summary> The color representing purple </summary>
    public static readonly RGBSpectrum Purple = new(0.8f, 0.2f, 0.8f);
    /// <summary> The color representing cyan </summary>
    public static readonly RGBSpectrum Cyan = new(0.2f, 0.8f, 0.8f);
}
