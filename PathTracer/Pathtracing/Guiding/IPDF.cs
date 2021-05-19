using System;

namespace PathTracer.Pathtracing.Guiding {
    public interface IPDF<T> {
        T Sample(Random random);
        float Probability(T sample);
    }
}
