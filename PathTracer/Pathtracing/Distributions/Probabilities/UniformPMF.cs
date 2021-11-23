using System;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A discreet <see cref="IPDF{T}"/> (probability mass function) with equal probabilities per item </summary>
    /// <typeparam name="T">The type of samples in the <see cref="UniformPMF{T}"/></typeparam>
    public class UniformPMF<T> : IPMF<T> {
        /// <summary> Whether the <see cref="UniformPMF{T}"/> has only a single solution </summary>
        public bool SingleSolution => items.Length == 1;
        /// <summary> The domain size of the <see cref="UniformPMF{T}"/> </summary>
        public int DomainSize => items.Length;

        readonly T[] items;

        /// <summary> Create a <see cref="UniformPMF{T}"/> with the specified <paramref name="items"/> </summary>
        /// <param name="items">The </param>
        public UniformPMF(params T[] items) {
            this.items = items;
        }

        /// <summary> Create a <see cref="UniformPMF{T}"/> from two equally typed <see cref="UniformPMF{T}"/>s </summary>
        /// <param name="left">The left <see cref="UniformPMF{T}"/></param>
        /// <param name="right">The right <see cref="UniformPMF{T}"/></param>
        public UniformPMF(UniformPMF<T> left, UniformPMF<T> right) {
            items = left.items.Concat(right.items).ToArray();
        }

        /// <summary> Check whether the <see cref="UniformPMF{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="UniformPMF{T}"/></returns>
        public bool Contains(T sample) => items.Contains(sample);

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="UniformPMF{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        public double ProbabilityMass(T sample) => 1 / (double)items.Length;

        /// <summary> Sample the <see cref="UniformPMF{T}{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        public T Sample(Random random) => items[random.Next(0, items.Length)];
    }
}
