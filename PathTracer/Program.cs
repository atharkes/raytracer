using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PathTracer.Drawing;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Multithreading;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.Integrators;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials;
using PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;
using System.Collections.Generic;

namespace PathTracer {
    public static class Program {
        #region Default Scene Definition
        /// <summary> The primitives in the default scene </summary>
        public static readonly List<ISceneObject> DefaultPrimitives = new() {
            new Primitive(new InfinityPlane(), ParametricMaterial.WhiteLight),
            new Primitive(new Sphere(new Position3(-3, 1, 5), 1), ParametricMaterial.DiffuseGreen),
            new Primitive(new Sphere(new Position3(3, 1, 5), 1), ParametricMaterial.GlossyRed),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), ParametricMaterial.Mirror),
            new Primitive(new Sphere(new Position3(-1, 1, 2), 1), new DiffuseVolumetric(new RGBSpectrum(0.8f, 0.8f, 0.8f), 4)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), ParametricMaterial.GlossyPurpleMirror),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), ParametricMaterial.DiffuseYellow),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), ParametricMaterial.DiffuseGray),
            new Primitive(new AxisAlignedBox(new Position3(-5, 0, 0), new Position3(5, 2, 10)), new DiffuseVolumetric(new RGBSpectrum(0.8f, 0.8f, 0.8f), 0.2)),
        };

        public static readonly List<ISceneObject> DefaultLights = DefaultPrimitives.FindAll(s => s is IPrimitive p && p.Material.IsEmitting);
        #endregion

        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new();
        /// <summary> The configuration of the renderer </summary>
        public static readonly Config Config = Config.LoadFromFile();
        /// <summary> The window supplied by OpenTK to render to </summary>
        public static readonly RenderWindow Window = new(GameWindowSettings.Default, NativeWindowSettings.Default) {
            RenderFrequency = 0,
            UpdateFrequency = 0,
            Location = Config.WindowPosition,
            Size = Config.WindowSize,
            Title = "C# .NET 5 OpenTK Pathtracer",
            WindowBorder = WindowBorder.Resizable
        };
        /// <summary> The <see cref="IObserver"/> viewing the scene </summary>
        public static readonly IObserver Observer = new Observer(Window.GameWindow, new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.FOV)) {
            DrawingMode = Config.DrawingMode,
            DebugInfo = Config.DebugInfo
        };
        /// <summary> The <see cref="IIntegrator"/> to integrate the scene </summary>
        public static readonly IIntegrator Integrator = new BackwardsSampler();
        /// <summary> The scene to render </summary>
        public static readonly IScene Scene = new Scene(Observer.Camera, DefaultLights, DefaultPrimitives);
        /// <summary> The <see cref="IRenderer"/> to supply images </summary>
        public static readonly IRenderer Renderer = new Renderer(Scene, Integrator, Observer);

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Run();
            Config.SaveToFile(Renderer);
        }

        static void UpdateRenderer(FrameEventArgs obj) {
            Renderer.Render(Renderer.Observer.TargetFrameTime);
        }

        static void HandleInput(FrameEventArgs obj) {
            Renderer.Observer.HandleInput(Window.KeyboardState, Window.MouseState);
        }
    }
}
