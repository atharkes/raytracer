namespace Graph;

public class PlotData {
    public List<float> Distances = new();
    public Dictionary<string, DistributionData> Distributions = new();
}

public class DistributionData {
    public List<double> MaterialDensities = new();
    public List<double> ProbabilityDensities = new();
    public List<double> CumulativeProbabilities = new();
}
