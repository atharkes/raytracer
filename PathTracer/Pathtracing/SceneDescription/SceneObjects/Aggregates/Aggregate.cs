using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Spectra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates {
    /// <summary> Simple container <see cref="ISceneObject"/> holding multiple <see cref="ISceneObject"/>s </summary>
    public class Aggregate : IAggregate {
        /// <summary> The <see cref="ISceneObject"/> children of the <see cref="Aggregate"/> </summary>
        public IEnumerable<ISceneObject> Children => Items;
        /// <summary> Whether any the children of the <see cref="Aggregate"/> encompasses a volume </summary>
        public bool Volumetric => Children.Any(c => c.Volumetric);
        /// <summary> The total volume of the children of the  <see cref="Aggregate"/> </summary>
        public float Volume => Children.Sum(c => c.Volume);
        /// <summary> The total surface area of the children of the <see cref="Aggregate"/> </summary>
        public float SurfaceArea => Children.Sum(c => c.SurfaceArea);
        /// <summary> The bounding box of the <see cref="Aggregate"/> </summary>
        public AxisAlignedBox BoundingBox { get; protected set; } = new(Vector3.PositiveInfinity, Vector3.NegativeInfinity);
        
        protected ICollection<ISceneObject> Items { get; set; } = new HashSet<ISceneObject>();

        public Aggregate() { }

        public Aggregate(IEnumerable<ISceneObject> items) {
            foreach (ISceneObject item in items) {
                Add(item);
            }
        }

        public void Add(ISceneObject item) {
            Items.Add(item);
            BoundingBox.MinCorner = Vector3.ComponentMin(BoundingBox.MinCorner, item.BoundingBox.MinCorner);
            BoundingBox.MaxCorner = Vector3.ComponentMax(BoundingBox.MaxCorner, item.BoundingBox.MaxCorner);
        }

        public void AddRange(IEnumerable<ISceneObject> items) {
            foreach (ISceneObject item in items) {
                Add(item);
            }
        }

        public bool Remove(ISceneObject item) {
            if (Items.Contains(item)) {
                var temp = Items;
                temp.Remove(item);
                Items = new HashSet<ISceneObject>(temp.Count);
                AddRange(temp);
                return true;
            }
            return false;
        }

        public virtual bool Intersects(IRay ray) {
            return BoundingBox.Intersects(ray) && Children.Any(c => c.Intersects(ray));
        }

        public virtual IEnumerable<float> IntersectDistances(Ray ray) {
            if (BoundingBox.Intersects(ray)) {
                foreach (ISceneObject item in Items) {
                    foreach (float distance in item.IntersectDistances(ray)) {
                        yield return distance;
                    }
                }
            } else {
                yield break;
            }
        }

        public virtual IEnumerable<IBoundaryPoint> Intersect(IRay ray) {
            throw new NotImplementedException("Return all boundary points?");
        }

        public (IPDF<float>, IPDF<float, IMaterial>) Trace(IRay ray, ISpectrum spectrum) {
            throw new NotImplementedException("Requires combine operations for pdfs");
        }

        public bool Inside(Vector3 position) => Items.Any(i => i.Inside(position));

        public Vector3 PointOnSurface(Random random) {
            throw new NotImplementedException("Draw random item to choose a point for");
        }

        public bool OnSurface(Vector3 position, float epsilon = 0.001F) => Items.Any(i => i.OnSurface(position, epsilon));

        public Vector3 SurfaceNormal(Vector3 position) {
            foreach (ISceneObject item in Items) {
                if (item.OnSurface(position)) {
                    return item.SurfaceNormal(position);
                }
            }
            throw new NotImplementedException("Position is not part of the surface of any of the Items. Return nullable maybe?");
        }

        IEnumerable<IShape> IDivisible<IShape>.Clip(AxisAlignedPlane plane) => Clip(plane);

        public IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            throw new NotImplementedException();
        }
    }
}
