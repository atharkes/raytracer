using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Linq;
using WhittedRaytracer.Multithreading;
using WhittedRaytracer.Raytracing;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedRaytracer {
    /// <summary> Main class of the raytracer </summary>
    class Main {
        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public readonly Scene Scene;
        /// <summary> Amount of tasks the raytracing is divided into </summary>
        public const int TaskAmount = 512;
        /// <summary> Whether it is drawing debug information </summary>
        public bool Debug = false;
        /// <summary> Whether to draw rays in debug </summary>
        public bool DebugShowRays = true;

        readonly Stopwatch stopwatch = Stopwatch.StartNew();
        readonly Stopwatch frameTimer = Stopwatch.StartNew();
        readonly Threadpool threadpool;
        readonly Action[] tasks;

        KeyboardState keyboardState;
        MouseState mouseStatePrevious;
        MouseState mouseStateCurrent;

        public Main(IScreen screen) {
            Scene = new Scene(screen);
            tasks = new Action[TaskAmount];
            threadpool = new Threadpool();
        }

        public void Tick() {
            // Move Light
            Scene.Lights.First().Position += new Vector3((float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds) * 0.5f, 0, 0);

            // Clear the screen
            Scene.Camera.ScreenPlane.Screen.Clear(0);

            // Check Input
            InputCheck();

            // Trace Rays
            DivideRays();

            // Debug
            if (Debug) {
                Scene.Primitives.ForEach(primitive => { if (primitive is Sphere) Scene.Camera.ScreenPlane.DrawSphere(primitive as Sphere); });
                Scene.Lights.ForEach(light => Scene.Camera.ScreenPlane.DrawLight(light));
                Scene.Camera.ScreenPlane.DrawScreenPlane();
                Scene.Camera.ScreenPlane.DrawCamera();
            }
            Scene.Camera.ScreenPlane.Screen.Print($"Frametime: {frameTimer.ElapsedMilliseconds}", 1, 1, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"FPS: {1000L / frameTimer.ElapsedMilliseconds}", 1, 17, 0xffffff);
            Scene.Camera.ScreenPlane.Screen.Print($"FOV: {Scene.Camera.FOV}", 1, 33, 0xffffff);
            frameTimer.Restart();
        }

        void InputCheck() {
            // Input: Keyboard
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
            // Input: Mouse
            mouseStateCurrent = Mouse.GetState();
            if (mouseStatePrevious != null) {
                float xDelta = mouseStateCurrent.X - mouseStatePrevious.X;
                float yDelta = mouseStateCurrent.Y - mouseStatePrevious.Y;
                Scene.Camera.Turn(xDelta * Scene.Camera.Right + yDelta * Scene.Camera.Down);
            }
            mouseStatePrevious = mouseStateCurrent;
        }

        // Multithreading
        void DivideRays() {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| OpenTK ms");
            stopwatch.Restart();
            float size = (float)Scene.Camera.ScreenPlane.Screen.Height / TaskAmount;
            float[] taskBounds = new float[TaskAmount + 1];
            taskBounds[0] = 0;
            for (int n = 0; n < TaskAmount; n++) {
                taskBounds[n + 1] = taskBounds[n] + size;
                int taskLower = (int)taskBounds[n];
                int taskUpper = (int)taskBounds[n + 1];
                tasks[n] = () => TraceRays(0, Scene.Camera.ScreenPlane.Screen.Width, taskLower, taskUpper);
            }
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| Divide Tasks ms");
            stopwatch.Restart();
            threadpool.DoTasks(tasks);
            threadpool.WaitTillDone();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}\t| Tracing ms");
            stopwatch.Restart();
        }

        void TraceRays(int xMin, int xMax, int yMin, int yMax) {
            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    TraceRay(x, y);
                }
            }
        }

        void TraceRay(int x, int y) {
            // Debug: Check if ray has to be drawn
            bool debugRay = Debug && DebugShowRays && (x % 16 == 0 || x == Scene.Camera.ScreenPlane.Screen.Width) && y == Scene.Camera.ScreenPlane.Screen.Height / 2;

            // Cast Ray
            Ray ray = Scene.Camera.CreatePrimaryRay(x, y);
            Vector3 pixelColor = Scene.CastRay(ray, 0, debugRay);

            // Draw Pixel
            Scene.Camera.ScreenPlane.Screen.Plot(x, y, pixelColor.ToIntColor());
        }
    }
}