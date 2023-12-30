using System.Numerics;
using System.Text;

namespace Algebra.Numbers;
public class VectorizedPga3d {
    public const byte BasisLength = 16;
    public static readonly byte VectorCount = (byte)(BasisLength / Vector<Number>.Count);
    public static readonly byte BitShifts = (byte)(Math.Sqrt(Vector<Number>.Count) + 1);
    // just for debug and print output, the basis names
    public static readonly string[] _basis = ["1", "e0", "e1", "e2", "e3", "e01", "e02", "e03", "e12", "e31", "e23", "e021", "e013", "e032", "e123", "e0123"];

    public readonly Vector<Number>[] values = new Vector<float>[VectorCount];

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="f"></param>
    /// <param name="idx"></param>
    public VectorizedPga3d(Number f = 0f, byte idx = 0) {
        this[idx] = f;
    }

    public VectorizedPga3d(Number[] array) {
        if (array.Length != _basis.Length) throw new ArgumentException(string.Empty, nameof(array));
        for (var i = 0; i < VectorCount; i++) {
            values[i] = new Vector<Number>(array, i * Vector<Number>.Count);
        }
    }

    public VectorizedPga3d(VectorizedPga3d reference) {
        for (var i = 0; i < VectorCount; i++) {
            values[i] = reference.values[i];
        }
    }

    #region Array Access
    public Number this[int index] {
        get {
            var vectorIndex = index >> BitShifts;
            return values[vectorIndex][index - vectorIndex * Vector<Number>.Count];
        }
        set {
            var vectorIndex = index >> BitShifts;
            values[vectorIndex] = values[vectorIndex].WithElement(index - vectorIndex * Vector<Number>.Count, value);
        }
    }
    #endregion

    public VectorizedPga3d With(byte index, Number value) {
        var result = new VectorizedPga3d(this);
        result[index] = value;
        return result;
    }

    public Number[] ToArray() {
        var result = new Number[BasisLength];
        for (var i = 0; i < VectorCount; i++) {
            values[i].CopyTo(result, i * Vector<Number>.Count);
        }
        return result;
    }

