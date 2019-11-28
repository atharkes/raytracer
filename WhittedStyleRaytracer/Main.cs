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

        /// <summary> Constant that defines the maximum recursion for secondary rays </summary>
        public const int MaxRecursionDepth = 5;

        /// <summary> Whether it is drawing debug information </summary>
        public bool Debug = false;
        /// <summary> Whether it is drawing primary rays as debug information </summary>
        public bool DebugPrimaryRay = true;
        /// <summary> Whether it is drawing shadow rays as debug information </summary>
        public bool DebugShadowRay = true;
        /// <summary> The scale to draw the debug information </summary>
        public float DebugScale = 50f;

        OpenTKProgram openTKApp;  

        Threadpool threadpool;
        Action[] tasks;
        const int taskAmount = 512;

        KeyboardState keyboardState;
        MouseState mouseStatePrevious, mouseStateCurrent;

        float lightPosMove = (float)Math.PI / 2;

        public void SetScreen(IScreen screen) {
            Screen = screen;
        }

        public void Init(OpenTKProgram openTKApp) {
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
                DrawCamera(Scene.Camera);
                Scene.Lights.ForEach(light => DrawLight(light));
                DrawScreenPlane(Scene.Camera);
                Scene.Primitives.ForEach(primitive => { if (primitive is Sphere) DrawSphere(primitive as Sphere); });
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

        // Raytracer
        void TraceRay(int x, int y) {
            // Debug: Check if ray has to be drawn
            bool debugRay = Debug && (x == 0 || (x + 1) % 16 == 0) && y == (Screen.Height - 1) / 2;

            // Cast Ray
            Ray ray = Scene.Camera.CreatePrimaryRay(x, y);
            Vector3 pixelColor = CastPrimaryRay(ray, 0, debugRay);

            // Draw Pixel
            Screen.Plot(x, y, GetColor(pixelColor));
        }

        // Intersect primary ray with primitives and fire a new ray if object is specular
        Vector3 CastPrimaryRay(Ray ray, int recursionDepth = 0, bool debugRay = false) {
            // Intersect with Scene
            Intersection intersection = Scene.AccelerationStructure.Intersect(ray);
            if (intersection == null) return Vector3.Zero;

            Vector3 color = CastShadowRays(intersection, debugRay);

            // Debug: Primary Rays
            if (Debug && debugRay && DebugPrimaryRay) {
                DrawRay(ray, 0xffff00);
            }

            // Specularity
            if (intersection.Primitive.Specularity > 0 && recursionDepth < MaxRecursionDepth) {
                recursionDepth += 1;
                // Cast Reflected Ray
                Vector3 normal = intersection.Normal;
                Vector3 newDirection = ray.Direction - 2 * Vector3.Dot(ray.Direction, normal) * normal;
                ray = new Ray(intersection.Position, newDirection);
                Vector3 colorReflection = CastPrimaryRay(ray, recursionDepth);

                // Calculate Specularity
                color = color * (1 - intersection.Primitive.Specularity) + colorReflection * intersection.Primitive.Specularity * intersection.Primitive.Color;
            }

            return color;
        }

        // Intersect a shadow ray with the scene for each light and calculate the color
        Vector3 CastShadowRays(Intersection intersection, bool debugRay = false) {
            Vector3 totalColor = new Vector3(0, 0, 0);
            foreach (Lightsource light in Scene.Lights) {
                Vector3 color = intersection.Primitive.Color;

                Ray shadowRay = Ray.CreateShadowRay(intersection.Position, light.Position);

                if (Scene.AccelerationStructure.IntersectBool(shadowRay)) {
                    continue;
                } else {
                    // Light Absorption
                    color = color * light.Color;
                    // N dot L
                    Vector3 normal = intersection.Normal;
                    float NdotL = Vector3.Dot(normal, shadowRay.Direction);
                    if (intersection.Primitive.Glossyness == 0) {
                        color = color * NdotL;
                    } else if (intersection.Primitive.Glossyness > 0) {
                        // Glossyness
                        Vector3 glossyDirection = (-shadowRay.Direction - 2 * (Vector3.Dot(-shadowRay.Direction, normal)) * normal);
                        float dot = Vector3.Dot(glossyDirection, -intersection.Ray.Direction);
                        if (dot > 0) {
                            float glossyness = (float)Math.Pow(dot, intersection.Primitive.GlossSpecularity);
                            // Phong-Shading (My Version)
                            color = color * (1 - intersection.Primitive.Glossyness) * NdotL + intersection.Primitive.Glossyness * glossyness * light.Color;
                            // Phong-Shading (Official)
                            //color = color * ((1 - intersection.primitive.glossyness) * NdotL + intersection.primitive.glossyness * glossyness);
                        } else {
                            color = color * (1 - intersection.Primitive.Glossyness) * NdotL;
                        } 
                    }
                    // Distance Attenuation
                    color = color * (1 / (shadowRay.Length * shadowRay.Length));

                    // Add Color to Total
                    totalColor += color;

                    // Debug: Shadow Rays
                    if (Debug && debugRay && DebugShadowRay && !(intersection.Primitive is Plane)) {
                        DrawRay(shadowRay, GetColor(light.Color));
                    }
                }

            }
            // Triangle Texture
            if (intersection.Primitive is Triangle) {
                if (Math.Abs(intersection.Position.X % 2) < 1) totalColor = totalColor * 0.5f;
                if (Math.Abs(intersection.Position.Z % 2) > 1) totalColor = totalColor * 0.5f;
            }

            return totalColor;
        }

        /// <summary> Convert Color from Vector3 to integer </summary>
        /// <param name="color">The color in Vector3 format</param>
        /// <returns>The color in integer format</returns>
        int GetColor(Vector3 color) {
            color = Vector3.Clamp(color, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            int r = (int)(color.X * 255) << 16;
            int g = (int)(color.Y * 255) << 8;
            int b = (int)(color.Z * 255) << 0;
            return r + g + b;
        }

        #region Debug Drawing
        // Debug: Draw Ray
        void DrawRay(Ray ray, int color) {
            int x1 = TX(ray.Origin.X);
            int y1 = TZ(ray.Origin.Z);
            int x2 = TX(ray.Origin.X + ray.Direction.X * ray.Length);
            int y2 = TZ(ray.Origin.Z + ray.Direction.Z * ray.Length);
            Screen.Line(x1, y1, x2, y2, color);
        }

        // Debug: Draw Camera
        void DrawCamera(Camera camera) {
            int x1 = TX(camera.Position.X) - 1;
            int y1 = TZ(camera.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
             Screen.Box(x1, y1, x2, y2, 0xffffff);
        }

        // Debug: Draw Lightsource
        void DrawLight(Lightsource light) {
            int x1 = TX(light.Position.X) - 1;
            int y1 = TZ(light.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            Screen.Box(x1, y1, x2, y2, GetColor(light.Color));
        }

        // Debug: Draw Screen Plane
        void DrawScreenPlane(Camera camera) {
            Screen.Line(TX(camera.ScreenPlane.TopLeft.X), TZ(camera.ScreenPlane.TopLeft.Z), TX(camera.ScreenPlane.TopRight.X), TZ(camera.ScreenPlane.TopRight.Z), 0xffffff);
            Screen.Line(TX(camera.ScreenPlane.TopRight.X), TZ(camera.ScreenPlane.TopRight.Z), TX(camera.ScreenPlane.BottomRight.X), TZ(camera.ScreenPlane.BottomRight.Z), 0xffffff);
            Screen.Line(TX(camera.ScreenPlane.BottomRight.X), TZ(camera.ScreenPlane.BottomRight.Z), TX(camera.ScreenPlane.BottomLeft.X), TZ(camera.ScreenPlane.BottomLeft.Z), 0xffffff);
            Screen.Line(TX(camera.ScreenPlane.BottomLeft.X), TZ(camera.ScreenPlane.BottomLeft.Z), TX(camera.ScreenPlane.TopLeft.X), TZ(camera.ScreenPlane.TopLeft.Z), 0xffffff);
        }

        // Debug: Draw Sphere
        void DrawSphere(Sphere sphere) {
            for (int i = 0; i < 128; i++) {
                int x1 = TX(sphere.Position.X + (float)Math.Cos(i / 128f * 2 * Math.PI) * sphere.Radius);
                int y1 = TZ(sphere.Position.Z + (float)Math.Sin(i / 128f * 2 * Math.PI) * sphere.Radius);
                int x2 = TX(sphere.Position.X + (float)Math.Cos((i + 1) / 128f * 2 * Math.PI) * sphere.Radius);
                int y2 = TZ(sphere.Position.Z + (float)Math.Sin((i + 1) / 128f * 2 * Math.PI) * sphere.Radius);
                Screen.Line(x1, y1, x2, y2, GetColor(sphere.Color));
            }
        }

        // Debug: Transform X coord to X on debug screen
        public int TX(float x) {
            x -= Scene.Camera.Position.X - DebugScale * .5f;
            int xDraw = (int)(512 / DebugScale * x);
            return xDraw;
        }

        // Debug: Transform Z coord to Y on debug screen
        public int TZ(float z) {
            z -= Scene.Camera.Position.Z - DebugScale * .5f;
            int yDraw = (int)(512 / DebugScale * z);
            return yDraw;
        }
        #endregion
    }
}