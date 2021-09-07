using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives {
    /// <summary> A simple primitive object for the scene </summary>
    public class Primitive : IPrimitive {
        /// <summary> The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/> </summary>
        public IShape Shape { get; }
        /// <summary> The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/> </summary>
        public IMaterial Material { get; }

        /// <summary> Whether the <see cref="Primitive"/> encompasses a volume </summary>
        public bool Volumetric => Shape.Volumetric;
        /// <summary> The volume of the <see cref="Primitive"/> </summary>
        public float Volume => Shape.Volume;
        /// <summary> The surface area of the <see cref="Primitive"/> </summary>
        public float SurfaceArea => Shape.SurfaceArea;
        /// <summary> The bounding box of the <see cref="Primitive"/> </summary>
        public virtual AxisAlignedBox BoundingBox => Shape.BoundingBox;

        /// <summary> Create a new <see cref="Primitive"/> with a <paramref name="shape"/> and <paramref name="material"/> </summary>
        /// <param name="shape">The <see cref="SceneDescription.Shape"/> of the <see cref="Primitive"/></param>
        /// <param name="material">The <see cref="SceneDescription.Material"/> of the <see cref="Primitive"/></param>
        public Primitive(IShape shape, IMaterial material) {
            Shape = shape;
            Material = material;
        }

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>An intersection with a primitive if there is any</returns>
        public ISurfacePoint? Trace(Ray ray) {
            IEnumerable<IBoundaryPoint> distances = Shape.Intersect(ray);
            throw new NotImplementedException("Requires an implementation of IScatteringPoint");
        }

        /// <summary> Intersect the <see cref="Primitive"/> with a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="Ray"/> to intersect the <see cref="Primitive"/> with</param>
        /// <returns>Whether the <paramref name="ray"/> intersects the <see cref="Primitive"/></returns>
        public bool Intersects(Ray ray) => Shape.Intersects(ray);
        public bool Inside(Vector3 position) => Shape.Inside(position);
        public Vector3 PointOnSurface(Random random) => Shape.PointOnSurface(random);
        public bool OnSurface(Vector3 position, float epsilon = 0.001F) => Shape.OnSurface(position, epsilon);
        public Vector3 SurfaceNormal(Vector3 position) => Shape.SurfaceNormal(position);
        public IEnumerable<float> IntersectDistances(Ray ray) => Shape.IntersectDistances(ray);
        public IEnumerable<IBoundaryPoint> Intersect(Ray ray) => Shape.Intersect(ray);

        IEnumerable<IShape> IDivisible<IShape>.Clip(AxisAlignedPlane plane) => Clip(plane);

        public virtual IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            foreach (IShape shape in Shape.Clip(plane)) {
                if (shape == Shape) {
                    yield return this;
                } else {
                    yield return new PrimitiveFragment(this, shape);
                }
            }
        }
    }
} 
