using OpenTK;
using System.Collections.Generic;

namespace WhittedRaytracer.Raytracing.SceneObjects {
    /// <summary> A abstract primitive for the 3d scene </summary>
    abstract class Primitive : ISceneObject {
        /// <summary> The position of the primitive </summary>
        public Vector3 Position { get; set; }
        /// <summary> The color of the primitive </summary>
        public Vector3 Color { get; set; }
        /// <summary> How specular this primitive is. A specular object reflects light like a mirror. </summary>
        public float Specularity { get; set; }
        /// <summary> How dielectric this primitive is. A dielectric object both passes light and reflects it like water or glass. </summary>
        public float Dielectric { get; set; }
        /// <summary> The refraction index of this primitive if it is a dielectric. This is typically a value between 1 and 3.
        /// <para> Vacuum 1 </para>
        /// <para> Gases at 0 °C: Air 1.000293, Helium 1.000036, Hydrogen 1.000132, Carbon dioxide 1.00045 </para>
        /// <para> Liquids at 20 °C: Water 1.333, Ethanol 1.36, Olive oil 1.47 </para>
        /// <para> Solids: Ice 1.31, Fused silica(quartz) 1.46, Plexiglas 1.49, Window glass 1.52, 
        /// Flint glass 1.62, Sapphire 1.77, Cubic zirconia 2.15, Diamond 2.42, Moissanite 2.65 </para>
        /// </summary>
        public float RefractionIndex { get; set; }
        /// <summary> The glossyness of the primitive </summary>
        public float Glossyness { get; set; }
        /// <summary> The gloss specularity of the primitive </summary>
        public float GlossSpecularity { get; set; }

        /// <summary> Create a new primitive for the 3d scene </summary>
        /// <param name="position">The position of the primitive</param>
        /// <param name="color">The color of the primitive</param>
        /// <param name="specularity">How specular this primitive is. A specular object reflects light like a mirror.</param>
        /// <param name="dielectric">How dielectric this primitive is. A dielectric object both passes light and reflects it like water or glass.</param>
        /// <param name="refractionIndex">The refraction index of this primitive if it is a dielectric. This is typically a value between 1 and 3.</param>
        /// <param name="glossyness">The glossyness of the primitive</param>
        /// <param name="glossSpecularity">The gloss specularity of the primitive</param>
        protected Primitive(Vector3? position = null, Vector3? color = null, float specularity = 0, float dielectric = 0, float refractionIndex = 1, float glossyness = 0, float glossSpecularity = 0) {
            Position = position ?? Vector3.Zero;
            Color = color ?? Vector3.One;
            Specularity = specularity;
            Dielectric = dielectric;
            RefractionIndex = refractionIndex;
            Glossyness = glossyness;
            GlossSpecularity = glossSpecularity;
        }

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>The distance the ray travels untill the intersection with this primitive</returns>
        public abstract float Intersect(Ray ray);

        /// <summary> Intersect this primitive with a ray </summary>
        /// <param name="ray">The ray to intersect the primitive with</param>
        /// <returns>Whether the ray intersects this primitive</returns>
        public abstract bool IntersectBool(Ray ray);

        /// <summary> Get the normal at an intersection on this primitive </summary>
        /// <param name="intersectionPoint">The point of the intersection</param>
        /// <returns>The normal at the point of intersection on this primitive</returns>
        public abstract Vector3 GetNormal(Vector3 intersectionPoint);

        /// <summary> Get the center of this primitive </summary>
        /// <returns>The center of this primitive</returns>
        public virtual Vector3 GetCenter() { return Position; }

        /// <summary> Get the axis-alinged bounding box bounds of this primitive </summary>
        /// <returns>The axis-alinged bounds of this primitive</returns>
        public abstract List<Vector3> GetBounds();
    }
}