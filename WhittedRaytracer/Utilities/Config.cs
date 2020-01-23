using Newtonsoft.Json;
using System;
using System.IO;

namespace WhittedRaytracer.Utilities {
    /// <summary> A file that holds the configuration of the camera </summary>
    class Config {
        /// <summary> Whether it is drawing debug information </summary>
        public bool DebugInfo { get; set; } = false;
        /// <summary> Whether to draw the amount of BVH traversals instead of normal light </summary>
        public bool DrawBVHTraversal { get; set; } = false;
        /// <summary> Minimum amount of rays to trace in a tick </summary>
        public int MinimumRayCount { get; set; } = 10_000;
        /// <summary> The targeted framerate of the raytracer </summary>
        public int TargetFrameRate { get; set; } = 20;

        /// <summary> The targeted frame time computed from the targeted frame rate </summary>
        public TimeSpan TargetFrameTime => new TimeSpan(0, 0, 0, 0, 1000 / TargetFrameRate);


        public const string FileName = "CameraConfig.json";
        public static string FilePath => Path.Combine(Directory.GetCurrentDirectory(), FileName);
        public static FileStream FileStream => File.Open(FilePath, FileMode.OpenOrCreate);

        public void SaveToFile() {
            StreamWriter streamWriter = new StreamWriter(FileStream);
            JsonSerializer.Create().Serialize(streamWriter, this);
            streamWriter.Flush();
        }

        public static Config LoadFromFile() {
            if (!File.Exists(FilePath)) return new Config();
            JsonTextReader reader = new JsonTextReader(new StreamReader(FileStream));
            return JsonSerializer.Create().Deserialize<Config>(reader) ?? new Config();
        }
    }
}
