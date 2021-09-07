using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures {
    public abstract class AccelerationStructure : Aggregate {
        public AccelerationStructure() : base() { }
        public AccelerationStructure(IEnumerable<ISceneObject> items) : base(items) { }
    }
}
