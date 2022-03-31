﻿using Graph;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Intervals;
using System.Diagnostics;
using System.Text.Json;

Dictionary<string, DistributionData> data = new();

// Create data of a uniform distribution
UniformInterval uniform = new(new Interval(0f, 1f));
UniformInterval uniform2 = new(new Interval(0.5f, 1.5f));
CombinedDistanceDistribution distribution = new(uniform, uniform2);

const int steps = 1000;
float margin = distribution.Domain.Size * 0.05f;
float dataStart = distribution.Domain.Entry - margin;
float dataEnd = distribution.Domain.Exit + margin;
float stepSize = (dataEnd - dataStart) / steps;

DistributionData uniformData = new();
for (float distance = dataStart; distance <= dataEnd; distance += stepSize) {
    uniformData.Distances.Add(distance);
    uniformData.MaterialDensities.Add(distribution.MaterialDensity(distance));
    uniformData.ProbabilityDensities.Add(distribution.ProbabilityDensity(distance));
    uniformData.CummulativeProbabilities.Add(distribution.CumulativeProbability(distance));
}

// Write distribution data to File
data.Add(distribution.ToString(), uniformData);
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
}

Console.ReadLine();


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