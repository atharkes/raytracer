using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.Spectra;
using System.Diagnostics;

namespace PathTracer.Tests.Quality;

/// <summary> Wall clock cost of tracing rays through a scene object: the hierarchy, the intersection code and the allocations together </summary>
/// <param name="Rays">The amount of rays per round</param>
/// <param name="Rounds">The amount of timed rounds</param>
/// <param name="MeanMicroseconds">The mean time per ray</param>
/// <param name="StandardDeviationMicroseconds">The spread of the time per ray over all rays and rounds</param>
/// <param name="MedianMicroseconds">The median time per ray</param>
/// <param name="P95Microseconds">The 95th percentile of the time per ray</param>
/// <param name="P99Microseconds">The 99th percentile of the time per ray</param>
/// <param name="RoundMeanSpreadMicroseconds">The standard deviation of the per round mean, a measure of the noise between runs</param>
/// <param name="AllocatedBytesPerRay">The bytes allocated per ray</param>
/// <param name="Gen0CollectionsPerThousandRays">The generation 0 collections per thousand rays</param>
public sealed record TraceTiming(int Rays, int Rounds, double MeanMicroseconds, double StandardDeviationMicroseconds, double MedianMicroseconds,
    double P95Microseconds, double P99Microseconds, double RoundMeanSpreadMicroseconds, double AllocatedBytesPerRay, double Gen0CollectionsPerThousandRays) {
    public override string ToString()
        => $"{Rays} rays x {Rounds} rounds: mean {MeanMicroseconds:F2} us, sd {StandardDeviationMicroseconds:F2} us, "
         + $"p50 {MedianMicroseconds:F2} p95 {P95Microseconds:F2} p99 {P99Microseconds:F2} us, round mean sd {RoundMeanSpreadMicroseconds:F3} us, "
         + $"{AllocatedBytesPerRay:F0} B/ray, gen0 {Gen0CollectionsPerThousandRays:F2}/1000 rays";

    public static TraceTiming Measure(ISceneObject root, IReadOnlyList<Ray> rays, int rounds = 5) {
        foreach (var ray in rays) root.Trace(ray, RGBSpectrum.White);

        var durations = new double[rays.Count * rounds];
        var roundMeans = new double[rounds];
        var microsecondsPerTick = 1_000_000d / Stopwatch.Frequency;
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var collectionsBefore = GC.CollectionCount(0);
        for (var round = 0; round < rounds; round++) {
            var roundTotal = 0d;
            for (var i = 0; i < rays.Count; i++) {
                var start = Stopwatch.GetTimestamp();
                root.Trace(rays[i], RGBSpectrum.White);
                var duration = (Stopwatch.GetTimestamp() - start) * microsecondsPerTick;
                durations[round * rays.Count + i] = duration;
                roundTotal += duration;
            }
            roundMeans[round] = roundTotal / rays.Count;
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var collections = GC.CollectionCount(0) - collectionsBefore;

        Array.Sort(durations);
        double Percentile(double p) => durations[Math.Min(durations.Length - 1, (int)(p * durations.Length))];
        var mean = durations.Average();

        return new TraceTiming(
            Rays: rays.Count,
            Rounds: rounds,
            MeanMicroseconds: mean,
            StandardDeviationMicroseconds: StandardDeviation(durations, mean),
            MedianMicroseconds: Percentile(.5),
            P95Microseconds: Percentile(.95),
            P99Microseconds: Percentile(.99),
            RoundMeanSpreadMicroseconds: StandardDeviation(roundMeans, roundMeans.Average()),
            AllocatedBytesPerRay: (double)allocated / durations.Length,
            Gen0CollectionsPerThousandRays: collections * 1000d / durations.Length);
    }

    private static double StandardDeviation(double[] values, double mean) => Math.Sqrt(values.Sum(value => (value - mean) * (value - mean)) / values.Length);
}
