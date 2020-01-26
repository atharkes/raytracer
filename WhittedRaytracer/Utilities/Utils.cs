using OpenTK;
using System;
using System.Threading;

namespace WhittedRaytracer.Utilities {
    /// <summary> Just some usefull static methods </summary>
    public static class Utils {
        /// <summary> A deterministic random to generate random scenes which are always the same </summary>
        public static readonly Random DetRandom = new Random(0);
        /// <summary> Random to generate random values </summary>
        public static Random Random => random ?? (random = new Random(Thread.CurrentThread.ManagedThreadId * (int)DateTime.Today.TimeOfDay.Ticks));
        [ThreadStatic]
        static Random random;

        /// <summary> Get a vector with random values between 0 and 1 </summary>
        public static Vector3 DetRandomVector => new Vector3((float)DetRandom.NextDouble(), (float)DetRandom.NextDouble(), (float)DetRandom.NextDouble());
        /// <summary> Get a vector with random values between 0 and 1 </summary>
        public static Vector3 RandomVector => new Vector3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble());
        /// <summary> Vector with float minimum values </summary>
        public static Vector3 MinVector => new Vector3(float.MinValue, float.MinValue, float.MinValue);
        /// <summary> Vector with float maximum values </summary>
        public static Vector3 MaxVector => new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        /// <summary> Get a color scale for a value from black to green to yellow to red </summary>
        /// <param name="value">The value to get the color for</param>
        /// <param name="min">The minimum value of the scale</param>
        /// <param name="max">The maximum value of the scale</param>
        /// <returns>A color of the color scale</returns>
        public static Vector3 ColorScaleBlackGreenYellowRed(float value, float min, float max) {
            const int transitions = 3;
            float range = max - min;
            float transition1 = range / transitions;
            float green = value < transition1 ? value / transition1 : transitions - value / transition1;
            float red = (value - transition1) / transition1;
            return new Vector3(red, green, 0);
        }
    }
}
