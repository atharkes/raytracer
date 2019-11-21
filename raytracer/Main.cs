using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using Raytracer.Multithreading;
using Raytracer.Objects;

namespace Raytracer {
    class Main {
        public Surface Screen;
        public Camera Camera;
        public Scene Scene;
        public BVHNode PrimitiveTree;

        OpenTKApp openTKApp;
        readonly int raytracerWidth = 512;
        readonly int raytracerHeight = 512;
        public const int RecursionDepth = 5;

        Threadpool threadpool;
        Action[] tasks;
        readonly int taskAmount = 64;

        float debugCamX;
        float debugCamZ;
        readonly float debugCamWidth = 20f;
        readonly float debugCamHeight = 20f;
        bool debugRay;
        bool debug = true;
        readonly bool debugPrimaryRay = true;
        readonly bool debugShadowRay = true;

        KeyboardState keyboardState;
        MouseState mouseStatePrevious, mouseStateCurrent;

        float lightStartPos = (float)Math.PI / 2;

        public void Init(OpenTKApp openTKApp) {
            this.openTKApp = openTKApp;
            Camera = new Camera();
            Scene = new Scene();
            PrimitiveTree = new BVHNode(Scene.Primitives);
            tasks = new Action[taskAmount];
            threadpool = new Threadpool();
        }

        public void SetScreen() {
            int screenWidth = raytracerWidth;
            int screenHeight = raytracerHeight;
            if (debug)
                screenWidth += 512;
            Screen = new Surface(screenWidth, screenHeight);
        }

        public void Tick() {
            // Moving Light
            lightStartPos += 0.1f;
            float move = (float)Math.Sin(lightStartPos) * 0.5f;
            Scene.Lights[0].Position += new Vector3(move, 0, 0);

            // Clear the screen
            Screen.Clear(0);

            // Check Input
            InputCheck();

            // Trace Rays
            DivideRays();

            // Debug
            if (debug) {
                Screen.Line(512, 0, 512, 511, 0xffffff);
                debugCamX = Camera.Position.X - debugCamHeight / 2;
                debugCamZ = Camera.Position.Z - debugCamWidth / 2;
                DrawCamera(Camera);
                foreach (Lightsource light in Scene.Lights)
                    DrawLight(light);
                DrawScreenPlane(Camera);
                foreach (Primitive primitive in Scene.Primitives)
                    if (primitive is Sphere)
                        DrawSphere((Sphere)primitive);
            }
            Screen.Print("FPS: " + (int)openTKApp.RenderFrequency, 1, 1, 0xffffff);
            Screen.Print("FOV: " + Camera.FOV, 1, 16, 0xffffff);
        }

        void InputCheck() {
            // Input: Keyboard
            keyboardState = Keyboard.GetState();
            if (keyboardState[Key.F1]) debug = !debug;
            if (keyboardState[Key.Space]) Camera.Move(Camera.Up);
            if (keyboardState[Key.LShift]) Camera.Move(Camera.Down);
            if (keyboardState[Key.W]) Camera.Move(Camera.Front);
            if (keyboardState[Key.S]) Camera.Move(Camera.Back);
            if (keyboardState[Key.A]) Camera.Move(Camera.Left);
            if (keyboardState[Key.D]) Camera.Move(Camera.Right);
            if (keyboardState[Key.KeypadPlus]) Camera.FOV *= 1.1f;
            if (keyboardState[Key.KeypadMinus]) Camera.FOV *= 0.9f;
            // Input: Mouse
            mouseStateCurrent = Mouse.GetState();
            if (mouseStatePrevious != null) {
                float xDelta = mouseStateCurrent.X - mouseStatePrevious.X;
                float yDelta = mouseStateCurrent.Y - mouseStatePrevious.Y;
                Camera.Turn(xDelta * Camera.Right + yDelta * Camera.Down);
            }
            mouseStatePrevious = mouseStateCurrent;
        }

