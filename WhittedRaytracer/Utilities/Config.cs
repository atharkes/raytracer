using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WhittedRaytracer.Utilities {
    class Config {
        static StreamReader streamReader;
        static StreamWriter streamWriter;

        static readonly Dictionary<string, string> settings = new Dictionary<string, string>();
        static readonly string path = $@"{Environment.CurrentDirectory}\config.ini";

        /// <summary> Initialize the config manager by giving it a configstream </summary>
        /// <param name="cs">A configstream for read/write actions </param>
        public static void Initialize() { }

        /// <summary> Undo any previously done initialisation, useful for testing </summary>
        public static void UnInitialize() {
            settings.Clear();
            streamReader = null;
        }

        /// <summary> The LoadContent of the config. Loads all the settings </summary>
        public static void LoadSettings() {
            CheckForFileExist();
            LoadSettingsFromFile();
        }

        /// <summary> Reads the config file and loads in all the settings </summary>
        static void LoadSettingsFromFile() {
            streamReader = new StreamReader(path);
            int lineNumber = 1;
            string line = streamReader.ReadLine();
            while (!string.IsNullOrEmpty(line)) {
                if (!line.Contains("=")) {
                    Debug.WriteLine($"On line {lineNumber} of the config.ini there is no '=' present");
                    continue;
                }
                string[] split = line.Split('=');
                if (split[0].Equals("") || split[1].Equals("")) {
                    Debug.WriteLine($"On line {lineNumber} of the config.ini there is an empty value");
                    continue;
                }
                settings[split[0].Trim()] = split[1].Trim();
                lineNumber++;
                line = streamReader.ReadLine();
            }
            streamReader.Close();
            streamReader = null;
        }

        /// <summary> Checks if the config.ini exists and otherwise makes a default version </summary>
        static void CheckForFileExist() {
            throw new NotImplementedException();
        }

        /// <summary> Add a new setting to the config </summary>
        /// <param name="key">The name of the setting (if already present it will be discarded)</param>
        /// <param name="value">The value of the setting</param>
        public static void Add(string key, string value) {
            if (!settings.ContainsKey(key)) {
                settings.Add(key.Trim(), value.Trim());
            }
        }

        /// <summary> Removing a setting from the config </summary>
        /// <param name="key">The name of the settings to be removed</param>
        public static void Remove(string key) {
            settings.Remove(key);
        }

        /// <summary> Helper function for the retrieval of a config value </summary>
        /// <param name="key">The name of the setting</param>
        /// <returns>The value saved in the config under that key</returns>
        public static string Get(string key) {
            try {
                return settings[key];
            } catch {
                throw new Exception($"The key '{key}' does not exists in the config.ini file. Typo?");
            }
        }

        /// <summary> Helper function for saving settings to the config file </summary>
        /// <param name="key">The name of the setting</param>
        /// <param name="value">The value you want to save under that setting</param>
        public static void Save(string key, string value) {
            try {
                settings[key] = value;
            } catch {
                throw new Exception($"The key '{key}' does not exists in the config.ini file. Typo?");
            }
        }

        /// <summary> Writes the entire settings dictionary to the config.ini file </summary>
        public static void WriteSettingsToFile() {
            streamWriter = new StreamWriter(path);
            foreach (KeyValuePair<string, string> kvp in settings) {
                streamWriter.WriteLine($"{kvp.Key}={kvp.Value}");
            }
            streamWriter.Close();
            streamWriter = null;
        }

        /// <summary> Property that return the amount of settings stored in the dictionary </summary>
        public static int SettingsAmount { get { return settings.Count; } }
    }
}
