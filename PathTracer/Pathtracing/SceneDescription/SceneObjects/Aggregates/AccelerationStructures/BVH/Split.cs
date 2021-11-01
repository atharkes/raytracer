using PathTracer.Geometry.Normals;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH {
    /// <summary> A possible split for a BVHNode </summary>
    public class Split {
        /// <summary> Along which direction the split is </summary>
        public Normal3 Direction { get; }
        /// <summary> The left AABB of the split </summary>
        public IAggregate Left { get; }
        /// <summary> The right AABB of the split </summary>
        public IAggregate Right { get; }

        /// <summary> The Surface Area Heuristic of the split </summary>
        public float SurfaceAreaHeuristic => BVH.SurfaceAreaHeuristic(Left.ItemCount, Left.SurfaceArea) + BVH.SurfaceAreaHeuristic(Right.ItemCount, Right.SurfaceArea);

        /// <summary> Create a new split with AABB's </summary>
        /// <param name="left">The left AABB</param>
        /// <param name="right">The right AABB</param>
        public Split(Normal3 direction, IAggregate left, IAggregate right) {
            Direction = direction;
            Left = left;
            Right = right;
        }

        /// <summary> Create a new split with primitives </summary>
        /// <param name="primitivesLeft">The primitives for the left AABB</param>
        /// <param name="primitivesRight">The primitives for the right AABB</param>
        public Split(Normal3 direction, IEnumerable<ISceneObject> primitivesLeft, IEnumerable<ISceneObject> primitivesRight) {
            Direction = direction;
            Left = new Aggregate(primitivesLeft);
            Right = new Aggregate(primitivesRight);
        }
    }
}
