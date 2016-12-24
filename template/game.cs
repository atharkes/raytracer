using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using template.multithreading;

namespace template {
    class Game
    {
	    public Surface screen;
        public camera camera;
        public scene scene;
        public BVHNode primitiveTree;
        OpenTKApp template;
        int raytracerWidth = 512;
        int raytracerHeight = 512;
        int recursionDepthMax = 5;

		threadpool threadpool;
		Action[] tasks;
		int taskAmount = 256;

        float debugCamX;
        float debugCamZ;
        float debugCamWidth = 20f;
        float debugCamHeight = 20f;
        bool debugRay;
        bool debug = false;
        bool debugPrimaryRay = true;
        bool debugShadowRay = true;

        KeyboardState keyboardState;
        MouseState mouseStatePrevious, mouseStateCurrent;

        float lightStartPos = (float)Math.PI / 2;

        public void Init(OpenTKApp template)
	    {
            this.template = template;
			camera = new camera(new Vector3(0, 0, -1), new Vector3(0, 0, 1));
            scene = new scene();
            primitiveTree = new BVHNode(scene.primitives);
			tasks = new Action[taskAmount];
			threadpool = new threadpool();
        }

		public void SetScreen()
		{
			int screenWidth = raytracerWidth;
			int screenHeight = raytracerHeight;
			if (debug)
				screenWidth += 512;
			screen = new Surface(screenWidth, screenHeight);
		}

	    public void Tick()
	    {
			// Moving Light
			lightStartPos += 0.1f;
            float move = (float)Math.Sin(lightStartPos) * 0.5f;
            scene.lights[0].position += new Vector3(move, 0, 0);

            // Clear the screen
            screen.Clear(0);

            // Check Input
            InputCheck();

			// Trace Rays
			DivideRays();

			// Debug
			if (debug)
            {
				screen.Line(512, 0, 512, 511, 0xffffff);
				debugCamX = camera.position.X - debugCamHeight / 2;
                debugCamZ = camera.position.Z - debugCamWidth / 2;
                DrawCamera(camera);
                foreach (lightsource light in scene.lights)
                    DrawLight(light);
                DrawScreenPlane(camera);
                foreach (primitive primitive in scene.primitives)
                    if (primitive is sphere)
                        DrawSphere((sphere)primitive);
            }
            screen.Print("FPS: " + (int)template.RenderFrequency, 1, 1, 0xffffff);
            screen.Print("FOV: " + camera.fov, 1, 16, 0xffffff);
		}
        
        void InputCheck()
        {
            // Input: Keyboard
            keyboardState = Keyboard.GetState();
            if (keyboardState[Key.F1])
                debug = !debug;
            if (keyboardState[Key.Space])
                camera.Move(camera.up);
            if (keyboardState[Key.LShift])
                camera.Move(-camera.up);
            if (keyboardState[Key.W])
                camera.Move(camera.direction);
            if (keyboardState[Key.S])
                camera.Move(-camera.direction);
            if (keyboardState[Key.A])
                camera.Move(camera.left);
            if (keyboardState[Key.D])
                camera.Move(-camera.left);
            if (keyboardState[Key.KeypadPlus])
                camera.fov += 1f;
            if (keyboardState[Key.KeypadMinus])
                camera.fov -= 1f;
            // Input: Mouse
            mouseStateCurrent = Mouse.GetState();
            if (mouseStatePrevious != null)
            {
                float xDelta = mouseStateCurrent.X - mouseStatePrevious.X;
                float yDelta = mouseStateCurrent.Y - mouseStatePrevious.Y;
                camera.Turn(-xDelta * camera.left + -yDelta * camera.up);
            }
            mouseStatePrevious = mouseStateCurrent;
        }

        // Multithreading
		void DivideRays() {
			float size = (float)raytracerHeight / taskAmount;
			float[] taskBounds = new float[taskAmount + 1];
			taskBounds[0] = 0;
			for (int n = 0; n < taskAmount; n++) {
				taskBounds[n + 1] = taskBounds[n] + size;
				int taskLower = (int)taskBounds[n];
				int taskUpper = (int)taskBounds[n + 1];
				tasks[n] = () => TraceRays(0, raytracerWidth, taskLower, taskUpper);
			}
			Stopwatch traceTime = Stopwatch.StartNew();
			threadpool.doTasks(tasks);
			while (!threadpool.workDone()) { /* Wait */ };
			Console.WriteLine(traceTime.ElapsedMilliseconds);
		}

