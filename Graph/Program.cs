using Graph;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
using System.Diagnostics;
using System.Text.Json;

Dictionary<string, DistributionData> data = new();

// Create data of a uniform distribution
UniformInterval uniform = new(new Interval(0, 1));
const int distributionSteps = 100;
float stepSize = uniform.Domain.CoveredArea / distributionSteps;
float dataStart = uniform.Domain.Entry - 0.1f;
float dataEnd = uniform.Domain.Exit + 0.1f;

DistributionData uniformData = new();
for (float distance = dataStart; distance <= dataEnd; distance += stepSize) {
    uniformData.Distances.Add(distance);
    float materialDensiy = uniform.Domain.Includes(distance) ? 1f / ((float)uniform.Domain.Exit - (float)distance) : 0f;
    uniformData.MaterialDensities.Add(materialDensiy);
    uniformData.ProbabilityDensities.Add(uniform.ProbabilityDensity(distance));
    uniformData.CummulativeProbabilities.Add(uniform.CumulativeProbability(distance));
}

// Write distribution data to File
data.Add(uniform.ToString(), uniformData);
string fileLocation = Environment.CurrentDirectory;
string fileName = @"data.json";
string filePath = Path.Combine(fileLocation, fileName);
using (StreamWriter file = File.CreateText(fileName)) {
    JsonSerializerOptions options = new() { WriteIndented = true, IncludeFields = true };
    string json = JsonSerializer.Serialize(data, options);
    file.Write(json);
}

/// <summary> Try get the path to the python executable </summary>
string PythonPath = PythonFinder.TryGetPythonPath("3.9");
/// <summary> Python script to plot the data with </summary>
string PlotScriptPath = Path.Combine(Environment.CurrentDirectory.Split(@"\Graph\")[0], @"Plot\plot.py");

string indirectArguments = $"--filepath {filePath}";

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
}


string GetDirectArguments(DistributionData data) {
    /// <summary> The name of the distances argument in the python script </summary>
    const string DistancesName = "--distances";
    /// <summary> The name of the material densities argument in the python script </summary>
    const string MaterialDensitiesName = "--material_densities";
    /// <summary> The name of the probability densities argument in the python script </summary>
    const string ProbabilityDensitiesName = "--probability_densities";
    /// <summary> The name of the cummulative probabilities argument in the python script </summary>
    const string CummulativeProbabilitiesName = "--cummulative_probabilities";
    /// <summary> Format floating point values without scientific notation </summary>
    string format = "0." + new string('#', 339);
    /// <summary> Arguments to directly transport data via the command line (maximum of 8191 characters on Windows 10) </summary>
    string directArguments =
        $"{DistancesName} {string.Join(" ", data.Distances.Select(d => d.ToString(format)))} " +
        $"{MaterialDensitiesName} {string.Join(" ", data.MaterialDensities.Select(d => d.ToString(format)))} " +
        $"{ProbabilityDensitiesName} {string.Join(" ", data.ProbabilityDensities.Select(d => d.ToString(format)))} " +
        $"{CummulativeProbabilitiesName} {string.Join(" ", data.CummulativeProbabilities.Select(d => d.ToString(format)))}";

    return directArguments;
}