using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Linq;
using WhittedStyleRaytracer.Multithreading;
using WhittedStyleRaytracer.Raytracing;
using WhittedStyleRaytracer.Raytracing.SceneObjects;
using WhittedStyleRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedStyleRaytracer {
    /// <summary> Main class of the raytracer </summary>
    class Main {
        /// <summary> The screen the raytracer is drawing to </summary>
        public IScreen Screen;
        /// <summary> The 3d scene in which the raytracing takes place </summary>
        public Scene Scene;

        /// <summary> Whether it is drawing debug information </summary>
        public bool Debug = false;
        /// <summary> Whether to draw rays in debug </summary>
        public bool DebugShowRays = true;
        

        readonly OpenTKProgram openTKApp;  

        readonly Threadpool threadpool;
        readonly Action[] tasks;
        const int taskAmount = 512;

        KeyboardState keyboardState;
        MouseState mouseStatePrevious, mouseStateCurrent;

        float lightPosMove = (float)Math.PI / 2;

        public Main(IScreen screen, OpenTKProgram openTKApp) {
            Screen = screen;
            this.openTKApp = openTKApp;
            Scene = new Scene(Screen);
            tasks = new Action[taskAmount];
            threadpool = new Threadpool();
        }

        public void Tick() {
            // Moving Light
            lightPosMove += 0.1f;
            float move = (float)Math.Sin(lightPosMove) * 0.5f;
            Scene.Lights.First().Position += new Vector3(move, 0, 0);

            // Clear the screen
            Screen.Clear(0);

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
            Screen.Print("FPS: " + (int)openTKApp.RenderFrequency, 1, 1, 0xffffff);
            Screen.Print("FOV: " + Scene.Camera.FOV, 1, 16, 0xffffff);
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
            Stopwatch divideTime = Stopwatch.StartNew();
            float size = (float)Screen.Height / taskAmount;
            float[] taskBounds = new float[taskAmount + 1];
            taskBounds[0] = 0;
            for (int n = 0; n < taskAmount; n++) {
                taskBounds[n + 1] = taskBounds[n] + size;
                int taskLower = (int)taskBounds[n];
                int taskUpper = (int)taskBounds[n + 1];
                tasks[n] = () => TraceRays(0, Screen.Width, taskLower, taskUpper);
            }
            Console.WriteLine("Divide Tasks Ticks: " + divideTime.ElapsedTicks);
            Stopwatch traceTime = Stopwatch.StartNew();
            threadpool.DoTasks(tasks);
            threadpool.WaitTillDone();
            Console.WriteLine("Trace Ticks: " + traceTime.ElapsedTicks);
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
            bool debugRay = Debug && DebugShowRays && (x % 16 == 0 || x == Screen.Width) && y == Screen.Height / 2;

            // Cast Ray
            Ray ray = Scene.Camera.CreatePrimaryRay(x, y);
            Vector3 pixelColor = Scene.CastPrimaryRay(ray, 0, debugRay);

            // Draw Pixel
            Screen.Plot(x, y, pixelColor.ToIntColor());
        }
    }
}