    #region Overloaded Operators
    /// <summary>
    /// VectorizedPga3d.Reverse : res = ~a
    /// Reverse the order of the basis blades.
    /// </summary>
    public static VectorizedPga3d operator ~(VectorizedPga3d a) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorCount; i++) {
            result.values[i] = -a.values[i];
        }
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Dual : res = !a
    /// Poincare duality operator.
    /// </summary>
    public static VectorizedPga3d operator !(VectorizedPga3d a) {
        return new(a.ToArray().Reverse().ToArray());
    }

    /// <summary>
    /// VectorizedPga3d.Conjugate : res = a.Conjugate()
    /// Clifford Conjugation
    /// </summary>
    public VectorizedPga3d Conjugate() {
        var result = new VectorizedPga3d();
        result[0] = this[0];
        result[1] = -this[1];
        result[2] = -this[2];
        result[3] = -this[3];
        result[4] = -this[4];
        result[5] = -this[5];
        result[6] = -this[6];
        result[7] = -this[7];
        result[8] = -this[8];
        result[9] = -this[9];
        result[10] = -this[10];
        result[11] = this[11];
        result[12] = this[12];
        result[13] = this[13];
        result[14] = this[14];
        result[15] = this[15];
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Involute : res = a.Involute()
    /// Main involution
    /// </summary>
    public VectorizedPga3d Involute() {
        var result = new VectorizedPga3d();
        result[0] = this[0];
        result[1] = -this[1];
        result[2] = -this[2];
        result[3] = -this[3];
        result[4] = -this[4];
        result[5] = this[5];
        result[6] = this[6];
        result[7] = this[7];
        result[8] = this[8];
        result[9] = this[9];
        result[10] = this[10];
        result[11] = -this[11];
        result[12] = -this[12];
        result[13] = -this[13];
        result[14] = -this[14];
        result[15] = this[15];
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Mul : res = a * b
    /// The geometric product.
    /// </summary>
    public static VectorizedPga3d operator *(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        result[0] = b[0] * a[0] + b[2] * a[2] + b[3] * a[3] + b[4] * a[4] - b[8] * a[8] - b[9] * a[9] - b[10] * a[10] - b[14] * a[14];
        result[1] = b[1] * a[0] + b[0] * a[1] - b[5] * a[2] - b[6] * a[3] - b[7] * a[4] + b[2] * a[5] + b[3] * a[6] + b[4] * a[7] + b[11] * a[8] + b[12] * a[9] + b[13] * a[10] + b[8] * a[11] + b[9] * a[12] + b[10] * a[13] + b[15] * a[14] - b[14] * a[15];
        result[2] = b[2] * a[0] + b[0] * a[2] - b[8] * a[3] + b[9] * a[4] + b[3] * a[8] - b[4] * a[9] - b[14] * a[10] - b[10] * a[14];
        result[3] = b[3] * a[0] + b[8] * a[2] + b[0] * a[3] - b[10] * a[4] - b[2] * a[8] - b[14] * a[9] + b[4] * a[10] - b[9] * a[14];
        result[4] = b[4] * a[0] - b[9] * a[2] + b[10] * a[3] + b[0] * a[4] - b[14] * a[8] + b[2] * a[9] - b[3] * a[10] - b[8] * a[14];
        result[5] = b[5] * a[0] + b[2] * a[1] - b[1] * a[2] - b[11] * a[3] + b[12] * a[4] + b[0] * a[5] - b[8] * a[6] + b[9] * a[7] + b[6] * a[8] - b[7] * a[9] - b[15] * a[10] - b[3] * a[11] + b[4] * a[12] + b[14] * a[13] - b[13] * a[14] - b[10] * a[15];
        result[6] = b[6] * a[0] + b[3] * a[1] + b[11] * a[2] - b[1] * a[3] - b[13] * a[4] + b[8] * a[5] + b[0] * a[6] - b[10] * a[7] - b[5] * a[8] - b[15] * a[9] + b[7] * a[10] + b[2] * a[11] + b[14] * a[12] - b[4] * a[13] - b[12] * a[14] - b[9] * a[15];
        result[7] = b[7] * a[0] + b[4] * a[1] - b[12] * a[2] + b[13] * a[3] - b[1] * a[4] - b[9] * a[5] + b[10] * a[6] + b[0] * a[7] - b[15] * a[8] + b[5] * a[9] - b[6] * a[10] + b[14] * a[11] - b[2] * a[12] + b[3] * a[13] - b[11] * a[14] - b[8] * a[15];
        result[8] = b[8] * a[0] + b[3] * a[2] - b[2] * a[3] + b[14] * a[4] + b[0] * a[8] + b[10] * a[9] - b[9] * a[10] + b[4] * a[14];
        result[9] = b[9] * a[0] - b[4] * a[2] + b[14] * a[3] + b[2] * a[4] - b[10] * a[8] + b[0] * a[9] + b[8] * a[10] + b[3] * a[14];
        result[10] = b[10] * a[0] + b[14] * a[2] + b[4] * a[3] - b[3] * a[4] + b[9] * a[8] - b[8] * a[9] + b[0] * a[10] + b[2] * a[14];
        result[11] = b[11] * a[0] - b[8] * a[1] + b[6] * a[2] - b[5] * a[3] + b[15] * a[4] - b[3] * a[5] + b[2] * a[6] - b[14] * a[7] - b[1] * a[8] + b[13] * a[9] - b[12] * a[10] + b[0] * a[11] + b[10] * a[12] - b[9] * a[13] + b[7] * a[14] - b[4] * a[15];
        result[12] = b[12] * a[0] - b[9] * a[1] - b[7] * a[2] + b[15] * a[3] + b[5] * a[4] + b[4] * a[5] - b[14] * a[6] - b[2] * a[7] - b[13] * a[8] - b[1] * a[9] + b[11] * a[10] - b[10] * a[11] + b[0] * a[12] + b[8] * a[13] + b[6] * a[14] - b[3] * a[15];
        result[13] = b[13] * a[0] - b[10] * a[1] + b[15] * a[2] + b[7] * a[3] - b[6] * a[4] - b[14] * a[5] - b[4] * a[6] + b[3] * a[7] + b[12] * a[8] - b[11] * a[9] - b[1] * a[10] + b[9] * a[11] - b[8] * a[12] + b[0] * a[13] + b[5] * a[14] - b[2] * a[15];
        result[14] = b[14] * a[0] + b[10] * a[2] + b[9] * a[3] + b[8] * a[4] + b[4] * a[8] + b[3] * a[9] + b[2] * a[10] + b[0] * a[14];
        result[15] = b[15] * a[0] + b[14] * a[1] + b[13] * a[2] + b[12] * a[3] + b[11] * a[4] + b[10] * a[5] + b[9] * a[6] + b[8] * a[7] + b[7] * a[8] + b[6] * a[9] + b[5] * a[10] - b[4] * a[11] - b[3] * a[12] - b[2] * a[13] - b[1] * a[14] + b[0] * a[15];
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Wedge : res = a ^ b
    /// The outer product. (MEET)
    /// </summary>
    public static VectorizedPga3d operator ^(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        result[0] = b[0] * a[0];
        result[1] = b[1] * a[0] + b[0] * a[1];
        result[2] = b[2] * a[0] + b[0] * a[2];
        result[3] = b[3] * a[0] + b[0] * a[3];
        result[4] = b[4] * a[0] + b[0] * a[4];
        result[5] = b[5] * a[0] + b[2] * a[1] - b[1] * a[2] + b[0] * a[5];
        result[6] = b[6] * a[0] + b[3] * a[1] - b[1] * a[3] + b[0] * a[6];
        result[7] = b[7] * a[0] + b[4] * a[1] - b[1] * a[4] + b[0] * a[7];
        result[8] = b[8] * a[0] + b[3] * a[2] - b[2] * a[3] + b[0] * a[8];
        result[9] = b[9] * a[0] - b[4] * a[2] + b[2] * a[4] + b[0] * a[9];
        result[10] = b[10] * a[0] + b[4] * a[3] - b[3] * a[4] + b[0] * a[10];
        result[11] = b[11] * a[0] - b[8] * a[1] + b[6] * a[2] - b[5] * a[3] - b[3] * a[5] + b[2] * a[6] - b[1] * a[8] + b[0] * a[11];
        result[12] = b[12] * a[0] - b[9] * a[1] - b[7] * a[2] + b[5] * a[4] + b[4] * a[5] - b[2] * a[7] - b[1] * a[9] + b[0] * a[12];
        result[13] = b[13] * a[0] - b[10] * a[1] + b[7] * a[3] - b[6] * a[4] - b[4] * a[6] + b[3] * a[7] - b[1] * a[10] + b[0] * a[13];
        result[14] = b[14] * a[0] + b[10] * a[2] + b[9] * a[3] + b[8] * a[4] + b[4] * a[8] + b[3] * a[9] + b[2] * a[10] + b[0] * a[14];
        result[15] = b[15] * a[0] + b[14] * a[1] + b[13] * a[2] + b[12] * a[3] + b[11] * a[4] + b[10] * a[5] + b[9] * a[6] + b[8] * a[7] + b[7] * a[8] + b[6] * a[9] + b[5] * a[10] - b[4] * a[11] - b[3] * a[12] - b[2] * a[13] - b[1] * a[14] + b[0] * a[15];
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Vee : res = a & b
    /// The regressive product. (JOIN)
    /// </summary>
    public static VectorizedPga3d operator &(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        result[15] = 1 * (a[15] * b[15]);
        result[14] = -1 * (a[14] * -1 * b[15] + a[15] * b[14] * -1);
        result[13] = -1 * (a[13] * -1 * b[15] + a[15] * b[13] * -1);
        result[12] = -1 * (a[12] * -1 * b[15] + a[15] * b[12] * -1);
        result[11] = -1 * (a[11] * -1 * b[15] + a[15] * b[11] * -1);
        result[10] = 1 * (a[10] * b[15] + a[13] * -1 * b[14] * -1 - a[14] * -1 * b[13] * -1 + a[15] * b[10]);
        result[9] = 1 * (a[9] * b[15] + a[12] * -1 * b[14] * -1 - a[14] * -1 * b[12] * -1 + a[15] * b[9]);
        result[8] = 1 * (a[8] * b[15] + a[11] * -1 * b[14] * -1 - a[14] * -1 * b[11] * -1 + a[15] * b[8]);
        result[7] = 1 * (a[7] * b[15] + a[12] * -1 * b[13] * -1 - a[13] * -1 * b[12] * -1 + a[15] * b[7]);
        result[6] = 1 * (a[6] * b[15] - a[11] * -1 * b[13] * -1 + a[13] * -1 * b[11] * -1 + a[15] * b[6]);
        result[5] = 1 * (a[5] * b[15] + a[11] * -1 * b[12] * -1 - a[12] * -1 * b[11] * -1 + a[15] * b[5]);
        result[4] = 1 * (a[4] * b[15] - a[7] * b[14] * -1 + a[9] * b[13] * -1 - a[10] * b[12] * -1 - a[12] * -1 * b[10] + a[13] * -1 * b[9] - a[14] * -1 * b[7] + a[15] * b[4]);
        result[3] = 1 * (a[3] * b[15] - a[6] * b[14] * -1 - a[8] * b[13] * -1 + a[10] * b[11] * -1 + a[11] * -1 * b[10] - a[13] * -1 * b[8] - a[14] * -1 * b[6] + a[15] * b[3]);
        result[2] = 1 * (a[2] * b[15] - a[5] * b[14] * -1 + a[8] * b[12] * -1 - a[9] * b[11] * -1 - a[11] * -1 * b[9] + a[12] * -1 * b[8] - a[14] * -1 * b[5] + a[15] * b[2]);
        result[1] = 1 * (a[1] * b[15] + a[5] * b[13] * -1 + a[6] * b[12] * -1 + a[7] * b[11] * -1 + a[11] * -1 * b[7] + a[12] * -1 * b[6] + a[13] * -1 * b[5] + a[15] * b[1]);
        result[0] = 1 * (a[0] * b[15] + a[1] * b[14] * -1 + a[2] * b[13] * -1 + a[3] * b[12] * -1 + a[4] * b[11] * -1 + a[5] * b[10] + a[6] * b[9] + a[7] * b[8] + a[8] * b[7] + a[9] * b[6] + a[10] * b[5] - a[11] * -1 * b[4] - a[12] * -1 * b[3] - a[13] * -1 * b[2] - a[14] * -1 * b[1] + a[15] * b[0]);
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Dot : res = a | b
    /// The inner product.
    /// </summary>
    public static VectorizedPga3d operator |(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        result[0] = b[0] * a[0] + b[2] * a[2] + b[3] * a[3] + b[4] * a[4] - b[8] * a[8] - b[9] * a[9] - b[10] * a[10] - b[14] * a[14];
        result[1] = b[1] * a[0] + b[0] * a[1] - b[5] * a[2] - b[6] * a[3] - b[7] * a[4] + b[2] * a[5] + b[3] * a[6] + b[4] * a[7] + b[11] * a[8] + b[12] * a[9] + b[13] * a[10] + b[8] * a[11] + b[9] * a[12] + b[10] * a[13] + b[15] * a[14] - b[14] * a[15];
        result[2] = b[2] * a[0] + b[0] * a[2] - b[8] * a[3] + b[9] * a[4] + b[3] * a[8] - b[4] * a[9] - b[14] * a[10] - b[10] * a[14];
        result[3] = b[3] * a[0] + b[8] * a[2] + b[0] * a[3] - b[10] * a[4] - b[2] * a[8] - b[14] * a[9] + b[4] * a[10] - b[9] * a[14];
        result[4] = b[4] * a[0] - b[9] * a[2] + b[10] * a[3] + b[0] * a[4] - b[14] * a[8] + b[2] * a[9] - b[3] * a[10] - b[8] * a[14];
        result[5] = b[5] * a[0] - b[11] * a[3] + b[12] * a[4] + b[0] * a[5] - b[15] * a[10] - b[3] * a[11] + b[4] * a[12] - b[10] * a[15];
        result[6] = b[6] * a[0] + b[11] * a[2] - b[13] * a[4] + b[0] * a[6] - b[15] * a[9] + b[2] * a[11] - b[4] * a[13] - b[9] * a[15];
        result[7] = b[7] * a[0] - b[12] * a[2] + b[13] * a[3] + b[0] * a[7] - b[15] * a[8] - b[2] * a[12] + b[3] * a[13] - b[8] * a[15];
        result[8] = b[8] * a[0] + b[14] * a[4] + b[0] * a[8] + b[4] * a[14];
        result[9] = b[9] * a[0] + b[14] * a[3] + b[0] * a[9] + b[3] * a[14];
        result[10] = b[10] * a[0] + b[14] * a[2] + b[0] * a[10] + b[2] * a[14];
        result[11] = b[11] * a[0] + b[15] * a[4] + b[0] * a[11] - b[4] * a[15];
        result[12] = b[12] * a[0] + b[15] * a[3] + b[0] * a[12] - b[3] * a[15];
        result[13] = b[13] * a[0] + b[15] * a[2] + b[0] * a[13] - b[2] * a[15];
        result[14] = b[14] * a[0] + b[0] * a[14];
        result[15] = b[15] * a[0] + b[0] * a[15];
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Add : res = a + b
    /// Multivector addition
    /// </summary>
    public static VectorizedPga3d operator +(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorCount; i++) {
            result.values[i] = a.values[i] + b.values[i];
        }
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.Sub : res = a - b
    /// Multivector subtraction
    /// </summary>
    public static VectorizedPga3d operator -(VectorizedPga3d a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorCount; i++) {
            result.values[i] = a.values[i] - b.values[i];
        }
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.smul : res = a * b
    /// scalar/multivector multiplication
    /// </summary>
    public static VectorizedPga3d operator *(Number a, VectorizedPga3d b) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorCount; i++) {
            result.values[i] = a * b.values[i];
        }
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.muls : res = a * b
    /// multivector/scalar multiplication
    /// </summary>
    public static VectorizedPga3d operator *(VectorizedPga3d a, Number b) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorCount; i++) {
            result.values[i] = a.values[i] * b;
        }
        return result;
    }

    /// <summary>
    /// VectorizedPga3d.sadd : res = a + b
    /// scalar/multivector addition
    /// </summary>
    public static VectorizedPga3d operator +(Number a, VectorizedPga3d b) {
        return b.With(0, a + b.values[0][0]);
    }

    /// <summary>
    /// VectorizedPga3d.adds : res = a + b
    /// multivector/scalar addition
    /// </summary>
    public static VectorizedPga3d operator +(VectorizedPga3d a, Number b) {
        return a.With(0, a.values[0][0] + b);
    }

    /// <summary>
    /// VectorizedPga3d.ssub : res = a - b
    /// scalar/multivector subtraction
    /// </summary>
    public static VectorizedPga3d operator -(Number a, VectorizedPga3d b) {
        return (~b).With(0, a - b.values[0][0]);
    }

    /// <summary>
    /// VectorizedPga3d.subs : res = a - b
    /// multivector/scalar subtraction
    /// </summary>
    public static VectorizedPga3d operator -(VectorizedPga3d a, Number b) {
        return a.With(0, a.values[0][0] - b);
    }
    #endregion

    /// <summary>
    /// VectorizedPga3d.norm()
    /// Calculate the Euclidean norm. (strict positive).
    /// </summary>
    public Number Norm() => (Number)Math.Sqrt(Math.Abs((this * Conjugate())[0]));

    /// <summary>
    /// VectorizedPga3d.inorm()
    /// Calculate the Ideal norm. (signed)
    /// </summary>
    public Number SignedNorm() => this[1] != 0.0f ? this[1] : this[15] != 0.0f ? this[15] : (!this).Norm();

    /// <summary>
    /// VectorizedPga3d.normalized()
    /// Returns a normalized (Euclidean) element.
    /// </summary>
    public VectorizedPga3d Normalized() => this * (1 / Norm());

    // PGA is plane based. Vectors are planes. (think linear functionals)
    public static readonly VectorizedPga3d e0 = new(1f, 1);
    public static readonly VectorizedPga3d e1 = new(1f, 2);
    public static readonly VectorizedPga3d e2 = new(1f, 3);
    public static readonly VectorizedPga3d e3 = new(1f, 4);

    // PGA lines are bivectors.
    public static readonly VectorizedPga3d e01 = e0 ^ e1;
    public static readonly VectorizedPga3d e02 = e0 ^ e2;
    public static readonly VectorizedPga3d e03 = e0 ^ e3;
    public static readonly VectorizedPga3d e12 = e1 ^ e2;
    public static readonly VectorizedPga3d e31 = e3 ^ e1;
    public static readonly VectorizedPga3d e23 = e2 ^ e3;

    // PGA points are trivectors.
    public static readonly VectorizedPga3d e123 = e1 ^ e2 ^ e3; // the origin
    public static readonly VectorizedPga3d e032 = e0 ^ e3 ^ e2;
    public static readonly VectorizedPga3d e013 = e0 ^ e1 ^ e3;
    public static readonly VectorizedPga3d e021 = e0 ^ e2 ^ e1;

    /// <summary>
    /// VectorizedPga3d.plane(a,b,c,d)
    /// A plane is defined using its homogenous equation ax + by + cz + d = 0
    /// </summary>
    public static VectorizedPga3d Plane(Number a, Number b, Number c, Number d)
        => a * e1 + b * e2 + c * e3 + d * e0;

    /// <summary>
    /// VectorizedPga3d.point(x,y,z)
    /// A point is just a homogeneous point, Euclidean coordinates plus the origin
    /// </summary>
    public static VectorizedPga3d Point(Number x, Number y, Number z)
        => e123 + x * e032 + y * e013 + z * e021;

    /// <summary>
    /// Rotors (Euclidean lines)
    /// </summary>
    public static VectorizedPga3d Rotor(Number angle, VectorizedPga3d line)
        => (Number)Math.Cos(angle / 2.0f) + (Number)Math.Sin(angle / 2.0f) * line.Normalized();

    /// <summary>
    /// Translators (ideal lines)
    /// </summary>
    public static VectorizedPga3d Translator(Number distance, VectorizedPga3d line)
        => 1.0f + distance / 2.0f * line;

    // for our toy problem (generate points on the surface of a torus)
    // we start with a function that generates motors.
    // circle(t) with t going from 0 to 1.
    public static VectorizedPga3d Circle(Number t, Number radius, VectorizedPga3d line)
        => Rotor(t * 2.0f * (Number)Math.PI, line) * Translator(radius, e1 * e0);

    // a torus is now the product of two circles. 
    public static VectorizedPga3d Torus(Number s, Number t, Number r1, VectorizedPga3d l1, Number r2, VectorizedPga3d l2)
        => Circle(s, r2, l2) * Circle(t, r1, l1);

    /// string cast
    public override string ToString() {
        var sb = new StringBuilder();
        var n = 0;
        for (var i = 0; i < 16; ++i) {
            if (this[i] != 0.0f) {
                _ = sb.Append($"{this[i]}{(i == 0 ? string.Empty : _basis[i])} + ");
                n++;
            }
        }
        if (n == 0) _ = sb.Append('0');
        return sb.ToString().TrimEnd(' ', '+');
    }
}
