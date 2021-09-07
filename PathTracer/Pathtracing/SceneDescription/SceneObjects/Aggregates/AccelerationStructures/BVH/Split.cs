using OpenTK.Mathematics;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH {
    /// <summary> A possible split for a BVHNode </summary>
    public class Split {
        /// <summary> Along which direction the split is </summary>
        public Vector3 Direction { get; }
        /// <summary> The left AABB of the split </summary>
        public IAggregate Left { get; }
        /// <summary> The right AABB of the split </summary>
        public IAggregate Right { get; }

        /// <summary> The Surface Area Heuristic of the split </summary>
        public float SurfaceAreaHeuristic => Left.SurfaceAreaHeuristic + Right.SurfaceAreaHeuristic;

        /// <summary> Create an empty split </summary>
        public Split() {
            Direction = Vector3.Zero;
            Left = new Aggregate();
            Right = new Aggregate();
        }

        /// <summary> Create a new split with AABB's </summary>
        /// <param name="left">The left AABB</param>
        /// <param name="right">The right AABB</param>
        public Split(Vector3 direction, IAggregate left, IAggregate right) {
            Direction = direction;
            Left = left;
            Right = right;
        }

        /// <summary> Create a new split with primitives </summary>
        /// <param name="primitivesLeft">The primitives for the left AABB</param>
        /// <param name="primitivesRight">The primitives for the right AABB</param>
        public Split(Vector3 direction, IEnumerable<ISceneObject> primitivesLeft, IEnumerable<ISceneObject> primitivesRight) {
            Direction = direction;
            Left = new Aggregate(primitivesLeft);
            Right = new Aggregate(primitivesRight);
        }
    }
}
