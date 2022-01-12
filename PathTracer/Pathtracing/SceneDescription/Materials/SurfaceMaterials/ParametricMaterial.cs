using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary> A material class for primitives in the scene </summary>
    public class ParametricMaterial : ISurfaceMaterial {
        /// <summary> The color of the <see cref="ParametricMaterial"/> </summary>
        public ISpectrum Albedo { get; set; }
        /// <summary> How large the specular part of the <see cref="ParametricMaterial"/> is </summary>
        public double Specularity { get; set; } = 0;
        /// <summary> How rough the surface of the <see cref="ParametricMaterial"/> is </summary>
        public double Roughness { get; set; } = 0;
        /// <summary> How dielectric this primitive is. A dielectric object both passes light and reflects it like water or glass. </summary>
        public double Dielectric { get; set; } = 0;
        

        /// <summary> The color of emitting light </summary>
        public ISpectrum EmittanceColor { get; set; } = ISpectrum.Black;
        /// <summary> How much light this material is emitting (in watt/m^2) </summary>
        public float EmittingStrength { get; set; } = 0f;
        /// <summary> The light this material is emitting </summary>
        public ISpectrum EmittingLight => EmittanceColor * EmittingStrength;
        /// <summary> Whether this material is emitting light </summary>
        public bool IsEmitting => EmittingStrength > 0;
        /// <summary> This material is not sensing light </summary>
        public bool IsSensing => false;

        /// <summary> Create a diffuse white material </summary>
        public static ParametricMaterial DiffuseWhite => new(new RGBSpectrum(0.9f, 0.9f, 0.9f));
        /// <summary> Create a diffuse white material </summary>
        public static ParametricMaterial DiffuseGray => new(new RGBSpectrum(0.7f, 0.7f, 0.7f));
        /// <summary> Create a diffuse green material </summary>
        public static ParametricMaterial DiffuseGreen => new(new RGBSpectrum(0.2f, 0.8f, 0.2f));
        /// <summary> Create a diffuse yellow material </summary>
        public static ParametricMaterial DiffuseYellow => new(new RGBSpectrum(0.8f, 0.8f, 0.2f));
        /// <summary> Create a glossy red material </summary>
        public static ParametricMaterial GlossyRed => new(new RGBSpectrum(0.8f, 0.2f, 0.2f), 1f, 0.2f);
        /// <summary> Create a glossy green material </summary>
        public static ParametricMaterial GlossyGreen => new(new RGBSpectrum(0.2f, 0.8f, 0.2f), 1f, 0.2f);
        /// <summary> Create a glossy mirror with a purple hue </summary>
        public static ParametricMaterial PurpleHalfMirror => new(new RGBSpectrum(0.8f, 0.2f, 0.8f), 0.5f, 0f);
        /// <summary> Create a mirror material </summary>
        public static ParametricMaterial Mirror => new(new RGBSpectrum(0.9f, 0.9f, 0.9f), 1f, 0.03f);
        /// <summary> Create a rough mirror material </summary>
        public static ParametricMaterial RoughMirror => new(new RGBSpectrum(0.9f, 0.9f, 0.9f), 1f, 0.2f);
        /// <summary> A white light </summary>
        public static ParametricMaterial WhiteLight => new(1f, ISpectrum.White);

        /// <summary> Create a new material </summary>
        /// <param name="color">The color of the material</param>
        /// <param name="specularity">The percentage of sepcular microfacets compared to diffuse</param>
        /// <param name="roughness">How rough the surface is</param>
        public ParametricMaterial(ISpectrum color, double specularity = 0, double roughness = 0) {
            Albedo = color;
            Specularity = specularity;
            Roughness = roughness;
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
            if (IsEmitting && IDirection3.InSameClosedHemisphere(orientation, direction)) {
                return EmittingLight;
            } else {
                return ISpectrum.Black;
            }
        }

        public IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            var diffuse = new HemisphericalDiffuse(orientation);
            var specular = new SpecularReflection(orientation, incomingDirection);
            return new CombinedProbabilityDistribution<Normal3>((diffuse, 1 - Specularity), (specular, Specularity));

            //} else if (surfacePoint.Primitive.Material.Dielectric > 0) {
            //    // Dielectric
            //    float reflected = intersection.Reflectivity();
            //    float refracted = 1 - reflected;
            //    Ray? refractedRay = intersection.Refract();
            //    Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay) : Vector3.Zero;
            //    Vector3 incReflectedLight = Sample(intersection.Reflect());
            //    radianceOut = irradianceIn * (1f - surfacePoint.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * surfacePoint.Primitive.Material.Dielectric * surfacePoint.Primitive.Material.Color;
        }
    }
}
