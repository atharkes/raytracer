using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary> A material class for primitives in the scene </summary>
    public class ParametricMaterial : ISurfaceMaterial {
        /// <summary> The color of the <see cref="ParametricMaterial"/> </summary>
        public ISpectrum Albedo { get; set; }
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
        /// <summary> The color of emitting light </summary>
        public ISpectrum EmittanceColor { get; set; } = ISpectrum.Black;
        /// <summary> How much light this material is emitting (in watt/m^2) </summary>
        public float EmittingStrength { get; set; } = 0f;
        /// <summary> The light this material is emitting </summary>
        public ISpectrum EmittingLight => EmittanceColor * EmittingStrength;
        /// <summary> Whether this material is emitting light </summary>
        public bool IsEmitting => EmittingStrength > 0f;
        /// <summary> This material is not sensing light </summary>
        public bool IsSensing => false;

        /// <summary> Create a diffuse white material </summary>
        public static ParametricMaterial DiffuseWhite => new(new RGBSpectrum(0.8f, 0.8f, 0.8f));
        /// <summary> Create a diffuse green material </summary>
        public static ParametricMaterial DiffuseGreen => new(new RGBSpectrum(0.2f, 0.8f, 0.2f));
        /// <summary> Create a diffuse yellow material </summary>
        public static ParametricMaterial DiffuseYellow => new(new RGBSpectrum(0.8f, 0.8f, 0.2f));
        /// <summary> Create a glossy red material </summary>
        public static ParametricMaterial GlossyRed => new(new RGBSpectrum(0.8f, 0.2f, 0.2f), 0, 0, 1, 0.5f, 15f);
        /// <summary> Create a glossy green material </summary>
        public static ParametricMaterial GlossyGreen => new(new RGBSpectrum(0.2f, 0.8f, 0.2f), 0, 0, 1, 0.7f, 50f);
        /// <summary> Create a glossy mirror with a purple hue </summary>
        public static ParametricMaterial GlossyPurpleMirror => new(new RGBSpectrum(0.8f, 0.2f, 0.8f), 0.4f, 0, 1, 0.7f, 50f);
        /// <summary> Create a mirror material </summary>
        public static ParametricMaterial Mirror => new(new RGBSpectrum(0.9f, 0.9f, 0.9f), 0.97f);
        /// <summary> Create a glass material </summary>
        public static ParametricMaterial Glass => new(new RGBSpectrum(0.9f, 0.9f, 0.9f), 0, 0.97f, 1.62f);
        /// <summary> A white light </summary>
        public static ParametricMaterial WhiteLight => new(1f, ISpectrum.White);

        /// <summary> Create a new material </summary>
        /// <param name="color">The color of the material</param>
        /// <param name="specularity">How specular the material is. A specular object reflects light like a mirror.</param>
        /// <param name="dielectric">How dielectric the material is. A dielectric object both passes light and reflects it like water or glass.</param>
        /// <param name="refractionIndex">The refraction index of the material if it is a dielectric. This is typically a value between 1 and 3.</param>
        /// <param name="glossyness">The glossyness of the material</param>
        /// <param name="glossSpecularity">The gloss specularity of the material</param>
        public ParametricMaterial(ISpectrum color, float specularity = 0, float dielectric = 0, float refractionIndex = 1, float glossyness = 0, float glossSpecularity = 0) {
            Albedo = color;
            Specularity = specularity;
            Dielectric = dielectric;
            RefractionIndex = refractionIndex;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        /// <summary> Create an emitting material </summary>
        /// <param name="color">The color of the material and the light</param>
        /// <param name="emittingStrength">The emitting strength of the light</param>
        public ParametricMaterial(float emittingStrength, ISpectrum color) {
            EmittingStrength = emittingStrength;
            EmittanceColor = color;
            Albedo = ISpectrum.Black;
        }

        public ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) {
            if (IsEmitting && IDirection3.Similar(orientation, direction)) {
                return EmittingLight;
            } else {
                return ISpectrum.Black;
            }
        }

        public IPDF<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            return new Diffuse(orientation);
            IPDF<Normal3> diffuse, specular;
            //Vector3 radianceOut;
            //if (surfacePoint.Primitive.Material.Specularity > 0) {
            //    // Specular
            //    Vector3 reflectedIn = Sample(intersection.Reflect());
            //    Vector3 reflectedOut = reflectedIn * intersection.SurfacePoint.Primitive.Material.Color;
            //    radianceOut = irradianceIn * (1 - intersection.SurfacePoint.Primitive.Material.Specularity) + reflectedOut * surfacePoint.Primitive.Material.Specularity;
            //} else if (surfacePoint.Primitive.Material.Dielectric > 0) {
            //    // Dielectric
            //    float reflected = intersection.Reflectivity();
            //    float refracted = 1 - reflected;
            //    Ray? refractedRay = intersection.Refract();
            //    Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay) : Vector3.Zero;
            //    Vector3 incReflectedLight = Sample(intersection.Reflect());
            //    radianceOut = irradianceIn * (1f - surfacePoint.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * surfacePoint.Primitive.Material.Dielectric * surfacePoint.Primitive.Material.Color;
            //} else {
            //    // Diffuse
            //    radianceOut = irradianceIn;
            //}
        }
    }
}
