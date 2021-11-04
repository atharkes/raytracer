using System;
using System.Linq;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A discreet <see cref="IPDF{T}"/> (probability mass function) with equal probabilities per item </summary>
    /// <typeparam name="T">The type of samples in the <see cref="PMF{T}"/></typeparam>
    public class PMF<T> : IPDF<T> {
        /// <summary> Whether the <see cref="PMF{T}"/> has only a single solution </summary>
        public bool SingleSolution => items.Length == 1;
        /// <summary> The domain size of the <see cref="PMF{T}"/> </summary>
        public double DomainSize => items.Length;

        readonly T[] items;

        /// <summary> Create a <see cref="PMF{T}"/> with the specified <paramref name="items"/> </summary>
        /// <param name="items">The </param>
        public PMF(params T[] items) {
            this.items = items;
        }

        /// <summary> Create a <see cref="PMF{T}"/> from two equally typed <see cref="PMF{T}"/>s </summary>
        /// <param name="left">The left <see cref="PMF{T}"/></param>
        /// <param name="right">The right <see cref="PMF{T}"/></param>
        public PMF(PMF<T> left, PMF<T> right) {
            items = left.items.Concat(right.items).ToArray();
        }

        /// <summary> Check whether the <see cref="PMF{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="PMF{T}"/></returns>
        public bool Contains(T sample) => items.Contains(sample);

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="PMF{T}{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        public double Probability(T sample) => 1 / (double)items.Length;

        /// <summary> Sample the <see cref="PMF{T}{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        public T Sample(Random random) => items[random.Next(0, items.Length)];
    }
}
