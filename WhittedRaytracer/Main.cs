using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Linq;
using WhittedRaytracer.Multithreading;
using WhittedRaytracer.Raytracing;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Raytracing.SceneObjects.CameraObjects;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedRaytracer {
    /// <summary> Main class of the raytracer </summary>
    class Main {
        /// <summary> The amount of tasks created for the threadpool </summary>
        public const int RayTaskAmount = 720;

        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public readonly Scene Scene;
        /// <summary> Whether it is drawing debug information </summary>
        public bool Debug = false;
        /// <summary> Whether to draw rays in debug </summary>
        public bool DebugShowRays = true;

        readonly Stopwatch stopwatch = Stopwatch.StartNew();
        readonly Stopwatch frameTimer = Stopwatch.StartNew();
        readonly Threadpool threadpool;
        readonly Action[] tasks;

        KeyboardState keyboardState;

        public Main(IScreen screen) {
            Scene = Scene.DefaultWithRandomSpheres(screen, 10000);
            tasks = new Action[RayTaskAmount];
            threadpool = new Threadpool();
        }

        /// <summary> Process a single frame </summary>
        public void Tick() {
            Scene.Lights.First().Position += new Vector3((float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds) * 0.5f, 0, 0);
            Scene.Camera.ScreenPlane.Screen.Clear(0);
            InputCheck();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| OpenTK ms");
            stopwatch.Restart();
            DivideRayTracingWork();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| Divide Tasks ms");
            stopwatch.Restart();
            threadpool.DoTasks(tasks);
            threadpool.WaitTillDone();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| Tracing ms");
            stopwatch.Restart();

            // Drawing
            Scene.Camera.ScreenPlane.DrawAccumulatedLight();
            if (Debug) {
                foreach (Primitive primitive in Scene.Primitives) if (primitive is Sphere) Scene.Camera.ScreenPlane.DrawSphere(primitive as Sphere);
                foreach (PointLight light in Scene.Lights) Scene.Camera.ScreenPlane.DrawLight(light);
                Scene.Camera.ScreenPlane.DrawScreenPlane();
                Scene.Camera.ScreenPlane.DrawCamera();
            }
            Scene.Camera.ScreenPlane.Screen.Print($"Frametime: {frameTimer.ElapsedMilliseconds}", 1, 1, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"FPS: {1000L / frameTimer.ElapsedMilliseconds}", 1, 17, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"FOV: {Scene.Camera.FOV}", 1, 33, 0xffffff);
            frameTimer.Restart();
        }

        /// <summary> Check if there was any input </summary>
        void InputCheck() {
            keyboardState = Keyboard.GetState();
            if (keyboardState[Key.F1]) Debug = !Debug;
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

        void DivideRayTracingWork() {
            float size = (float)Scene.Camera.ScreenPlane.Screen.Height * Scene.Camera.ScreenPlane.Screen.Width / RayTaskAmount;
            float[] taskBounds = new float[RayTaskAmount + 1];
            taskBounds[0] = 0;
            for (int n = 0; n < RayTaskAmount; n++) {
                taskBounds[n + 1] = taskBounds[n] + size;
                int taskLower = (int)taskBounds[n];
                int taskUpper = (int)taskBounds[n + 1];
                tasks[n] = () => TraceRays(taskLower, taskUpper);
            }
        }

        void TraceRays(int from, int to) {
            for (int i = from; i < to; i++) {
                TraceRay(i % Scene.Camera.ScreenPlane.Screen.Width, i / Scene.Camera.ScreenPlane.Screen.Width);
            }
        }

        void TraceRay(int x, int y) {
            bool debugRay = Debug && DebugShowRays && (x % 64 == 0 || x == Scene.Camera.ScreenPlane.Screen.Width) && y == Scene.Camera.ScreenPlane.Screen.Height / 2;
            Ray ray = Scene.Camera.CreatePrimaryRay(x, y);
            Vector3 pixelColor = Scene.CastRay(ray, 0, debugRay);
            Scene.Camera.ScreenPlane.Accumulator.AddPhoton(x, y, pixelColor);
        }
    }
}