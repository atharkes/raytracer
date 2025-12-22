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
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;
using SimpleImageIO;
using System.Diagnostics;
using System.Globalization;

namespace PathTracer;

public static class Program {
    #region Scene Definitions
    /// <summary> Simple test scene </summary>
    public static readonly List<ISceneObject> Test = new() {
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.Diffuse(RGBColors.Red)),
    };

    /// <summary> The primitives in the default scene </summary>
    public static readonly List<ISceneObject> LuxCoreComparison = new() {
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Yellow)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.SpecularDiffuseBlend(RGBColors.Purple, 0.5f)),
        new Primitive(new Sphere(new Position3(-3, 1, 5), 1), Material.Diffuse(RGBColors.Green)),
        new Primitive(new Sphere(new Position3(3, 1, 5), 1), Material.Glossy(RGBColors.Red, 0.2f)),
        new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.Glossy(RGBColors.OffWhite, 0.2f)),
        new Primitive(new Sphere(new Position3(0, 1, 8), 1), Material.Specular(RGBColors.OffWhite)),
        new Primitive(new Sphere(new Position3(-1, 1, 2), 1), Material.IsotropicVolumetric(RGBColors.OffWhite, 2f)),
    };

    /// <summary> The scene primitives to test roughness and density of volumetric </summary>
    public static List<ISceneObject> RoughnessDensityComparison(float density, float roughness) => new() {
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.DarkGray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.Specular(RGBColors.Gray)),
        new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.SpecularParticleCloud(RGBColors.OffWhite, density, roughness)),
        new Primitive(new AxisAlignedBox(new Position3(2, 0, 5), new Position3(4, 2, 7)), Material.Diffuse(RGBColors.Blue)),
        new Primitive(Tetrahedron.Regular(new Position3(-1.25f, 0, 4.75f), .5f), Material.Diffuse(RGBColors.Green)),
    };

    /// <summary> Simple test scene </summary>
    public static readonly List<ISceneObject> SurfaceIntervalSizeComparison = new() {
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new Plane(new Normal3(0, 0, 1), new Position1(-1)), Material.Specular(RGBColors.OffWhite)),
        new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.Diffuse(RGBColors.Red)),
    };
    #endregion

    /// <summary> The threadpool of this application </summary>
    public static readonly Threadpool Threadpool = new();
    /// <summary> The configuration of the renderer </summary>
    public static readonly Config Config = Config.LoadFromFile();
    /// <summary> The <see cref="IIntegrator"/> to integrate the scene </summary>
    public static readonly IIntegrator Integrator = new BackwardsSampler();
    /// <summary> The <see cref="GameWindow"/> settings </summary>
    public static readonly GameWindowSettings GameWindowSettings = new() {
        UpdateFrequency = 0,
    };
    /// <summary> The <see cref="NativeWindow"/> settings </summary>
    public static readonly NativeWindowSettings NativeWindowSettings = new() {
        Location = Config.WindowPosition,
        ClientSize = Config.WindowSize,
        Title = ".NET OpenTK Pathtracer",
        WindowBorder = WindowBorder.Resizable,
    };

    /// <summary> Entry point of the application </summary>
    public static void Main() {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        RunGameWindow(RoughnessDensityComparison(2, 2));
        Threadpool.Dispose();
    }

    private static void RunGameWindow(List<ISceneObject> sceneObjects) {
        /// Setup
        var window = new RenderWindow(GameWindowSettings, NativeWindowSettings);
        var camera = new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.HorizontalFOV);
        var observer = new InteractiveObserver(window, camera) {
            Drawing = Config.Drawing,
            Debug = Config.Debug,
            TextColor = Config.TextColor,
            CameraLock = Config.CameraLock,
        };
        var scene = new Scene(observer.Camera, sceneObjects);
        IRenderer renderer = new Renderer(scene, Integrator, observer);

        /// Attach to Game Window
        void UpdateRenderer(FrameEventArgs obj) => renderer.Render((observer as IInteractiveObserver).TargetFrameTime);

        void HandleInput(FrameEventArgs obj) => observer.HandleInput(window.KeyboardState, window.MouseState);

        window.UpdateFrame += HandleInput;
        window.RenderFrame += UpdateRenderer;

        window.Run();

        window.UpdateFrame -= HandleInput;
        window.RenderFrame -= UpdateRenderer;

        Config.SaveToFile(renderer);

        window.Dispose();
    }

    private static void CreateLuxCoreComparisonImage(int mins = 30) {
        /// Setup
        var camera = new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.HorizontalFOV);
        var observer = new Observer(camera, Config.WindowWidth, Config.WindowHeight);
        var scene = new Scene(observer.Camera, LuxCoreComparison);
        var renderer = new Renderer(scene, Integrator, observer);
        /// Render
        var renderTime = new TimeSpan(0, mins, 00);
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < renderTime) {
            renderer.Render(renderTime - timer.Elapsed);
        }
        /// Output
        OutputImage(observer, $"pathtracer-mins{renderTime.TotalMinutes}");
    }

    private static void CreateSelfIntersectionImages() {
        var renderTime = new TimeSpan(0, 1, 00);
        int[] bitLengthValues = { 1, 2, 3, 4, 5, 18, 19, 20, 21, 22 };
        foreach (var bitLength in bitLengthValues) {
            Console.WriteLine($"Time = {DateTime.Now.TimeOfDay:c} | Currently at: bit length = {bitLength:0.0}");
            var intervalLength = 1u << (bitLength - 1);
            InverseMultiplicative.IntervalLength = intervalLength;

            /// Setup
            var camera = new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.HorizontalFOV);
            var observer = new Observer(camera, Config.WindowWidth, Config.WindowHeight);
            var scene = new Scene(observer.Camera, SurfaceIntervalSizeComparison);
            var renderer = new Renderer(scene, Integrator, observer);
            /// Render
            var timer = Stopwatch.StartNew();
            while (timer.Elapsed < renderTime) {
                renderer.Render(renderTime - timer.Elapsed);
            }
            /// Output
            OutputImage(observer, $"bitLength{bitLength:0.0}");
        }
    }

    private static void CreateDensityRoughnessImages() {
        var renderTime = new TimeSpan(0, 20, 00);
        var densityValues = new float[] { 0.5f, 2f, 8f, 32f };
        var rougnessValues = new float[] { 0f, 0.1f, 0.2f, 0.5f, 1f };
        foreach (var density in densityValues) {
            foreach (var roughness in rougnessValues) {
                Console.WriteLine($"Time = {DateTime.Now.TimeOfDay:c} | Currently at: density = {density:0.0}, roughness = {roughness:0.00}");
                /// Setup
                var camera = new PinholeCamera(Config.Position, Config.Rotation, Config.AspectRatio, Config.HorizontalFOV);
                var observer = new Observer(camera, Config.WindowWidth, Config.WindowHeight);
                var scene = new Scene(observer.Camera, RoughnessDensityComparison(density, roughness));
                var renderer = new Renderer(scene, Integrator, observer);
                /// Render
                var timer = Stopwatch.StartNew();
                while (timer.Elapsed < renderTime) {
                    renderer.Render(renderTime - timer.Elapsed);
                }
                /// Output
                OutputImage(observer, $"density{density:0.0}roughness{roughness:0.00}");
            }
        }
    }

    private static void OutputImage(IObserver observer, string filename) {
        RgbImage img = new(w: observer.Accumulator.Width, h: observer.Accumulator.Height);
        for (var y = 0; y < observer.Accumulator.Height; y++) {
            for (var x = 0; x < observer.Accumulator.Width; x++) {
                var color = observer.Accumulator.Get(x, y).AverageLight.ToRGBSpectrum();
                img.SetPixel(x, y, new(color.Red, color.Green, color.Blue));
            }
        }
        img.WriteToFile($"{filename}.exr");
    }
}
