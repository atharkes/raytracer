using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace Algebra.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class GenericStructBenchmark {
    private double[] _doubles = [];
    private float[] _floats = [];
    private AddDouble[] _wrappedDoubles = [];
    private AddFloat[] _wrappedFloats = [];
    private AddGeneric<double>[] _wrappedGenericDoubles = [];
    private AddGeneric<float>[] _wrappedGenericFloats = [];
    private AddGenericClass<double>[] _wrappedGenericClassDoubles = [];
    private AddClassGeneric<double>[] _wrappedClassGenericDoubles = [];

    [GlobalSetup]
    public void Setup() {
        _doubles = [.. Enumerable.Repeat(0, 1000).Select(_ => Random.Shared.NextDouble())];
        _floats = [.. _doubles.Select(d => (float)d)];
        _wrappedDoubles = [.. _doubles.Select(d => new AddDouble(d))];
        _wrappedFloats = [.. _floats.Select(d => new AddFloat(d))];
        _wrappedGenericDoubles = [.. _doubles.Select(d => new AddGeneric<double>(d))];
        _wrappedGenericFloats = [.. _floats.Select(d => new AddGeneric<float>(d))];
        _wrappedGenericClassDoubles = [.. _doubles.Select(d => new AddGenericClass<double>(d))];
        _wrappedClassGenericDoubles = [.. _doubles.Select(d => new AddClassGeneric<double>(d))];
    }

    [Benchmark(Baseline = true)]
    public double Double() {
        double result = 0;
        for (var i = 0; i < _doubles.Length; i++) {
            result += _doubles[i];
        }
        return result;
    }

    [Benchmark()]
    public float Float() {
        float result = 0;
        for (var i = 0; i < _floats.Length; i++) {
            result += _floats[i];
        }
        return result;
    }

    [Benchmark]
    public double DoubleSum() => _doubles.Sum();

    [Benchmark]
    public float FloatSum() => _floats.Sum();

    [Benchmark]
    public double WrappedDouble() {
        AddDouble result = new(0);
        for (var i = 0; i < _wrappedDoubles.Length; i++) {
            result += _wrappedDoubles[i];
        }
        return result.Value;
    }

    [Benchmark]
    public float WrappedFloat() {
        AddFloat result = new(0);
        for (var i = 0; i < _wrappedFloats.Length; i++) {
            result += _wrappedFloats[i];
        }
        return result.Value;
    }

    [Benchmark]
    public double WrappedGenericDouble() {
        AddGeneric<double> result = new(0);
        for (var i = 0; i < _wrappedGenericDoubles.Length; i++) {
            result += _wrappedGenericDoubles[i];
        }
        return result.Value;
    }

    [Benchmark]
    public float WrappedGenericFloat() {
        AddGeneric<float> result = new(0);
        for (var i = 0; i < _wrappedGenericFloats.Length; i++) {
            result += _wrappedGenericFloats[i];
        }
        return result.Value;
    }

    [Benchmark]
    public double WrappedGenericClassDouble() {
        AddGenericClass<double> result = new(0);
        for (var i = 0; i < _wrappedGenericClassDoubles.Length; i++) {
            result += _wrappedGenericClassDoubles[i];
        }
        return result.Value;
    }

    [Benchmark]
    public double WrappedClassGenericDouble() {
        AddClassGeneric<double> result = new(0);
        for (var i = 0; i < _wrappedClassGenericDoubles.Length; i++) {
            result += _wrappedClassGenericDoubles[i];
        }
        return result.Value;
    }

    private readonly struct AddFloat(float value) : IAdditionOperators<AddFloat, AddFloat, AddFloat> {
        public readonly float Value = value;
        public static AddFloat operator +(AddFloat left, AddFloat right) => new(left.Value + right.Value);
    }

    private readonly struct AddDouble(double value) : IAdditionOperators<AddDouble, AddDouble, AddDouble> {
        public readonly double Value = value;
        public static AddDouble operator +(AddDouble left, AddDouble right) => new(left.Value + right.Value);
    }

    private readonly struct AddGeneric<T>(T value) : IAdditionOperators<AddGeneric<T>, AddGeneric<T>, AddGeneric<T>> where T : struct, IAdditionOperators<T, T, T> {
        public readonly T Value = value;
        public static AddGeneric<T> operator +(AddGeneric<T> left, AddGeneric<T> right) => new(left.Value + right.Value);
    }

    private readonly struct AddGenericClass<T>(T value) : IAdditionOperators<AddGenericClass<T>, AddGenericClass<T>, AddGenericClass<T>> where T : IAdditionOperators<T, T, T> {
        public readonly T Value = value;
        public static AddGenericClass<T> operator +(AddGenericClass<T> left, AddGenericClass<T> right) => new(left.Value + right.Value);
    }

    private class AddClassGeneric<T>(T value) : IAdditionOperators<AddClassGeneric<T>, AddClassGeneric<T>, AddClassGeneric<T>> where T : struct, IAdditionOperators<T, T, T> {
        public readonly T Value = value;
        public static AddClassGeneric<T> operator +(AddClassGeneric<T> left, AddClassGeneric<T> right) => new(left.Value + right.Value);
    }
}
