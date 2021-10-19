namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A cummulative distribution/density function </summary>
    /// <typeparam name="T">The variable of the distribution function</typeparam>
    public interface ICDF<T> : IPDF<T> {
        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="ICDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double CumulativeDistribution(T sample);
    }
}
