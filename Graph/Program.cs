using Graph;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Utilities.Extensions;
using System.Diagnostics;
using System.Text.Json;

// Create data of a uniform distribution
UniformInterval uniform1 = new(new Interval(0f, 1f));
UniformInterval uniform2 = new(new Interval(1.4f, 1.5f));
ExponentialInterval exponential1 = new(new Interval(0.0f, 1.0f), 1d);
ExponentialInterval exponential2 = new(new Interval(1.1f, 1.3f), 4d);
CombinedDistanceDistribution combinedDistribution = new(exponential1, exponential2, uniform1, uniform2);
PlotData data = new();

const int steps = 1000;
float margin = combinedDistribution.Domain.Size * 0.05f;
float dataStart = combinedDistribution.Domain.Entry - margin;
float dataEnd = combinedDistribution.Domain.Exit + margin;
float stepSize = (dataEnd - dataStart) / steps;

// Determine sample positions
foreach (float transition in combinedDistribution.Domain.Transitions) {
    data.Distances.Add(transition.Previous());
    data.Distances.Add(transition);
    data.Distances.Add(transition.Next());
}
for (float distance = dataStart; distance <= dataEnd; distance += stepSize) {
    data.Distances.Add(distance);
}
data.Distances = new List<float>(data.Distances.GroupBy(d => d).Select(l => l.First()).OrderBy(d => d));

// Log Data per Distribution
foreach (IDistanceDistribution current in combinedDistribution) {
    IDistanceDistribution others = new CombinedDistanceDistribution(combinedDistribution.Where(d => !d.Equals(current)).ToArray());
    DistributionData distributionData = new();
    foreach (float distance in data.Distances) {
        distributionData.MaterialDensities.Add(current.MaterialDensity(distance).TryMakeFinite());
        distributionData.ProbabilityDensities.Add((current.ProbabilityDensity(distance) * (1 - others.CumulativeProbability(distance))).TryMakeFinite());
        distributionData.CumulativeProbabilities.Add(current.CumulativeProbability(distance).TryMakeFinite());
    }
    data.Distributions.Add(current.ToString(), distributionData);
}

// Write distribution data to File
string fileLocation = Environment.CurrentDirectory;
string fileName = @"data.json";
string filePath = Path.Combine(fileLocation, fileName);
using (StreamWriter file = File.CreateText(fileName)) {
    JsonSerializerOptions options = new() {
        WriteIndented = true,
        IncludeFields = true
    };
    string json = JsonSerializer.Serialize(data, options);
    file.Write(json);
}

/// <summary> Try get the path to the python executable </summary>
string PythonPath = PythonFinder.TryGetPythonPath("3.9");
/// <summary> Python script to plot the data with </summary>
string PlotScriptPath = Path.Combine(Environment.CurrentDirectory.Split(@"\Graph\")[0], @"Plot\plot.py");

string indirectArguments = $"--data_path {filePath}";

// Initiate Plot Script
ProcessStartInfo start = new();
start.FileName = PythonPath;
start.Arguments = $"{PlotScriptPath} {indirectArguments}";
start.UseShellExecute = false;
start.RedirectStandardOutput = true;
using (Process? process = Process.Start(start)) {
    if (process == null) throw new InvalidOperationException();
    using StreamReader reader = process.StandardOutput;
    string result = reader.ReadToEnd();
    Console.Write(result);
    Console.ReadLine();
}


string GetDirectArguments(PlotData data) {
    /// <summary> The name of the distances argument in the python script </summary>
    const string DistancesName = "--distances";
    /// <summary> The name of the material densities argument in the python script </summary>
    const string MaterialDensitiesName = "--material_densities";
    /// <summary> The name of the probability densities argument in the python script </summary>
    const string ProbabilityDensitiesName = "--probability_densities";
    /// <summary> The name of the cummulative probabilities argument in the python script </summary>
    const string CumulativeProbabilitiesName = "--cumulative_probabilities";
    /// <summary> Format floating point values without scientific notation </summary>
    string format = "0." + new string('#', 339);
    /// <summary> Arguments to directly transport data via the command line (maximum of 8191 characters on Windows 10) </summary>
    string directArguments =
        $"{DistancesName} {string.Join(" ", data.Distances.Select(d => d.ToString(format)))} " +
        $"{MaterialDensitiesName} {string.Join(" ", data.Distributions.First().Value.MaterialDensities.Select(d => d.ToString(format)))} " +
        $"{ProbabilityDensitiesName} {string.Join(" ", data.Distributions.First().Value.ProbabilityDensities.Select(d => d.ToString(format)))} " +
        $"{CumulativeProbabilitiesName} {string.Join(" ", data.Distributions.First().Value.CumulativeProbabilities.Select(d => d.ToString(format)))}";

    return directArguments;
}