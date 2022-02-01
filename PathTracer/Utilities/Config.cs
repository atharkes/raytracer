using Newtonsoft.Json;
using OpenTK.Mathematics;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing;
using PathTracer.Pathtracing.Observers;
using System.IO;

namespace PathTracer.Utilities {
    /// <summary> A file that holds the configuration of the camera </summary>
    public class Config {
        /// <summary> The size of the game window </summary>
        [JsonIgnore] public Vector2i WindowSize { get; set; } = new Vector2i(500, 400);
        /// <summary> The width of the game window </summary>
        public int WindowWidth { get => WindowSize.X; set => WindowSize = new Vector2i(value, WindowSize.Y); }
        /// <summary> The height of the game window </summary>
        public int WindowHeight { get => WindowSize.Y; set => WindowSize = new Vector2i(WindowSize.X, value); }
        /// <summary> The aspect ratio of the game window </summary>
        [JsonIgnore] public float AspectRatio => (float)WindowWidth / WindowHeight;

        /// <summary> The position of the game window </summary>
        [JsonIgnore] public Vector2i WindowPosition { get; set; } = new Vector2i(40, 80);
        /// <summary> The x position of the game window </summary>
        public int WindowPositionX { get => WindowPosition.X; set => WindowPosition = new Vector2i(value, WindowPosition.Y); }
        /// <summary> The y position of the game window </summary>
        public int WindowPositionY { get => WindowPosition.Y; set => WindowPosition = new Vector2i(WindowPosition.X, value); }

        /// <summary> The drawing mode of the observer </summary>
        public DrawingMode Drawing { get; set; } = DrawingMode.Light;
        /// <summary> Which debug information is being displayed to the observer </summary>
        public DebugOutput Debug { get; set; } = DebugOutput.None;
        /// <summary> The color of the debug output </summary>
        public int TextColor { get; set; } = 0xffffff;
        /// <summary> Whether the camera is locked </summary>
        public bool CameraLock { get; set; } = false;

        /// <summary> The position of the camera </summary>
        [JsonIgnore] public Position3 Position { get; set; } = new Position3(0, 1, 0);
        /// <summary> The X-position of the camera </summary>
        public float PositionX { get => Position.X.Vector.Value; set => Position = new Position3(value, Position.Y, Position.Z); }
        /// <summary> The Y-position of the camera </summary>
        public float PositionY { get => Position.Y.Vector.Value; set => Position = new Position3(Position.X, value, Position.Z); }
        /// <summary> The Z-position of the camera </summary>
        public float PositionZ { get => Position.Z.Vector.Value; set => Position = new Position3(Position.X, Position.Y, value); }

        /// <summary> The X-position of the camera in Blender </summary>
        public float BlenderPositionX => PositionX;
        /// <summary> The Y-position of the camera in Blender </summary>
        public float BlenderPositionY => PositionZ;
        /// <summary> The Z-position of the camera in Blender </summary>
        public float BlenderPositionZ => PositionY;

        /// <summary> The rotation quaternion of the camera </summary>
        [JsonIgnore] public Quaternion Rotation { get; set; } = Quaternion.Identity;
        /// <summary> The W-component of the rotation quaternion </summary>
        public float RotationW { get => Rotation.W; set => Rotation = new Quaternion(Rotation.X, Rotation.Y, Rotation.Z, value); }
        /// <summary> The X-component of the rotation quaternion </summary>
        public float RotationX { get => Rotation.X; set => Rotation = new Quaternion(value, Rotation.Y, Rotation.Z, Rotation.W); }
        /// <summary> The Y-component of the rotation quaternion </summary>
        public float RotationY { get => Rotation.Y; set => Rotation = new Quaternion(Rotation.X, value, Rotation.Z, Rotation.W); }
        /// <summary> The Z-component of the rotation quaternion </summary>
        public float RotationZ { get => Rotation.Z; set => Rotation = new Quaternion(Rotation.X, Rotation.Y, value, Rotation.W); }

        /// <summary> The W-component of the rotation quaternion in Blender </summary>
        public float BlenderRotationW => 0.7071f * RotationW - 0.7071f * -RotationX;
        /// <summary> The X-component of the rotation quaternion in Blender </summary>
        public float BlenderRotationX => 0.7071f * -RotationX + 0.7071f * RotationW;
        /// <summary> The Y-component of the rotation quaternion in Blender </summary>
        public float BlenderRotationY => 0.7071f * -RotationZ + 0.7071f * -RotationY;
        /// <summary> The Z-component of the rotation quaternion in Blender </summary>
        public float BlenderRotationZ => 0.7071f * -RotationY - 0.7071f * -RotationZ;

        /// <summary> The horizontal field of view of the camera </summary>
        public float HorizontalFOV { get; set; } = 90f;

        /// <summary> The name of the file in which the config is saved </summary>
        public const string FileName = "CameraConfig.json";
        /// <summary> The path of the file in which the config is saved </summary>
        public static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
        /// <summary> The filestream to the config file </summary>
        public static readonly FileStream FileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        /// <summary> The settings of the json serializer </summary>
        public static readonly JsonSerializerSettings Settings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented };
        /// <summary> The json serializer </summary>
        public static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(Settings);

        /// <summary> Save the camera configuration to the file </summary>
        public void SaveToFile(IRenderer renderer) {
            /// Obtain Configuration
            if (renderer.Observer is IInteractiveObserver observer) {
                WindowSize = observer.Screen.Size;
                WindowPosition = observer.Screen.Location;
                Drawing = observer.Drawing;
                Debug = observer.Debug;
                TextColor = observer.TextColor;
                CameraLock = observer.CameraLock;
            }
            Position = renderer.Observer.Camera.Position;
            Rotation = renderer.Observer.Camera.Rotation;
            HorizontalFOV = renderer.Observer.Camera.HorizontalFOV;
            /// Write to File
            FileStream.SetLength(0);
            StreamWriter streamWriter = new(FileStream);
            JsonSerializer.Serialize(streamWriter, this);
            streamWriter.Flush();
        }

        /// <summary> Load the camera configuration from the file </summary>
        /// <returns>The camera configuration from the file</returns>
        public static Config LoadFromFile() {
            if (!File.Exists(FilePath)) return new Config();
            JsonTextReader reader = new(new StreamReader(FileStream));
            return JsonSerializer.Deserialize<Config>(reader) ?? new Config();
        }
    }
}
