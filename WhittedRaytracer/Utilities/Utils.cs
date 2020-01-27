using OpenTK;
using System;
using System.Threading;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedRaytracer.Utilities {
    /// <summary> Just some usefull static methods </summary>
    public static class Utils {
        /// <summary> A deterministic random to generate random scenes which are always the same </summary>
        public static readonly Random DetRandom = new Random(0);
        /// <summary> Random to generate random values </summary>
        public static Random Random => random ?? (random = new Random(Thread.CurrentThread.ManagedThreadId * (int)DateTime.Today.TimeOfDay.Ticks));
        [ThreadStatic]
        static Random random;

        /// <summary> Create a random Vector </summary>
        /// <param name="r">The random to create the vector with</param>
        /// <param name="scale">The scale of the vector</param>
        /// <returns>A random vector with values between 0 and the scale parameter</returns>
        public static Vector3 Vector(this Random r, float scale = 1f) {
            return new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()) * scale;
        }

        /// <summary> Create a random Unit Vector </summary>
        /// <param name="r">The random to create the unit vector with</param>
        /// <returns>A random unit vector</returns>
        public static Vector3 UnitVector(this Random r) {
            float a = (float)r.NextDouble();
            return a < 0.33f ? Vector3.UnitX : (a < 0.66f ? Vector3.UnitY : Vector3.UnitZ);
        }

        /// <summary> Create a random Primitive </summary>
        /// <param name="r">The random to create the primitive with</param>
        /// <param name="posRange">The range around which the primitive can be</param>
        /// <param name="scale">The possible scale of the random primitive</param>
        /// <returns>A random primitive</returns>
        public static Primitive Primitive(this Random r, float posRange = 1f, float scale = 1f) {
            return r.NextDouble() < 0.5f ? r.Sphere(posRange, scale) as Primitive : r.Triangle(posRange, scale) as Primitive;
        }

        /// <summary> Create a random Triangle </summary>
        /// <param name="r">The random to create the triangle with</param>
        /// <param name="posRange">The range in which the first point can be</param>
        /// <param name="scale">The possible scale of the random triangle</param>
        /// <returns>A random triangle</returns>
        public static Triangle Triangle(this Random r, float posRange = 1f, float scale = 1f) {
            Vector3 pos = r.Vector(posRange);
            return new Triangle(pos, pos + r.Vector(scale), pos + r.Vector(scale));
        }

        /// <summary> Create a random sphere </summary>
        /// <param name="r">The random to create the sphere with</param>
        /// <param name="posRange">The range in which the position of the sphere can be</param>
        /// <param name="scale">The scale of the random sphere</param>
        /// <returns>A random sphere</returns>
        public static Sphere Sphere(this Random r, float posRange = 0f, float scale = 1f) {
            return new Sphere(r.Vector(posRange), (float)r.NextDouble() * scale);
        }

        /// <summary> Vector with float minimum values </summary>
        public static Vector3 MinVector => new Vector3(float.MinValue, float.MinValue, float.MinValue);
        /// <summary> Vector with float maximum values </summary>
        public static Vector3 MaxVector => new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        /// <summary> Get the bounds of a set of points </summary>
        /// <param name="points">The points to compute the bounds for</param>
        /// <returns>The bounds of the set of points</returns>
        public static Vector3[] GetBounds(Vector3[] points) {
            Vector3 min = MaxVector;
            Vector3 max = MinVector;
            foreach (Vector3 point in points) {
                min = Vector3.ComponentMin(min, point);
                max = Vector3.ComponentMax(max, point);
            }
            return new Vector3[] { min, max };
        }

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
