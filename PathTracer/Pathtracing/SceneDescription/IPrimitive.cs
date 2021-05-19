using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Pathtracing.SceneDescription {
    public interface IPrimitive : ISceneObject, IMaterial {
        IMaterial Material { get; }
        IShape Shape { get; }
    }
}
