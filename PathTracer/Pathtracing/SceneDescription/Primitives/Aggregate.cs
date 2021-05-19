using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Primitives {
    public class Aggregate : IAggregate {
        public Vector3 Center => throw new NotImplementedException();

        public Vector3[] Bounds => throw new NotImplementedException();

        public IEnumerable<ISceneObject> Children => Items;

        protected ICollection<ISceneObject> Items { get; set; }

        public Aggregate() {
            Items = new HashSet<ISceneObject>();
            /// Update Eager Properties
        }

        public Aggregate(ICollection<ISceneObject> items) {
            Items = items;
            /// Update Eager Properties
        }

        public void Add(ISceneObject item) {
            Items.Add(item);
            /// Update Eager Properties
        }

        public bool Remove(ISceneObject item) {
            if (Items.Contains(item)) {
                Items.Remove(item);
                /// Update Eager Properties
            }
            return false;
        }

        public bool Intersects(Ray ray) {
            return Children.Any(c => c.Intersects(ray));
        }

        public float? IntersectDistance(Ray ray, out IPrimitive? primitive) {
            float? closest = null;
            primitive = null;
            foreach (ISceneObject item in Items) {
                float? current = item.IntersectDistance(ray, out IPrimitive? p);
                if ((current ?? float.MaxValue) < (closest ?? ray.Length)) {
                    closest = current;
                    primitive = p;
                }
            }
            return closest;
        }

        public (ISceneObject left, ISceneObject right) Divide(AxisAlignedPlane plane) {
            throw new NotImplementedException();
        }
    }
}
