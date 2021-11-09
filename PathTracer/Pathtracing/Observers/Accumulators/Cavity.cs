using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;

namespace PathTracer.Pathtracing.Observers.Accumulators {
    /// <summary> A cavity of the accumulator that catches light </summary>
    public class Cavity {
        /// <summary> The amount of light in the cavity </summary>
        public ISpectrum Light { get; private set; } = ISpectrum.Black;
        /// <summary> The amount of photons caught by the cavity </summary>
        public int Samples { get; private set; } = 0;
        /// <summary> Amount of times the bvh is traversed by photons in the cavity </summary>
        public int BVHTraversals { get; private set; } = 0;
        /// <summary> How many samples have intersected something </summary>
        public int Intersections { get; private set; } = 0;

        /// <summary> Average light of the photons in the cavity </summary>
        public ISpectrum AverageLight => Light.IsBlack || Samples == 0 ? ISpectrum.Black : Light / Samples;
        /// <summary> Average BVH traversals of photons in the cavity </summary>
        public float AverageBVHTraversals => Samples > 0 ? BVHTraversals / Samples : 0f;
        /// <summary> The green to red color fade for the BVH traversals </summary>
        public RGBSpectrum AverageBVHTraversalColor => Utils.ColorScaleBlackGreenYellowRed(AverageBVHTraversals, 0, 255);
        /// <summary> The average chance of intersections per sample </summary>
        public float IntersectionChance => Samples > 0 ? (float)Intersections / Samples : 0f;
        /// <summary> The color for intersection chance </summary>
        public RGBSpectrum IntersectionChanceColor => Utils.ColorScaleBlackGreenYellowRed(IntersectionChance, 0, 1);

        /// <summary> Add a sample to the cavity </summary>
        /// <param name="light">The light to add to the cavity</param>
        public void AddSample(ISpectrum light, int bvhTraversals, bool intersection) {
            Light += light;
            BVHTraversals += bvhTraversals;
            if (intersection) Intersections++;
            Samples++;
        }

        /// <summary> Clear the light in the cavity </summary>
        public void Clear() {
            Light = ISpectrum.Black;
            BVHTraversals = 0;
            Intersections = 0;
            Samples = 0;
        }

        public override string ToString() => AverageLight.ToString();
    }
}
