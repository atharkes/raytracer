using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using System;
using System.Threading;

namespace PathTracer.Utilities {
    /// <summary> Just some usefull static methods </summary>
    public static class Utils {
        /// <summary> A deterministic <see cref="Random"/> to generate random scenes which are always the same </summary>
        public static Random DeterministicRandom { get; } = new Random(0);
        /// <summary> The <see cref="Random"/> to use per thread </summary>
        public static Random ThreadRandom => random ??= new Random(Thread.CurrentThread.ManagedThreadId * (int)DateTime.Today.TimeOfDay.Ticks);

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

        /// <summary> Create a <paramref name="random"/> <see cref="Vector3"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Vector3"/> with</param>
        /// <param name="scale">The scale of the <see cref="Vector3"/></param>
        /// <returns>A <paramref name="random"/> <see cref="Vector3"/> with values between 0 and the scale parameter</returns>
        public static Vector3 Vector(this Random random, float scale = 1f) {
            return new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()) * scale;
        }

        /// <summary> Create a <paramref name="random"/> <see cref="Unit3"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Unit3"/> with</param>
        /// <returns>A <paramref name="random"/> <see cref="Unit3"/></returns>
        public static Unit3 Unit(this Random random) {
            float a = (float)random.NextDouble();
            if (a < 1 / 6) {
                return Unit3.X;
            } else if (a < 2 / 6) {
                return Unit3.Y;
            } else if (a < 3 / 6) {
                return Unit3.Z;
            } else if (a < 4 / 6) {
                return Unit3.MinX;
            } else if (a < 5 / 6) {
                return Unit3.MinY;
            } else {
                return Unit3.MinZ;
            }
        }

        /// <summary> Create a <paramref name="random"/> unit vector</summary>
        /// <param name="random">The <see cref="Random"/> to create the unit vector with</param>
        /// <returns>A <paramref name="random"/> unit vector</returns>
        public static Normal3 UnitVector3(this Random random) => new(random.Unit());

        /// <summary> Create a <paramref name="random"/> <see cref="IPrimitive"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="IPrimitive"/> with</param>
        /// <returns>A <paramref name="random"/> <see cref="IPrimitive"/></returns>
        public static IPrimitive Primitive(this Random random, float posRange = 1f, float scale = 1f) {
            return new Primitive(random.Shape(posRange, scale), random.Material());
        }

        /// <summary> Create a <paramref name="random"/> <see cref="IShape"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="IShape"/> with</param>
        /// <param name="posRange">The range around which the <see cref="IShape"/> can be</param>
        /// <param name="scale">The possible scale of the <see cref="IShape"/></param>
        /// <returns>A <paramref name="random"/> <see cref="IShape"/></returns>
        public static IShape Shape(this Random random, float posRange = 1f, float scale = 1f) {
            return random.NextDouble() < 0.5 ? random.CreateSphere(posRange, scale) : random.CreateTriangle(posRange, scale);
        }

        /// <summary> Create a <paramref name="random"/> <see cref="Triangle"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Triangle"/> with</param>
        /// <param name="posRange">The range in which the first point can be</param>
        /// <param name="scale">The possible scale of the <see cref="Triangle"/></param>
        /// <returns>A <paramref name="random"/> <see cref="Triangle"/></returns>
        public static Triangle CreateTriangle(this Random random, float posRange = 1f, float scale = 1f) {
            Vector3 pos = random.Vector(posRange);
            return new Triangle(pos, pos + random.Vector(scale), pos + random.Vector(scale));
        }

        /// <summary> Create a <paramref name="random"/> <see cref="Sphere"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Sphere"/> with</param>
        /// <param name="posRange">The range in which the position of the <see cref="Sphere"/> can be</param>
        /// <param name="scale">The possible scale of the <see cref="Sphere"/></param>
        /// <returns>A <paramref name="random"/> <see cref="Sphere"/></returns>
        public static Sphere CreateSphere(this Random random, float posRange = 0f, float scale = 1f) {
            return new Sphere(random.Vector(posRange), (float)random.NextDouble() * scale);
        }

        /// <summary> Create a <paramref name="random"/> <see cref="IMaterial"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="IMaterial"/></param>
        /// <returns>A <paramref name="random"/> <see cref="IMaterial"/></returns>
        public static IMaterial Material(this Random random) {
            return random.NextDouble() < 0.9995f ? random.RandomNonEmitter() : random.RandomEmitter();
        }

        /// <summary> Create a <paramref name="random"/> non-emitting <see cref="IMaterial"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the non emitter</param>
        /// <returns>A <paramref name="random"/> non-emitting <see cref="IMaterial"/></returns>
        public static IMaterial RandomNonEmitter(this Random random) {
            ISpectrum color = new RGBSpectrum(random.Vector());
            float specularity = random.NextDouble() < 0.3f ? (float)random.NextDouble() : 0;
            float dielectric = random.NextDouble() < 0.1f ? (float)random.NextDouble() : 0;
            float refractionIndex = (float)random.NextDouble() * 2f + 1f;
            float glossyness = random.NextDouble() < 0.5f ? (float)random.NextDouble() : 0;
            float glossSpecularity = (float)random.NextDouble() * 10f;
            return new ParametricMaterial(color, specularity, dielectric, refractionIndex, glossyness, glossSpecularity);
        }

        /// <summary> Create a random emitting <see cref="IMaterial"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the emitter</param>
        /// <returns>A <paramref name="random"/> emitting <see cref="IMaterial"/></returns>
        public static IMaterial RandomEmitter(this Random random) {
            ISpectrum color = new RGBSpectrum(random.Vector());
            return new ParametricMaterial(random.Next(1, 50), color);
        }

        /// <summary> Get a color scale for a value from black to green to yellow to red </summary>
        /// <param name="value">The value to get the color for</param>
        /// <param name="min">The minimum value of the scale</param>
        /// <param name="max">The maximum value of the scale</param>
        /// <returns>A color of the color scale</returns>
        public static RGBSpectrum ColorScaleBlackGreenYellowRed(float value, float min, float max) {
            const int transitions = 3;
            float range = max - min;
            float transition1 = range / transitions;
            float green = value < transition1 ? value / transition1 : transitions - value / transition1;
            float red = (value - transition1) / transition1;
            return new RGBSpectrum(red, green, 0);
        }
    }
}
