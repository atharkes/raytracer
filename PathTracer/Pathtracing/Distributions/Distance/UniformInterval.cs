﻿using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Utilities.Extensions;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct UniformInterval : IDistanceDistribution {
        public IInterval Domain { get; }

        public UniformInterval(IInterval interval) {
            Domain = interval;
        }

        /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
        /// <param name="distance">The distance to get the material density at</param>
        /// <returns>The material density at the specified <paramref name="distance"/></returns>
        public double MaterialDensity(Position1 distance) {
            return Domain.Includes(distance) ? 1d / (Domain.Exit - distance) : 0f;
        }

        public Position1 Sample(Random random) {
            return Domain.Entry + (float)(random.NextDouble() * Domain.CoveredArea);
        }

        public double ProbabilityDensity(Position1 sample) {
            return Domain.Includes(sample) ? 1d / Domain.CoveredArea : 0d;
        }

        public double CumulativeProbability(Position1 sample) {
            return Math.Min(Math.Max(0d, ((float)sample).Previous() - Domain.Entry), Domain.CoveredArea) / Domain.CoveredArea;
        }

        public override bool Equals(object? obj) => obj is UniformInterval ud && Equals(ud);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is UniformInterval ud && Equals(ud);
        public bool Equals(UniformInterval other) => Domain.Equals(other.Domain);
        public override int GetHashCode() => HashCode.Combine(963929819, Domain);
        public override string ToString() => $"Uniform{Domain}";

        public static bool operator ==(UniformInterval left, UniformInterval right) => left.Equals(right);
        public static bool operator !=(UniformInterval left, UniformInterval right) => !(left == right);
    }
}
