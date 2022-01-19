using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates {
    /// <summary> Simple container <see cref="ISceneObject"/> holding multiple <see cref="ISceneObject"/>s </summary>
    public class Aggregate : IAggregate {
        /// <summary> The bounding box of the <see cref="Aggregate"/> </summary>
        public AxisAlignedBox BoundingBox { get; protected set; } = new(Position3.PositiveInfinity, Position3.NegativeInfinity);
        /// <summary> The amount of children of the <see cref="Aggregate"/> </summary>
        public int ItemCount => Items.Count;
        /// <summary> The <see cref="IShape"/> of the items in this <see cref="Aggregate"/> </summary>
        public IShape Shape => throw new NotImplementedException();

        /// <summary> The <see cref="ISceneObject"/>s in the <see cref="IAggregate"/> </summary>
        protected ICollection<ISceneObject> Items { get; set; } = new HashSet<ISceneObject>();

        public Aggregate() { }

        public Aggregate(IEnumerable<ISceneObject> items) {
            foreach (ISceneObject item in items) {
                Add(item);
            }
        }

        public void Add(ISceneObject item) {
            Items.Add(item);
            Position3 minCorner = Position3.ComponentMax(BoundingBox.MinCorner, item.Shape.BoundingBox.MinCorner);
            Position3 maxCorner = Position3.ComponentMin(BoundingBox.MaxCorner, item.Shape.BoundingBox.MaxCorner);
            BoundingBox = new(minCorner, maxCorner);
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

        public IEnumerator<ISceneObject> GetEnumerator() => Items.GetEnumerator();

        public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
            IDistanceQuery? result = null;
            foreach (ISceneObject sceneObject in Items) {
                result += sceneObject.Trace(ray, spectrum);
            }
            return result;
        }

        public IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            throw new NotImplementedException();
        }
    }
}
