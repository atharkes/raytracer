using Graph;
using System.Diagnostics;

/// <summary> Try get the path to the python executable </summary>
string PythonPath = PythonFinder.TryGetPythonPath("3.9");
/// <summary> Python script to plot the data with </summary>
string FilePath = Path.Combine(Environment.CurrentDirectory.Split(@"\Graph\")[0], @"Plot\plot.py");
/// <summary> The name of the distances argument in the python script </summary>
const string DistancesName = "--distances";
/// <summary> The name of the material densities argument in the python script </summary>
const string MaterialDensitiesName = "--material_densities";
/// <summary> The name of the probability densities argument in the python script </summary>
const string ProbabilityDensitiesName = "--probability_densities";
/// <summary> The name of the cummulative probabilities argument in the python script </summary>
const string CummulativeProbabilitiesName = "--cummulative_probabilities";

List<float> distances = new() { 0, 1 };
List<float> materialDensities = new() { 0, 1 };
List<float> probabilityDensities = new() { 0, 1 };
List<float> cummulativeProbabilities = new() { 0, 1 };

/// <summary> Arguments of the python script as defined by argparse </summary>
string arguments = 
    $"{DistancesName} {string.Join(" ", distances)} " +
    $"{MaterialDensitiesName} {string.Join(" ", materialDensities)} " +
    $"{ProbabilityDensitiesName} {string.Join(" ", probabilityDensities)} " +
    $"{CummulativeProbabilitiesName} {string.Join(" ", cummulativeProbabilities)}";

ProcessStartInfo start = new();
start.FileName = PythonPath;
start.Arguments = string.Format("{0} {1}", FilePath, arguments);
start.UseShellExecute = false;
start.RedirectStandardOutput = true;
using (Process? process = Process.Start(start)) {
    if (process == null) throw new InvalidOperationException();
    using StreamReader reader = process.StandardOutput;
    string result = reader.ReadToEnd();
    Console.Write(result);
}
