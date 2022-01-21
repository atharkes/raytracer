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
        #region Scene Definitions
        /// <summary> The primitives in the default scene </summary>
        public static readonly List<ISceneObject> DefaultPrimitives = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBSpectrum.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBSpectrum.Gray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBSpectrum.Yellow)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.SpecularDiffuseBlend(RGBSpectrum.Purple, 0.5f)),
            new Primitive(new AxisAlignedBox(new Position3(-5, 0, 0), new Position3(5, 3f, 10)), Material.DiffuseParticleCloud(RGBSpectrum.OffWhite, 0.2f)),
            new Primitive(new Sphere(new Position3(-3, 1, 5), 1), Material.Diffuse(RGBSpectrum.Green)),
            new Primitive(new Sphere(new Position3(3, 1, 5), 1), Material.Glossy(RGBSpectrum.Red, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.Glossy(RGBSpectrum.OffWhite, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 8), 1), Material.Specular(RGBSpectrum.OffWhite)),
            new Primitive(new Sphere(new Position3(-1, 1, 2), 1), Material.SpecularParticleCloud(RGBSpectrum.OffWhite, 4f, 0.1f)),
        };

        /// <summary> The scene primitives to test roughness and density of volumetric </summary>
        public static readonly List<ISceneObject> RoughnessDensityTest = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBSpectrum.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBSpectrum.DarkGray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBSpectrum.Gray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.Specular(RGBSpectrum.Gray)),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.SpecularParticleCloud(RGBSpectrum.OffWhite, 80000f, 0.2f)),
            new Primitive(new AxisAlignedBox(new Position3(2, 0, 5), new Position3(4, 2, 7)), Material.Diffuse(RGBSpectrum.Blue)),
            new Primitive(new AxisAlignedBox(new Position3(-1.5f, 0, 4.5f), new Position3(-1, .5f, 5)), Material.Diffuse(RGBSpectrum.Green)),
        };

        /// <summary> Simple test scene </summary>
        public static readonly List<ISceneObject> Test = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBSpectrum.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBSpectrum.DarkGray)),
            new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.Glossy(RGBSpectrum.Red, 0.2f)), // Broken due to origin
        };
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
        public static readonly IObserver Observer = new Observer(Window, new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.FOV)) {
            DrawingMode = Config.DrawingMode,
            DebugInfo = Config.DebugInfo,
            DebugColor = Config.DebugColor,
            CameraLock = Config.CameraLock,
        };
        /// <summary> The <see cref="IIntegrator"/> to integrate the scene </summary>
        public static readonly IIntegrator Integrator = new BackwardsSampler();
        /// <summary> The scene to render </summary>
        public static readonly IScene Scene = new Scene(Observer.Camera, Test);
        /// <summary> The <see cref="IRenderer"/> to supply images </summary>
        public static readonly IRenderer Renderer = new Renderer(Scene, Integrator, Observer);

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            /// Compute Blender Rotation Quaternion
            var blenderRotation = new Quaternion(-Config.Rotation.X, -Config.Rotation.Z, -Config.Rotation.Y, Config.Rotation.W);
            var blenderFrontCompensation = new Quaternion((float)Math.Sqrt(2) / 2f, 0f, 0f, (float)Math.Sqrt(2) / 2f);
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
