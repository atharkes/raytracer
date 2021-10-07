using Newtonsoft.Json;
using OpenTK.Mathematics;
using System.IO;

namespace PathTracer.Utilities {
    /// <summary> A file that holds the configuration of the camera </summary>
    public class Config {
        /// <summary> The position of the camera. This value should be modified in the camera </summary>
        [JsonIgnore] public Vector3 Position { get; set; } = new Vector3(0, 1, 0);
        /// <summary> The x position of the camera. This value should be modified in the camera </summary>
        public float PositionX { get => Position.X; set => Position = new Vector3(value, Position.Y, Position.Z); }
        /// <summary> The y position of the camera. This value should be modified in the camera </summary>
        public float PositionY { get => Position.Y; set => Position = new Vector3(Position.X, value, Position.Z); }
        /// <summary> The z position of the camera. This value should be modified in the camera </summary>
        public float PositionZ { get => Position.Z; set => Position = new Vector3(Position.X, Position.Y, value); }

        /// <summary> The direction the camera is facing. This value should be modified in the camera </summary>
        [JsonIgnore] public Vector3 ViewDirection { get; set; } = new Vector3(0, 0, 1);
        /// <summary> The x direction the camera is facing. This value should be modified in the camera </summary>
        public float ViewDirectionX { get => ViewDirection.X; set => ViewDirection = new Vector3(value, ViewDirection.Y, ViewDirection.Z); }
        /// <summary> The y direction the camera is facing. This value should be modified in the camera </summary>
        public float ViewDirectionY { get => ViewDirection.Y; set => ViewDirection = new Vector3(ViewDirection.X, value, ViewDirection.Z); }
        /// <summary> The z direction the camera is facing. This value should be modified in the camera </summary>
        public float ViewDirectionZ { get => ViewDirection.Z; set => ViewDirection = new Vector3(ViewDirection.X, ViewDirection.Y, value); }

        /// <summary> The field of view of the camera. This value should be modified in the camera </summary>
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
        public void SaveToFile() {
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
