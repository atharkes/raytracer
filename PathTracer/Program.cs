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
using SimpleImageIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathTracer {
    public static class Program {
        #region Scene Definitions
        /// <summary> The primitives in the default scene </summary>
        public static readonly List<ISceneObject> DefaultPrimitives = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.Gray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Yellow)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.SpecularDiffuseBlend(RGBColors.Purple, 0.5f)),
            new Primitive(new AxisAlignedBox(new Position3(-5, 0, 0), new Position3(5, 3f, 10)), Material.DiffuseParticleCloud(RGBColors.OffWhite, 0.2f)),
            new Primitive(new Sphere(new Position3(-3, 1, 5), 1), Material.Diffuse(RGBColors.Green)),
            new Primitive(new Sphere(new Position3(3, 1, 5), 1), Material.Glossy(RGBColors.Red, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.Glossy(RGBColors.OffWhite, 0.2f)),
            new Primitive(new Sphere(new Position3(0, 1, 8), 1), Material.Specular(RGBColors.OffWhite)),
            new Primitive(new Sphere(new Position3(-1, 1, 2), 1), Material.SpecularParticleCloud(RGBColors.OffWhite, 4f, 0.1f)),
        };

        /// <summary> The scene primitives to test roughness and density of volumetric </summary>
        public static List<ISceneObject> RoughnessDensityTest(float density, float roughness) => new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.DarkGray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Gray)),
            new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.Specular(RGBColors.Gray)),
            new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.SpecularParticleCloud(RGBColors.OffWhite, density, roughness)),
            new Primitive(new AxisAlignedBox(new Position3(2, 0, 5), new Position3(4, 2, 7)), Material.Diffuse(RGBColors.Blue)),
            new Primitive(Tetrahedron.Regular(new Position3(-1.25f, 0, 4.75f), .5f), Material.Diffuse(RGBColors.Green)),
        };

        /// <summary> Simple test scene </summary>
        public static readonly List<ISceneObject> Test = new() {
            new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBColors.DarkGray)),
            new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.SpecularParticleCloud(RGBColors.Red, 1024f, 0.0f)),
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
        /// <summary> The <see cref="ICamera"/> viewing the scene </summary>
        public static readonly ICamera Camera = new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.FOV);
        /// <summary> The <see cref="IObserver"/> viewing the scene </summary>
        public static readonly IObserver Observer = new Observer(Window, Camera) {
            DrawingMode = Config.DrawingMode,
            DebugInfo = Config.DebugInfo,
            DebugColor = Config.DebugColor,
            CameraLock = Config.CameraLock,
        };
        /// <summary> The <see cref="IIntegrator"/> to integrate the scene </summary>
        public static readonly IIntegrator Integrator = new BackwardsSampler();
        /// <summary> The scene to render </summary>
        public static readonly IScene Scene = new Scene(Observer.Camera, RoughnessDensityTest(2f, 0.1f));
        /// <summary> The <see cref="IRenderer"/> to supply images </summary>
        public static readonly IRenderer Renderer = new Renderer(Scene, Integrator, Observer);

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            RunTests();
        }

        static void RunGameWindow() {
            OutputBlenderInformation();

            void UpdateRenderer(FrameEventArgs obj) => Renderer.Render(Renderer.Observer.TargetFrameTime);
            void HandleInput(FrameEventArgs obj) => Renderer.Observer.HandleInput(Window.KeyboardState, Window.MouseState);

            /// Run Renderer
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Run();

            /// Dispose
            Window.UpdateFrame -= HandleInput;
            Window.RenderFrame -= UpdateRenderer;
            Window.Dispose();
            Threadpool.Dispose();

            /// Output
            Config.SaveToFile(Renderer);
        }

        static void RunTests() {
            TimeSpan renderTime = new(0, 0, 30);
            float[] densityValues = { 0.5f, 2f, 8f, 32f };
            float[] rougnessValues = { 0f, 0.1f, 0.2f, 0.5f, 1f };
            foreach (float density in densityValues) {
                foreach (float roughness in rougnessValues) {
                    Console.WriteLine($"Time = {DateTime.Now.TimeOfDay:c} | Currently at: density = {density:0.0}, roughness = {roughness:0.00}");
                    /// Setup
                    IObserver observer = new Observer(Window, Camera);
                    IScene scene = new Scene(observer.Camera, RoughnessDensityTest(density, roughness));
                    IRenderer renderer = new Renderer(scene, Integrator, observer);
                    /// Render
                    var timer = Stopwatch.StartNew();
                    while (timer.Elapsed < renderTime) {
                        renderer.Render(renderTime - timer.Elapsed);
                    }
                    /// Output
                    OutputImage($"density{density:0.0}roughness{roughness:0.00}");
                }
            }
        }

        static void OutputBlenderInformation() {
            /// Compute Blender Rotation Quaternion
            var blenderRotation = new Quaternion(-Config.Rotation.X, -Config.Rotation.Z, -Config.Rotation.Y, Config.Rotation.W);
            var blenderFrontCompensation = new Quaternion((float)Math.Sqrt(2) / 2f, 0f, 0f, (float)Math.Sqrt(2) / 2f);
            Console.WriteLine($"Blender Rotation Quaternion | {blenderRotation * blenderFrontCompensation}");
        }

        static void OutputImage(string filename) {
            RgbImage img = new(w: Config.WindowWidth, h: Config.WindowHeight);
            for (int y = 0; y < Config.WindowHeight; y++) {
                for (int x = 0; x < Config.WindowWidth; x++) {
                    RGBSpectrum color = Observer.Accumulator.Get(x, y).AverageLight.ToRGBSpectrum();
                    img.SetPixel(x, y, new(color.Red, color.Green, color.Blue));
                }
            }
            img.WriteToFile($"{filename}.exr");
        }
    }
}
