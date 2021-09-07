using System;

namespace PathTracer.Pathtracing.Guiding {
    public interface IPDF<In, Out> {
        Out Sample(In input, Random random);
        float Probability(In input, Out sample);
    }
    public interface IPDF<Out> {
        Out Sample(Random random);
        float Probability(Out sample);
    }
}
