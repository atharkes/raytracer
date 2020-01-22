using OpenTK;
using OpenTK.Input;
using System;
using WhittedRaytracer.Multithreading;
using WhittedRaytracer.Raytracing;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Raytracing.SceneObjects.CameraParts;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;
using WhittedRaytracer.Utilities;

namespace WhittedRaytracer {
    /// <summary> Main class of the raytracer </summary>
    class Main {
        public static readonly Threadpool Threadpool = new Threadpool();
        /// <summary> The amount of tasks created for the threadpool </summary>
        public const int MultithreadingTaskCount = 720;
        /// <summary> The targeted framerate of the raytracer </summary>
        public const int TargetFrameRate = 20;
        /// <summary> The targeted frame time computed from the targeted frame rate </summary>
        public static TimeSpan TargetFrameTime => new TimeSpan(0, 0, 0, 0, 1000 / TargetFrameRate);
        /// <summary> Minimum amount of rays to trace in a tick </summary>
        public const int MinimumRayCount = 10_000;

        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public readonly Scene Scene;
        /// <summary> Statistics of the raytracer </summary>
        public readonly Statistics Stats = new Statistics();

        /// <summary> Whether it is drawing debug information </summary>
        public bool Debug = false;
        /// <summary> Whether to draw rays in debug </summary>
        public bool DebugShowRays = true;
        
        readonly Action[] tasks;

        public Main(IScreen screen) {
            Scene = Scene.DefaultWithRandomSpheres(screen, 10_000);
            tasks = new Action[MultithreadingTaskCount];
        }

        /// <summary> Process a single frame </summary>
        public void Tick() {
            Stats.LogFrameTime();
            InputCheck();
            Stats.LogTaskTime(Stats.OpenTKTime);
            DivideRayTracingTasks();
            Stats.LogTaskTime(Stats.MultithreadingOverhead);
            Threadpool.DoTasks(tasks);
            Threadpool.WaitTillDone();
            Stats.LogTaskTime(Stats.TracingTime);

            // Drawing
            Scene.Camera.ScreenPlane.Screen.Clear(0);
            Scene.Camera.ScreenPlane.DrawAccumulator();
            if (Debug) {
                foreach (Primitive primitive in Scene.Primitives) if (primitive is Sphere) Scene.Camera.ScreenPlane.DrawSphere(primitive as Sphere);
                foreach (Primitive primitive in Scene.Lights) if (primitive is PointLight) Scene.Camera.ScreenPlane.DrawLight(primitive as PointLight);
                Scene.Camera.ScreenPlane.DrawScreenPlane();
                Scene.Camera.ScreenPlane.DrawCamera();
            }
            Scene.Camera.ScreenPlane.Screen.Print($"FPS: {1000 / (int)Stats.FrameTime.LastTick.TotalMilliseconds}", 1, 1, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"Rays Traced: {Stats.RaysTracedLastTick}", 1, 17, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"Frame Time (ms): {(int)Stats.FrameTime.LastTick.TotalMilliseconds}", 1, 33, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"Tracing Time (ms): {(int)Stats.TracingTime.LastTick.TotalMilliseconds}", 1, 49, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"Drawing Time (ms): {(int)Stats.DrawingTime.LastTick.TotalMilliseconds}", 1, 65, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"OpenTK Time (ms): {(int)Stats.OpenTKTime.LastTick.TotalMilliseconds}", 1, 81, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"Multithreading Overhead (ms): {(int)Stats.MultithreadingOverhead.LastTick.TotalMilliseconds}", 1, 97, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"FOV: {Scene.Camera.FOV}", 1, 113, 0xffffff);
            Stats.LogTaskTime(Stats.DrawingTime);
        }

        /// <summary> Check if there was any input </summary>
        void InputCheck() {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState[Key.F1]) Debug = !Debug;
            if (keyboardState[Key.F2]) Scene.Camera.ScreenPlane.Accumulator.DrawBVH = !Scene.Camera.ScreenPlane.Accumulator.DrawBVH;
            if (keyboardState[Key.Space]) Scene.Camera.Move(Scene.Camera.Up);
            if (keyboardState[Key.LShift]) Scene.Camera.Move(Scene.Camera.Down);
            if (keyboardState[Key.W]) Scene.Camera.Move(Scene.Camera.Front);
            if (keyboardState[Key.S]) Scene.Camera.Move(Scene.Camera.Back);
            if (keyboardState[Key.A]) Scene.Camera.Move(Scene.Camera.Left);
            if (keyboardState[Key.D]) Scene.Camera.Move(Scene.Camera.Right);
            if (keyboardState[Key.KeypadPlus]) Scene.Camera.FOV *= 1.1f;
            if (keyboardState[Key.KeypadMinus]) Scene.Camera.FOV *= 0.9f;
            if (keyboardState[Key.Left]) Scene.Camera.Turn(Scene.Camera.Left);
            if (keyboardState[Key.Right]) Scene.Camera.Turn(Scene.Camera.Right);
            if (keyboardState[Key.Up]) Scene.Camera.Turn(Scene.Camera.Up);
            if (keyboardState[Key.Down]) Scene.Camera.Turn(Scene.Camera.Down);
        }

        void DivideRayTracingTasks() {
            int rayCount = Stats.RayCountNextTick();
            Stats.LogTickRays(rayCount);
            float size = rayCount / MultithreadingTaskCount;
            float[] taskBounds = new float[MultithreadingTaskCount + 1];
            taskBounds[0] = 0;
            for (int n = 0; n < MultithreadingTaskCount; n++) {
                taskBounds[n + 1] = taskBounds[n] + size;
                int taskLower = (int)taskBounds[n];
                int taskUpper = (int)taskBounds[n + 1];
                tasks[n] = () => TraceRays(taskLower, taskUpper);
            }
        }

        void TraceRays(int from, int to) {
            CameraRay[] rays = Scene.Camera.GetRandomCameraRays(to - from);
            for (int i = 0; i < rays.Length; i++) {
                int x = i % Scene.Camera.ScreenPlane.Screen.Width;
                int y = i / Scene.Camera.ScreenPlane.Screen.Width;
                bool debugRay = Debug && DebugShowRays && (x % 64 == 0 || x == Scene.Camera.ScreenPlane.Screen.Width) && y == Scene.Camera.ScreenPlane.Screen.Height / 2;
                Vector3 pixelColor = Scene.CastRay(rays[i], 0, debugRay);
                rays[i].Cavity.AddPhoton(pixelColor, rays[i].BVHTraversals);
            }
        }
    }
}