		void TraceRays(int xMin, int xMax, int yMin, int yMax) {
			for (int y = yMin; y < yMax; y++) {
				for (int x = xMin; x < xMax; x++) {
					TraceRay(x, y);
				}
			}
		}

		// Raytracer
		void TraceRay(int x, int y)
        {
            // Debug: Check if ray has to be drawn
            if (debug)
            {
                if ((x == 0 || (x + 1) % 16 == 0) && y == (raytracerHeight - 1) / 2)
                    debugRay = true;
                else
                    debugRay = false;
            }

            // Cast Ray
            ray ray = GetPrimaryRay(x, y);
            Vector3 pixelColor = CastPrimaryRay(ray);

			// Draw Pixel
			screen.Plot(x, y, GetColor(pixelColor));
        }

        // Create primary ray from camera origin trough screen plane
        ray GetPrimaryRay(int x, int y)
        {
            Vector3 planePoint = camera.planeCorner1 + ((float)x / (raytracerWidth - 1)) * (camera.planeCorner2 - camera.planeCorner1) + ((float)y / (raytracerHeight - 1)) * (camera.planeCorner3 - camera.planeCorner1);
            return new ray(camera.position, planePoint - camera.position);
        }

        // Intersect primary ray with primitives and fire a new ray if object is specular
        Vector3 CastPrimaryRay(ray ray, int recursionDepth = 0)
        {
            // Intersect with Scene
            Tuple<float, primitive> rayIntersection = primitiveTree.IntersectTree(ray);
            float intersectDistance = rayIntersection.Item1;
            primitive intersectPrimitive = rayIntersection.Item2;
            ray.length = intersectDistance;
            intersection intersection = new intersection(ray.origin + ray.direction * ray.length, intersectPrimitive);
            Vector3 color = CastShadowRays(intersection, ray);

            // Debug: Primary Rays
            if (debug && debugRay && debugPrimaryRay)
            {
                if (ray.length < 0 || intersection.primitive is plane)
                    DrawRay(ray, 1.5f, 0xffff00);
                else
                    DrawRay(ray, ray.length, 0xffff00);
            }
            // Specularity
            if (intersection.primitive != null)
            {
                if (intersection.primitive.specularity > 0 && recursionDepth < recursionDepthMax)
                {
                    recursionDepth += 1;
                    // Cast Reflected Ray
                    Vector3 normal = intersection.primitive.GetNormal(intersection.position);
                    Vector3 newDirection = ray.direction - 2 * Vector3.Dot(ray.direction, normal) * normal;
                    ray = new ray(intersection.position, newDirection);
                    Vector3 colorReflection = CastPrimaryRay(ray, recursionDepth);

                    // Calculate Specularity
                    color = color * (1 - intersection.primitive.specularity) + colorReflection * intersection.primitive.specularity * intersection.primitive.color;
                }
            }
            return color;
        }

        // Intersect shadow ray with primitives for each light and calculating the color
        Vector3 CastShadowRays(intersection intersection, ray primaryRay)
        {
            Vector3 totalColor = new Vector3(0, 0, 0);
            if (intersection.primitive == null)
                return totalColor;

            foreach (lightsource light in scene.lights)
            {
                Vector3 color = intersection.primitive.color;

                ray shadowRay = new ray(intersection.position, light.position - intersection.position);
                shadowRay.length = (float)Math.Sqrt(Vector3.Dot(light.position - shadowRay.origin, light.position - shadowRay.origin));

                if (primitiveTree.IntersectTreeBool(shadowRay))
                    continue;
                else
                {
                    // Light Absorption
                    color = color * light.color;
                    // N dot L
                    Vector3 normal = intersection.primitive.GetNormal(intersection.position);
                    float NdotL = Vector3.Dot(normal, shadowRay.direction);
                    if (intersection.primitive.glossyness == 0)
                        color = color * NdotL;
                    // Glossyness
                    else if (intersection.primitive.glossyness > 0)
                    {
                        Vector3 glossyDirection = (-shadowRay.direction - 2 * (Vector3.Dot(-shadowRay.direction, normal)) * normal);
                        float dot = Vector3.Dot(glossyDirection, -primaryRay.direction);
                        if (dot > 0)
                        {
                            float glossyness = (float)Math.Pow(dot, intersection.primitive.glossSpecularity);
                            // Phong-Shading (My Version)
                            color = color * (1 - intersection.primitive.glossyness) * NdotL + intersection.primitive.glossyness * glossyness * light.color;
                            // Phong-Shading (Official)
                            //color = color * ((1 - intersection.primitive.glossyness) * NdotL + intersection.primitive.glossyness * glossyness);
                        }
                        else
                            color = color * (1 - intersection.primitive.glossyness) * NdotL;
                    }
                    // Distance Attenuation
                    color = color * (1 / (shadowRay.length * shadowRay.length));

                    // Add Color to Total
                    totalColor += color;

                    // Debug: Shadow Rays
                    if (debug && debugRay && debugShadowRay && !(intersection.primitive is plane))
                        DrawRay(shadowRay, shadowRay.length, GetColor(light.color));
                }
                
            }
            // Triangle Texture
            if (intersection.primitive is triangle)
            {
                if (Math.Abs(intersection.position.X % 2) < 1)
                    totalColor = totalColor * 0.5f;
                if (Math.Abs(intersection.position.Z % 2) > 1)
                    totalColor = totalColor * 0.5f;
            }

            return totalColor;
        }

