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
using PathTracer.Services;
using PathTracer.Utilities;
using SimpleImageIO;
using System.Diagnostics;
using System.Globalization;

namespace PathTracer;

public static class Program {
    #region Scene Definitions
    /// <summary> Simple test scene </summary>
    public static readonly List<ISceneObject> Test = [
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.Diffuse(RGBColors.Red)),
    ];

    /// <summary> The primitives in the default scene </summary>
    public static readonly List<ISceneObject> LuxCoreComparison = [
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Yellow)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.SpecularDiffuseBlend(RGBColors.Purple, 0.5f)),
        new Primitive(new Sphere(new Position3(-3, 1, 5), 1), Material.Diffuse(RGBColors.Green)),
        new Primitive(new Sphere(new Position3(3, 1, 5), 1), Material.Glossy(RGBColors.Red, 0.2f)),
        new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.Glossy(RGBColors.OffWhite, 0.2f)),
        new Primitive(new Sphere(new Position3(0, 1, 8), 1), Material.Specular(RGBColors.OffWhite)),
        new Primitive(new Sphere(new Position3(-1, 1, 2), 1), Material.IsotropicVolumetric(RGBColors.OffWhite, 2f)),
    ];

    /// <summary> The scene primitives to test roughness and density of volumetric </summary>
    public static List<ISceneObject> RoughnessDensityComparison(float density, float roughness) => [
        new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White)),
        new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(-1)), Material.Diffuse(RGBColors.DarkGray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(-5, 0, 0), new Position3(-5, 0, 10), null), Material.Diffuse(RGBColors.Gray)),
        new Primitive(new Triangle(new Position3(5, 0, 10), new Position3(5, 0, 0), new Position3(-5, 0, 0), null), Material.Specular(RGBColors.Gray)),
        new Primitive(new Sphere(new Position3(0, 1, 5), 1), Material.SpecularParticleCloud(RGBColors.OffWhite, density, roughness)),
        new Primitive(new AxisAlignedBox(new Position3(2, 0, 5), new Position3(4, 2, 7)), Material.Diffuse(RGBColors.Blue)),
        new Primitive(Tetrahedron.Regular(new Position3(-1.25f, 0, 4.75f), .5f), Material.Diffuse(RGBColors.Green)),
        .. ObjFileLoader.Load(Path.Combine(AppContext.BaseDirectory, "models", "stanford-bunny.obj"), new Position3(-3, 0, 6.5f), 1.5f)
    ];

    /// <summary> The scene primitives to test the interval length of the surface density profile </summary>
    public static List<ISceneObject> SurfaceIntervalSizeComparison(uint intervalLength) {
        var surface = new InverseMultiplicative(intervalLength);
        return [
            new Primitive(new InfinityPlane(), Material.Emitter(RGBColors.White).WithDensity(surface)),
            new Primitive(new Plane(new Normal3(0, 1, 0), new Position1(0)), Material.Diffuse(RGBColors.Gray).WithDensity(surface)),
            new Primitive(new Plane(new Normal3(0, 0, 1), new Position1(-1)), Material.Specular(RGBColors.OffWhite).WithDensity(surface)),
            new Primitive(new AxisAlignedBox(new Position3(0, 0, 0), new Position3(2, 2, 2)), Material.Diffuse(RGBColors.Red).WithDensity(surface)),
        ];
    }
    #endregion

    /// <summary> Entry point of the application </summary>
    public static void Main() {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        var config = Config.LoadFromFile();
        using var threadpool = new Threadpool();
        RunGameWindow(config, threadpool, RoughnessDensityComparison(2, 2));
    }

    private static void RunGameWindow(Config config, Threadpool threadpool, List<ISceneObject> sceneObjects) {
        /// Setup
        var gameWindowSettings = new GameWindowSettings() {
            UpdateFrequency = 0,
        };
        var nativeWindowSettings = new NativeWindowSettings() {
            Location = config.WindowPosition,
            ClientSize = config.WindowSize,
            Title = ".NET OpenTK Pathtracer",
            WindowBorder = WindowBorder.Resizable,
        };
        var window = new RenderWindow(gameWindowSettings, nativeWindowSettings);
        var camera = new PinholeCamera(config.Position, config.Rotation, config.AspectRatio, config.HorizontalFOV);
        var observer = new InteractiveObserver(window, camera, threadpool) {
            Drawing = config.Drawing,
            Debug = config.Debug,
            TextColor = config.TextColor,
            CameraLock = config.CameraLock,
        };
        var scene = new Scene(observer.Camera, sceneObjects);
        var renderer = new Renderer(scene, new BackwardsSampler(threadpool), observer);

        /// Attach to Game Window
        void UpdateRenderer(FrameEventArgs obj) => renderer.Render((observer as IInteractiveObserver).TargetFrameTime);

        void HandleInput(FrameEventArgs obj) => observer.HandleInput(window.KeyboardState, window.MouseState);

        window.UpdateFrame += HandleInput;
        window.RenderFrame += UpdateRenderer;

        window.Run();

        window.UpdateFrame -= HandleInput;
        window.RenderFrame -= UpdateRenderer;

        config.SaveToFile(renderer);

        window.Dispose();
    }

    private static void CreateLuxCoreComparisonImage(Config config, Threadpool threadpool, int mins = 30) {
        /// Setup
        var camera = new PinholeCamera(config.Position, config.Rotation, config.AspectRatio, config.HorizontalFOV);
        var observer = new Observer(camera, config.WindowWidth, config.WindowHeight, threadpool);
        var scene = new Scene(observer.Camera, LuxCoreComparison);
        var renderer = new Renderer(scene, new BackwardsSampler(threadpool), observer);
        /// Render
        var renderTime = new TimeSpan(0, mins, 00);
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < renderTime) {
            renderer.Render(renderTime - timer.Elapsed);
        }
        /// Output
        OutputImage(observer, $"pathtracer-mins{renderTime.TotalMinutes}");
    }

    private static void CreateSelfIntersectionImages(Config config, Threadpool threadpool) {
        var renderTime = new TimeSpan(0, 1, 00);
        int[] bitLengthValues = { 1, 2, 3, 4, 5, 18, 19, 20, 21, 22 };
        foreach (var bitLength in bitLengthValues) {
            Console.WriteLine($"Time = {DateTime.Now.TimeOfDay:c} | Currently at: bit length = {bitLength:0.0}");
            var intervalLength = 1u << (bitLength - 1);

            /// Setup
            var camera = new PinholeCamera(config.Position, config.Rotation, config.AspectRatio, config.HorizontalFOV);
            var observer = new Observer(camera, config.WindowWidth, config.WindowHeight, threadpool);
            var scene = new Scene(observer.Camera, SurfaceIntervalSizeComparison(intervalLength));
            var renderer = new Renderer(scene, new BackwardsSampler(threadpool), observer);
            /// Render
            var timer = Stopwatch.StartNew();
            while (timer.Elapsed < renderTime) {
                renderer.Render(renderTime - timer.Elapsed);
            }
            /// Output
            OutputImage(observer, $"bitLength{bitLength:0.0}");
        }
    }

    private static void CreateDensityRoughnessImages(Config config, Threadpool threadpool) {
        var renderTime = new TimeSpan(0, 20, 00);
        var densityValues = new float[] { 0.5f, 2f, 8f, 32f };
        var rougnessValues = new float[] { 0f, 0.1f, 0.2f, 0.5f, 1f };
        foreach (var density in densityValues) {
            foreach (var roughness in rougnessValues) {
                Console.WriteLine($"Time = {DateTime.Now.TimeOfDay:c} | Currently at: density = {density:0.0}, roughness = {roughness:0.00}");
                /// Setup
                var camera = new PinholeCamera(config.Position, config.Rotation, config.AspectRatio, config.HorizontalFOV);
                var observer = new Observer(camera, config.WindowWidth, config.WindowHeight, threadpool);
                var scene = new Scene(observer.Camera, RoughnessDensityComparison(density, roughness));
                var renderer = new Renderer(scene, new BackwardsSampler(threadpool), observer);
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
                var color = observer.Accumulator.Get(x, y).AverageLight;
                img.SetPixel(x, y, new(color.Red, color.Green, color.Blue));
            }
        }
        img.WriteToFile($"{filename}.exr");
    }
}
