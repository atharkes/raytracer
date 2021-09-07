using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH {
    /// <summary> A bin for spatial splitting </summary>
    public class SpatialBin {
        /// <summary> The AABB of this bin </summary>
        public IAggregate Aggregate { get; } = new Aggregate();
        /// <summary> The clipping plane on the left </summary>
        public AxisAlignedPlane SplitPlaneLeft { get; }
        /// <summary> The clipping plane on the right </summary>
        public AxisAlignedPlane SplitPlaneRight { get; }
        /// <summary> The fragments in this spatial bin </summary>
        public List<PrimitiveFragment> Fragments { get; } = new List<PrimitiveFragment>();

        /// <summary> Create a spatial bin </summary>
        /// <param name="clipPlaneCreator">The function to create the clip planes with</param>
        /// <param name="start">The starting point of this bin in the specified direction</param>
        /// <param name="end">The ending point of this bin in the specified directio</param>
        public SpatialBin(Func<bool, float, AxisAlignedPlane> clipPlaneCreator, float start, float end) {
            SplitPlaneLeft = clipPlaneCreator(true, start);
            SplitPlaneRight = clipPlaneCreator(false, end);
        }

        /// <summary> Add a primitive to the spatial bin and clip it </summary>
        /// <param name="item">The primitive to clipp and add to the bin</param>
        public void ClipAndAdd(ISceneObject item) {
            foreach (ISceneObject singleClipped in ((IDivisible<ISceneObject>)item).Clip(SplitPlaneLeft)) {
                foreach (ISceneObject doubleClipped in ((IDivisible<ISceneObject>)singleClipped).Clip(SplitPlaneRight)) {
                    Aggregate.Add(doubleClipped);
                    if (doubleClipped is PrimitiveFragment fragment) Fragments.Add(fragment);
                }
            }
        }
    }
}