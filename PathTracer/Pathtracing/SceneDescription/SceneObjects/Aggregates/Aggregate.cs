using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates {
    /// <summary> Simple container <see cref="ISceneObject"/> holding multiple <see cref="ISceneObject"/>s </summary>
    public class Aggregate : IAggregate {
        /// <summary> The amount of children of the <see cref="Aggregate"/> </summary>
        public int ItemCount => Items.Count;
        /// <summary> The <see cref="IShape"/> of the items in this <see cref="Aggregate"/> </summary>
        public IShape Shape => new Union(Items.Select(i => i.Shape).ToArray());

        /// <summary> The <see cref="ISceneObject"/>s in the <see cref="IAggregate"/> </summary>
        protected ICollection<ISceneObject> Items { get; set; } = new HashSet<ISceneObject>();

        public Aggregate() { }

        public Aggregate(IEnumerable<ISceneObject> items) {
            AddRange(items);
        }

        public void Add(ISceneObject item) => Items.Add(item);

        public void AddRange(IEnumerable<ISceneObject> items) {
            foreach (ISceneObject item in items) {
                Add(item);
            }
        }

        public bool Remove(ISceneObject item) => Items.Remove(item);

        public IEnumerator<ISceneObject> GetEnumerator() => Items.GetEnumerator();

        public IDistanceQuery? Trace(IRay ray, ISpectrum spectrum) {
            IDistanceQuery? result = null;
            foreach (ISceneObject sceneObject in Items) {
                result += sceneObject.Trace(ray, spectrum);
            }
            return result;
        }

        public IEnumerable<ISceneObject> Clip(AxisAlignedPlane plane) {
            throw new NotImplementedException("Split items and clip items on the border");
        }
    }
}
