using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer.Integrators {
    public abstract class Integrator : IIntegrator {
        public abstract void Integrate(IScene scene);
    }
}
