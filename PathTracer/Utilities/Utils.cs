using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Threading;

namespace PathTracer.Utilities {
    /// <summary> Just some usefull static methods </summary>
    public static class Utils {
        /// <summary> A deterministic random to generate random scenes which are always the same </summary>
        public static Random DetRandom { get; } = new Random(0);
        /// <summary> Random to generate random values </summary>
        public static Random Random => random ??= new Random(Thread.CurrentThread.ManagedThreadId * (int)DateTime.Today.TimeOfDay.Ticks);
        
        [ThreadStatic] static Random? random;

        /// <summary> Get the minimum of two <see cref="IComparable{T}"/> </summary>
        /// <typeparam name="T">The type of the objects</typeparam>
        /// <param name="first">The first <see cref="IComparable{T}"/></param>
        /// <param name="second">The second <see cref="IComparable{T}"/></param>
        /// <returns>The minimum <see cref="IComparable{T}"/></returns>
        public static T Min<T>(T first, T second) where T : IComparable<T> {
            return first.CompareTo(second) <= 0 ? first : second;
        }

        /// <summary> Get the maximum of two <see cref="IComparable{T}"/> </summary>
        /// <typeparam name="T">The type of the objects</typeparam>
        /// <param name="first">The first <see cref="IComparable{T}"/></param>
        /// <param name="second">The second <see cref="IComparable{T}"/></param>
        /// <returns>The maximum <see cref="IComparable{T}"/></returns>
        public static T Max<T>(T first, T second) where T : IComparable<T> {
            return first.CompareTo(second) >= 0 ? first : second;
        }

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
        public static Shape Primitive(this Random r, float posRange = 1f, float scale = 1f) {
            return r.NextDouble() < 0.5f ? r.Sphere(posRange, scale) : r.Triangle(posRange, scale) as Shape;
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
