using PathTracer.Geometry.Positions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PathTracer.Pathtracing.Distributions.Intervals;

/// <summary> An interval </summary>
public readonly struct Interval : IInterval {
    /// <summary> The entry point of the <see cref="Interval"/> </summary>
    public Position1 Entry { get; }
    /// <summary> The exit pointof the <see cref="Interval"/> </summary>
    public Position1 Exit { get; }

    /// <summary> Create a new <see cref="Interval"/> </summary>
    /// <param name="entry">The entry point of the <see cref="Interval"/></param>
    /// <param name="exit">The exit point of the <see cref="Interval"/></param>
    public Interval(Position1 entry, Position1 exit) {
        Debug.Assert(!float.IsNaN(entry) && !float.IsNaN(exit));
        Entry = entry;
        Exit = exit;
    }

    public static bool operator ==(Interval left, Interval right) => left.Equals(right);
    public static bool operator !=(Interval left, Interval right) => !(left == right);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Interval other && Entry.Equals(other.Entry) && Exit.Equals(other.Exit);
    public override int GetHashCode() => HashCode.Combine(287655961, Entry, Exit);
    public override string? ToString() => $"[{Entry},{Exit}]";
}
