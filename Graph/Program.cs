using Graph;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
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

UniformInterval uniform = new(new Interval(0, 1));
const int steps = 100;
float stepSize = uniform.Domain.CoveredArea / steps;
float dataStart = uniform.Domain.Entry - 0.1f;
float dataEnd = uniform.Domain.Exit + 0.1f;

List<float> distances = new();
List<float> materialDensities = new();
List<double> probabilityDensities = new();
List<double> cummulativeProbabilities = new();

for (float distance = dataStart; distance <= dataEnd; distance += stepSize) {
    distances.Add(distance);
    float materialDensiy = uniform.Domain.Includes(distance) ? 1f / ((float)uniform.Domain.Exit - (float)distance) : 0f;
    materialDensities.Add(materialDensiy);
    probabilityDensities.Add(uniform.ProbabilityDensity(distance));
    cummulativeProbabilities.Add(uniform.CumulativeProbability(distance));
}

/// <summary> Arguments of the python script as defined by argparse </summary>
string format = "0." + new string('#', 339);
string arguments =
    $"{DistancesName} {string.Join(" ", distances.Select(d => d.ToString(format)))} " +
    $"{MaterialDensitiesName} {string.Join(" ", materialDensities.Select(d => d.ToString(format)))} " +
    $"{ProbabilityDensitiesName} {string.Join(" ", probabilityDensities.Select(d => d.ToString(format)))} " +
    $"{CummulativeProbabilitiesName} {string.Join(" ", cummulativeProbabilities.Select(d => d.ToString(format)))}";

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

Console.WriteLine(arguments);
Console.ReadLine();
