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

namespace PathTracer.Utilities.Extensions {
    /// <summary> Extension methods for <see cref="Random"/> </summary>
    public static class RandomExtensions {
        /// <summary> Get an index for a <paramref name="random"/> finite <see cref="float"/> </summary>
        /// <param name="random">The <see cref="Random"/> to execute the random decisions</param>
        /// <returns>A <paramref name="random"/> finite <see cref="float"/>-index </returns>
        public static int FiniteSingleIndex(this Random random) => random.Next(SingleExtensions.MinIndex + 1, SingleExtensions.MaxIndex);

        /// <summary> Get an index for a <paramref name="random"/> finite <see cref="double"/> </summary>
        /// <param name="random">The <see cref="Random"/> to execute the random decsisions</param>
        /// <returns>A <paramref name="random"/> finite <see cref="double"/>-index </returns>
        public static long FiniteDoubleIndex(this Random random) => random.NextInt64(DoubleExtensions.MinIndex + 1, DoubleExtensions.MaxIndex);

        /// <summary> Get a <paramref name="random"/> finite <see cref="float"/> </summary>
        /// <param name="random">The <see cref="Random"/> to execute the random decisions</param>
        /// <returns>A <paramref name="random"/> finite <see cref="float"/></returns>
        public static float FiniteSingle(this Random random) => random.FiniteSingleIndex().FromIndex();

        /// <summary> Get a <paramref name="random"/> finite <see cref="double"/> </summary>
        /// <param name="random">The <see cref="Random"/> to execute the random decisions</param>
        /// <returns>A <paramref name="random"/> finite <see cref="double"/></returns>
        public static double FiniteDouble(this Random random) => random.FiniteDoubleIndex().FromIndex();

        /// <summary> Create a <paramref name="random"/> <see cref="Vector3"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Vector3"/> with</param>
        /// <param name="scale">The scale of the <see cref="Vector3"/></param>
        /// <returns>A <paramref name="random"/> <see cref="Vector3"/> with values between 0 and the scale parameter</returns>
        public static Vector3 Vector(this Random random, float scale = 1f) {
            return new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle()) * scale;
        }

        /// <summary> Create a <paramref name="random"/> <see cref="Unit3"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the <see cref="Unit3"/> with</param>
        /// <returns>A <paramref name="random"/> <see cref="Unit3"/></returns>
        public static Unit3 Unit(this Random random) {
            float a = random.NextSingle();
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

        /// <summary> Create a <paramref name="random"/> normalized vector </summary>
        /// <param name="random">The <see cref="Random"/> to create the vector with</param>
        /// <returns>A <paramref name="random"/> normalized vector</returns>
        public static Normal3 Normal3(this Random random) => new(random.Vector());

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
            float roughness = random.NextDouble() < 0.5f ? (float)random.NextDouble() : 0;
            //float dielectric = random.NextDouble() < 0.1f ? (float)random.NextDouble() : 0;
            //float refractionIndex = (float)random.NextDouble() * 2f + 1f;
            return new ParametricMaterial(color, specularity, roughness);
        }

        /// <summary> Create a random emitting <see cref="IMaterial"/> </summary>
        /// <param name="random">The <see cref="Random"/> to create the emitter</param>
        /// <returns>A <paramref name="random"/> emitting <see cref="IMaterial"/></returns>
        public static IMaterial RandomEmitter(this Random random) {
            ISpectrum color = new RGBSpectrum(random.Vector());
            return new ParametricMaterial(random.Next(1, 50), color);
        }
    }
}
