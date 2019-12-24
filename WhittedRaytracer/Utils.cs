using OpenTK;
using System;

namespace WhittedRaytracer {
    /// <summary> Just some usefull static methods </summary>
    public static class Utils {
        /// <summary> Random to generate random values </summary>
        public static readonly Random Random = new Random((int)DateTime.Today.TimeOfDay.Ticks);
        /// <summary> Vector with float minimum values </summary>
        public static Vector3 MinVector => new Vector3(float.MinValue, float.MinValue, float.MinValue);
        /// <summary> Vector with float maximum values </summary>
        public static Vector3 MaxVector => new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        /// <summary> Get a vector with random values between 0 and 1 </summary>
        public static Vector3 RandomVector => new Vector3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble());
    }
}
