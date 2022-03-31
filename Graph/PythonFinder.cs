using Microsoft.Win32;

namespace Graph {
    /// <summary> Helper class to find the python executable </summary>
    public static class PythonFinder {
        /// <summary> Try to get the path to the python executable.
        /// Credits: https://stackoverflow.com/questions/41920032/automatically-find-the-path-of-the-python-executable </summary>
        /// <param name="requiredVersion">The required python version</param>
        /// <param name="maxVersion">The maximum python version</param>
        /// <returns>The path to the python executable</returns>
        /// <exception cref="PlatformNotSupportedException">Only windows is supported</exception>
        /// <exception cref="InvalidOperationException">Python has to be installed</exception>
        public static string TryGetPythonPath(string requiredVersion = "", string maxVersion = "") {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Currently python can only be found on Windows");

            string[] possiblePythonLocations = new string[3] {
                @"HKLM\SOFTWARE\Python\PythonCore\",
                @"HKCU\SOFTWARE\Python\PythonCore\",
                @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\"
            };

            Dictionary<string, string> pythonLocations = new();

            foreach (string possibleLocation in possiblePythonLocations) {
                string regKey = possibleLocation[..4], actualPath = possibleLocation.Substring(5);
                RegistryKey theKey = regKey == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser;
                RegistryKey? theValue = theKey.OpenSubKey(actualPath);
                if (theValue == null) continue;
                foreach (var v in theValue.GetSubKeyNames()) {
                    RegistryKey? productKey = theValue.OpenSubKey(v);
                    string? pythonExePath = productKey?.OpenSubKey("InstallPath")?.GetValue("ExecutablePath")?.ToString();
                    if (pythonExePath != null && pythonExePath != "") {
                        pythonLocations.Add(v.ToString(), pythonExePath);
                    }
                }
            }

            if (pythonLocations.Count > 0) {
                Version desiredVersion = new(requiredVersion == "" ? "0.0.1" : requiredVersion);
                Version maxPVersion = new(maxVersion == "" ? "999.999.999" : maxVersion);

                string highestVersion;
                string highestVersionPath = "";

                foreach (KeyValuePair<string, string> pVersion in pythonLocations) {
                    //TODO; if on 64-bit machine, prefer the 64 bit version over 32 and vice versa
                    int index = pVersion.Key.IndexOf("-"); //For x-32 and x-64 in version numbers
                    string formattedVersion = index > 0 ? pVersion.Key[..index] : pVersion.Key;

                    Version thisVersion = new Version(formattedVersion);
                    int comparison = desiredVersion.CompareTo(thisVersion),
                        maxComparison = maxPVersion.CompareTo(thisVersion);

                    if (comparison <= 0) {
                        if (maxComparison >= 0) {
                            desiredVersion = thisVersion;
                            highestVersion = pVersion.Key;
                            highestVersionPath = pVersion.Value;
                        }
                    }
                }
                return highestVersionPath;
            }

            throw new InvalidOperationException("A path to a python executable could not be detected");
        }
    }
}
