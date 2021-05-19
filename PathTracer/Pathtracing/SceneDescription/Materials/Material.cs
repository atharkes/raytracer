using OpenTK.Mathematics;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> A material class for primitives in the scene </summary>
    public class Material : IMaterial {
        /// <summary> The color of the primitive </summary>
        public Vector3 Color { get; set; } = Vector3.One;
        /// <summary> How specular this primitive is. A specular object reflects light like a mirror. </summary>
        public float Specularity { get; set; } = 0f;
        /// <summary> How dielectric this primitive is. A dielectric object both passes light and reflects it like water or glass. </summary>
        public float Dielectric { get; set; } = 0f;
        /// <summary> The refraction index of this primitive if it is a dielectric. This is typically a value between 1 and 3.
        /// <para> Vacuum 1 </para>
        /// <para> Gases at 0 °C: Air 1.000293, Helium 1.000036, Hydrogen 1.000132, Carbon dioxide 1.00045 </para>
        /// <para> Liquids at 20 °C: Water 1.333, Ethanol 1.36, Olive oil 1.47 </para>
        /// <para> Solids: Ice 1.31, Fused silica(quartz) 1.46, Plexiglas 1.49, Window glass 1.52, 
        /// Flint glass 1.62, Sapphire 1.77, Cubic zirconia 2.15, Diamond 2.42, Moissanite 2.65 </para>
        /// </summary>
        public float RefractionIndex { get; set; } = 1f;
        /// <summary> The glossyness of the primitive </summary>
        public float Glossyness { get; set; } = 0f;
        /// <summary> The gloss specularity of the primitive </summary>
        public float GlossSpecularity { get; set; } = 0f;

        /// <summary> How much light this material is emitting (in watt/m^2) </summary>
        public float EmittingStrength { get; set; } = 0f;

        /// <summary> The light this material is emitting </summary>
        public Vector3 EmittingLight => Color * EmittingStrength;
        /// <summary> Whether this material is emitting light </summary>
        public bool Emitting => EmittingStrength > 0f;

        /// <summary> Create a new material </summary>
        /// <param name="color">The color of the material</param>
        /// <param name="specularity">How specular the material is. A specular object reflects light like a mirror.</param>
        /// <param name="dielectric">How dielectric the material is. A dielectric object both passes light and reflects it like water or glass.</param>
        /// <param name="refractionIndex">The refraction index of the material if it is a dielectric. This is typically a value between 1 and 3.</param>
        /// <param name="glossyness">The glossyness of the material</param>
        /// <param name="glossSpecularity">The gloss specularity of the material</param>
        public Material(Vector3? color = null, float specularity = 0, float dielectric = 0, float refractionIndex = 1, float glossyness = 0, float glossSpecularity = 0) {
            Color = color ?? Vector3.One;
            Specularity = specularity;
            Dielectric = dielectric;
            RefractionIndex = refractionIndex;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        /// <summary> Create an emitting material </summary>
        /// <param name="color">The color of the material and the light</param>
        /// <param name="emittingStrength">The emitting strength of the light</param>
        public Material(float emittingStrength, Vector3 color) {
            EmittingStrength = emittingStrength;
            Color = color;
        }

        /// <summary> Create a random material </summary>
        /// <returns>A random material</returns>
        public static Material Random() {
            return Utils.DetRandom.NextDouble() < 0.9995f ? RandomNonEmitter() : RandomEmitter();
        }

        /// <summary> Create a random material that doesn't emit light </summary>
        /// <returns>A random material that doesn't emit light</returns>
        public static Material RandomNonEmitter() {
            Random r = Utils.DetRandom;
            Vector3 color = Utils.DetRandom.Vector();
            float specularity = r.NextDouble() < 0.3f ? (float)r.NextDouble() : 0;
            float dielectric = r.NextDouble() < 0.1f ? (float)r.NextDouble() : 0;
            float refractionIndex = (float)r.NextDouble() * 2f + 1f;
            float glossyness = r.NextDouble() < 0.5f ? (float)r.NextDouble() : 0;
            float glossSpecularity = (float)r.NextDouble() * 10f;
            return new Material(color, specularity, dielectric, refractionIndex, glossyness, glossSpecularity);
        }

        /// <summary> Create a random emitting material </summary>
        /// <returns>A random emitting material</returns>
        public static Material RandomEmitter() {
            return new Material(Utils.DetRandom.Next(1, 50), Utils.DetRandom.Vector());
        }

        /// <summary> The default material is bright green </summary>
        public static Material Default => new(new Vector3(0f, float.MaxValue, 0f));
        /// <summary> Create a diffuse white material </summary>
        public static Material DiffuseWhite => new(new Vector3(0.8f, 0.8f, 0.8f));
        /// <summary> Create a diffuse green material </summary>
        public static Material DiffuseGreen => new(new Vector3(0.2f, 0.8f, 0.2f));
        /// <summary> Create a diffuse yellow material </summary>
        public static Material DiffuseYellow => new(new Vector3(0.8f, 0.8f, 0.2f));
        /// <summary> Create a glossy red material </summary>
        public static Material GlossyRed => new(new Vector3(0.8f, 0.2f, 0.2f), 0, 0, 1, 0.5f, 15f);
        /// <summary> Create a glossy green material </summary>
        public static Material GlossyGreen => new(new Vector3(0.2f, 0.8f, 0.2f), 0, 0, 1, 0.7f, 50f);
        /// <summary> Create a glossy mirror with a purple hue </summary>
        public static Material GlossyPurpleMirror => new(new Vector3(0.8f, 0.2f, 0.8f), 0.4f, 0, 1, 0.7f, 50f);
        /// <summary> Create a mirror material </summary>
        public static Material Mirror => new(new Vector3(0.9f, 0.9f, 0.9f), 0.97f);
        /// <summary> Create a glass material </summary>
        public static Material Glass => new(new Vector3(0.9f, 0.9f, 0.9f), 0, 0.97f, 1.62f);

        public static Material WhiteLight => new(1f, Vector3.One);
    }
}
