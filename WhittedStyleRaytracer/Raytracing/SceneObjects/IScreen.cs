using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhittedStyleRaytracer.Raytracing.SceneObjects {
    public interface IScreen {
        int Width { get; }
        int Height { get; }

        void Clear(int color);
        void Plot(int x, int y, int color);
        void Line(int x1, int y1, int x2, int y2, int color);
        void Box(int x1, int y1, int x2, int y2, int color);
        void Print(string text, int x, int y, int color);
    }
}
