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
        [JsonIgnore] public Vector2i WindowSize { get; set; } = new Vector2i(1280, 720);
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
        public DrawingMode DrawingMode { get; set; } = DrawingMode.Light;
        /// <summary> Whether the debug information is being displayed to the observer </summary>
        public bool DebugInfo { get; set; } = false;

        /// <summary> The position of the camera </summary>
        [JsonIgnore] public Position3 Position { get; set; } = new Position3(0, 1, 0);
        /// <summary> The x position of the camera </summary>
        public float PositionX { get => Position.X.Vector.Value; set => Position = new Position3(value, Position.Y, Position.Z); }
        /// <summary> The y position of the camera </summary>
        public float PositionY { get => Position.Y.Vector.Value; set => Position = new Position3(Position.X, value, Position.Z); }
        /// <summary> The z position of the camera </summary>
        public float PositionZ { get => Position.Z.Vector.Value; set => Position = new Position3(Position.X, Position.Y, value); }

        /// <summary> The rotation quaternion of the camera </summary>
        [JsonIgnore] public Quaternion Rotation { get; set; } = Quaternion.Identity;
        /// <summary> The x component of the rotation quaternion </summary>
        public float RotationX { get => Rotation.X; set => Rotation = new Quaternion(value, Rotation.Y, Rotation.Z, Rotation.W); }
        /// <summary> The y component of the rotation quaternion </summary>
        public float RotationY { get => Rotation.Y; set => Rotation = new Quaternion(Rotation.X, value, Rotation.Z, Rotation.W); }
        /// <summary> The z component of the rotation quaternion </summary>
        public float RotationZ { get => Rotation.Z; set => Rotation = new Quaternion(Rotation.X, Rotation.Y, value, Rotation.W); }
        /// <summary> The w component of the rotation quaternion </summary>
        public float RotationW { get => Rotation.W; set => Rotation = new Quaternion(Rotation.X, Rotation.Y, Rotation.Z, value); }

        /// <summary> The field of view of the camera </summary>
        public float FOV { get; set; } = 90f;

        /// <summary> The name of the file in which the config is saved </summary>
        public const string FileName = "CameraConfig.json";
        /// <summary> The path of the file in which the config is saved </summary>
        public static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
        /// <summary> The filestream to the config file </summary>
        public static readonly FileStream FileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        /// <summary> The settings of the json serializer </summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented };
        /// <summary> The json serializer </summary>
        public static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(Settings);

        /// <summary> Save the camera configuration to the file </summary>
        public void SaveToFile(IRenderer renderer) {
            /// Obtain Configuration
            WindowSize = renderer.Observer.Screen.Size;
            // Window Position is currently not accessible
            DrawingMode = renderer.Observer.DrawingMode;
            DebugInfo = renderer.Observer.DebugInfo;
            Position = renderer.Observer.Camera.Position;
            Rotation = renderer.Observer.Camera.Rotation;
            FOV = renderer.Observer.Camera.FOV;
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