        // Multithreading
        void DivideRays() {
            Stopwatch divideTime = Stopwatch.StartNew();
            float size = (float)raytracerHeight / taskAmount;
            float[] taskBounds = new float[taskAmount + 1];
            taskBounds[0] = 0;
            for (int n = 0; n < taskAmount; n++) {
                taskBounds[n + 1] = taskBounds[n] + size;
                int taskLower = (int)taskBounds[n];
                int taskUpper = (int)taskBounds[n + 1];
                tasks[n] = () => TraceRays(0, raytracerWidth, taskLower, taskUpper);
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
            if (debug) {
                if ((x == 0 || (x + 1) % 16 == 0) && y == (raytracerHeight - 1) / 2) {
                    debugRay = true;
                } else {
                    debugRay = false;
                }
            }

            // Cast Ray
            Ray ray = CreatePrimaryRay(x, y);
            Vector3 pixelColor = CastPrimaryRay(ray);

            // Draw Pixel
            Screen.Plot(x, y, GetColor(pixelColor));
        }

        // Create primary ray from camera origin trough screen plane
        Ray CreatePrimaryRay(int x, int y) {
            Vector3 planePoint = Camera.ScreenPlane.TopLeft + ((float)x / (raytracerWidth - 1)) * (Camera.ScreenPlane.TopRight - Camera.ScreenPlane.TopLeft) + (float)y / (raytracerHeight - 1) * (Camera.ScreenPlane.BottomLeft - Camera.ScreenPlane.TopLeft);
            return new Ray(Camera.Position, planePoint - Camera.Position);
        }

        // Intersect primary ray with primitives and fire a new ray if object is specular
        Vector3 CastPrimaryRay(Ray ray, int recursionDepth = 0) {
            // Intersect with Scene
            Intersection intersection = PrimitiveTree.IntersectTree(ray);
            Vector3 color = intersection != null ? CastShadowRays(intersection) : Vector3.Zero;

            // Debug: Primary Rays
            if (debug && debugRay && debugPrimaryRay && intersection != null) {
                DrawRay(ray, intersection.Distance, 0xffff00);
            }

            // Specularity
            if (intersection != null && intersection.Primitive.Specularity > 0 && recursionDepth < RecursionDepth) {
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
        Vector3 CastShadowRays(Intersection intersection) {
            if (intersection == null) return Vector3.Zero;

            Vector3 totalColor = new Vector3(0, 0, 0);
            foreach (Lightsource light in Scene.Lights) {
                Vector3 color = intersection.Primitive.Color;

                Ray shadowRay = Ray.CreateShadowRay(intersection.Position, light.Position);

                if (PrimitiveTree.IntersectTreeBool(shadowRay)) {
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
                    if (debug && debugRay && debugShadowRay && !(intersection.Primitive is Plane)) {
                        DrawRay(shadowRay, shadowRay.Length, GetColor(light.Color));
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

        // Convert Color from Vector3 to int
        int GetColor(Vector3 color) {
            color = Vector3.Clamp(color, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            int r = (int)(color.X * 255) << 16;
            int g = (int)(color.Y * 255) << 8;
            int b = (int)(color.Z * 255) << 0;
            return r + g + b;
        }

        // Debug: Draw Ray
        void DrawRay(Ray ray, float length, int color) {
            int x1 = TX(ray.Origin.X);
            int y1 = TZ(ray.Origin.Z);
            int x2 = TX(ray.Origin.X + ray.Direction.X * length);
            int y2 = TZ(ray.Origin.Z + ray.Direction.Z * length);
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2)) {
                Screen.Line(x1, y1, x2, y2, color);
            }
        }

        // Debug: Draw Camera
        void DrawCamera(Camera camera) {
            int x1 = TX(camera.Position.X) - 1;
            int y1 = TZ(camera.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2)) {
                Screen.Box(x1, y1, x2, y2, 0xffffff);
            }
        }

        // Debug: Draw Lightsource
        void DrawLight(Lightsource light) {
            int x1 = TX(light.Position.X) - 1;
            int y1 = TZ(light.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2)) {
                Screen.Box(x1, y1, x2, y2, GetColor(light.Color));
            }
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
                if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2)) {
                    Screen.Line(x1, y1, x2, y2, GetColor(sphere.Color));
                }
            }
        }

        // Debug: Check if outside debug screen bounds
        bool CheckDebugBounds(int x, int y) {
            if (x < raytracerWidth || x > raytracerWidth + 511 || y < 0 || y > 511) {
                return false;
            } else {
                return true;
            }
        }

        // Debug: Transform X coord to X on debug screen
        public int TX(float x) {
            x -= debugCamX;
            int xDraw = (int)((512 / debugCamWidth) * x);
            return xDraw + raytracerWidth;
        }

        // Debug: Transform Z coord to Y on debug screen
        public int TZ(float z) {
            z -= debugCamZ;
            int yDraw = (int)((512 / debugCamHeight) * z);
            return yDraw;
        }
    }
}