        // Convert Color from Vector3 to int
        int GetColor(Vector3 color)
        {
            color = Vector3.Clamp(color, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            int r = (int)(color.X * 255) << 16;
            int g = (int)(color.Y * 255) << 8;
            int b = (int)(color.Z * 255) << 0;
            return r + g + b;
        }

        // Debug: Draw Ray
        void DrawRay(ray ray, float length, int color)
        {
            int x1 = TX(ray.origin.X);
            int y1 = TZ(ray.origin.Z);
            int x2 = TX(ray.origin.X + ray.direction.X * length);
            int y2 = TZ(ray.origin.Z + ray.direction.Z * length);
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2))
                screen.Line(x1, y1, x2, y2, color);
        }

        // Debug: Draw Camera
        void DrawCamera(camera camera)
        {
            int x1 = TX(camera.position.X) - 1;
            int y1 = TZ(camera.position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2))
                screen.Box(x1, y1, x2,  y2, 0xffffff);
        }

        // Debug: Draw Lightsource
        void DrawLight(lightsource light)
        {
            int x1 = TX(light.position.X) - 1;
            int y1 = TZ(light.position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2))
                screen.Box(x1, y1, x2, y2, GetColor(light.color));
        }

        // Debug: Draw Screen Plane
        void DrawScreenPlane(camera camera)
        {
            screen.Line(TX(camera.planeCorner1.X), TZ(camera.planeCorner1.Z), TX(camera.planeCorner2.X), TZ(camera.planeCorner2.Z), 0xffffff);
            screen.Line(TX(camera.planeCorner2.X), TZ(camera.planeCorner2.Z), TX(camera.planeCorner4.X), TZ(camera.planeCorner4.Z), 0xffffff);
            screen.Line(TX(camera.planeCorner3.X), TZ(camera.planeCorner3.Z), TX(camera.planeCorner4.X), TZ(camera.planeCorner4.Z), 0xffffff);
            screen.Line(TX(camera.planeCorner3.X), TZ(camera.planeCorner3.Z), TX(camera.planeCorner1.X), TZ(camera.planeCorner1.Z), 0xffffff);
        }

        // Debug: Draw Sphere
        void DrawSphere(sphere sphere)
        {
            for (int i = 0; i < 128; i++)
            {
                int x1 = TX(sphere.position.X + (float)Math.Cos(i / 128f * 2 * Math.PI) * sphere.radius);
                int y1 = TZ(sphere.position.Z + (float)Math.Sin(i / 128f * 2 * Math.PI) * sphere.radius);
                int x2 = TX(sphere.position.X + (float)Math.Cos((i + 1) / 128f * 2 * Math.PI) * sphere.radius);
                int y2 = TZ(sphere.position.Z + (float)Math.Sin((i + 1) / 128f * 2 * Math.PI) * sphere.radius);
                if (CheckDebugBounds(x1, y1) && CheckDebugBounds(x2, y2))
                    screen.Line(x1, y1, x2, y2, GetColor(sphere.color));
            }
        }

        // Debug: Check if outside debug screen bounds
        bool CheckDebugBounds(int x, int y)
        {
            if (x < raytracerWidth || x > raytracerWidth + 511 || y < 0 || y > 511)
                return false;
            else
                return true;
        }
        
        // Debug: Transform X coord to X on debug screen
        public int TX(float x)
        {
            x -= debugCamX;
            int xDraw = (int)((512 / debugCamWidth) * x);
            return xDraw + raytracerWidth;
        }

        // Debug: Transform Z coord to Y on debug screen
        public int TZ(float z)
        {
            z -= debugCamZ;
            int yDraw = (int)((512 / debugCamHeight) * z);
            return yDraw;
        }
    }
}