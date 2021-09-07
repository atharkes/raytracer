using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer {
    /// <summary> A renderer that uses an <see cref="IIntegrator"/> to render a <see cref=""/>. </summary>
    public interface IRenderer {
        /// <summary> The <see cref="IScene"/> to be rendered </summary>
        IScene Scene { get; }
        /// <summary> The <see cref="IIntegrator"/> to render the scene </summary>
        IIntegrator Integrator { get; }
        /// <summary> The <see cref="IObserver"/> that views the scene </summary>
        IObserver Observer { get; }
    }
}
