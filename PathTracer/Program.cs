using OpenTK.Mathematics;
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
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;

namespace PathTracer {
    public static class Program {
        #region Default Scene Definition
        /// <summary> The primitives in the default scene </summary>
        public static readonly List<ISceneObject> DefaultPrimitives = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBSpectrum.White)),
            new Primitive(new Sphere(new Position3(-3, 1, 5), 1), Material.Diffuse(RGBSpectrum.Green)),
            new Primitive(new Sphere(new Position3(3, 1, 5), 1), Material.Glossy(RGBSpectrum.Red, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.Glossy(RGBSpectrum.OffWhite, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 8), 1), Material.Specular(RGBSpectrum.OffWhite)),
            new Primitive(new Sphere(new Position3(-1, 1, 2), 1), Material.IsotropicVolumetric(RGBSpectrum.LightGray, 2f)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.SpecularDiffuseBlend(RGBSpectrum.Purple, 0.5f)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBSpectrum.Yellow)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBSpectrum.Gray)),
            //new Primitive(new AxisAlignedBox(new Position3(-5, 0, 0), new Position3(5, 2, 10)), new DiffuseVolumetric(new RGBSpectrum(0.8f, 0.8f, 0.8f), 0.2)),
        };

        public static readonly List<ISceneObject> DefaultLights = DefaultPrimitives.FindAll(s => s is IPrimitive p && p.Material.EmittanceProfile.IsEmitting);
        #endregion

        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new();
        /// <summary> The configuration of the renderer </summary>
        public static readonly Config Config = Config.LoadFromFile();
        /// <summary> The window supplied by OpenTK to render to </summary>
        public static readonly RenderWindow Window = new(
            new GameWindowSettings() {
                UpdateFrequency = 0,
                RenderFrequency = 0,
                IsMultiThreaded = false,
            }, 
            new NativeWindowSettings() {
                Location = Config.WindowPosition,
                Size = Config.WindowSize,
                Title = "C# .NET 5 OpenTK Pathtracer",
                WindowBorder = WindowBorder.Resizable,
            });
        /// <summary> The <see cref="IObserver"/> viewing the scene </summary>
        public static readonly IObserver Observer = new Observer(Window.GameWindow, new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.FOV)) {
            DrawingMode = Config.DrawingMode,
            DebugInfo = Config.DebugInfo,
            DebugColor = Config.DebugColor,
            CameraLock = Config.CameraLock,
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
            /// Compute Blender Rotation Quaternion
            var blenderRotation = new Quaternion(-Config.Rotation.X, -Config.Rotation.Z, -Config.Rotation.Y, Config.Rotation.W);
            var blenderFrontCompensation = new Quaternion(0.707107f, 0f, 0f, 0.707107f);
            Console.WriteLine($"Blender Rotation Quaternion | {blenderRotation * blenderFrontCompensation}");

            /// Start Renderer
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Run();
            Window.Dispose();
            Threadpool.Dispose();
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
