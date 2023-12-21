global using Number = float;
using System.Text;

namespace Algebra;
public class PGA3D {
    // just for debug and print output, the basis names
    public static readonly string[] _basis = ["1", "e0", "e1", "e2", "e3", "e01", "e02", "e03", "e12", "e31", "e23", "e021", "e013", "e032", "e123", "e0123"];

    private readonly float[] _mVec = new float[16];

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="f"></param>
    /// <param name="idx"></param>
    public PGA3D(float f = 0f, byte idx = 0) {
        _mVec[idx] = f;
    }

    #region Array Access
    public float this[int idx] {
        get => _mVec[idx];
        set => _mVec[idx] = value;
    }
    #endregion

    #region Overloaded Operators
    /// <summary>
    /// PGA3D.Reverse : res = ~a
    /// Reverse the order of the basis blades.
    /// </summary>
    public static PGA3D operator ~(PGA3D a) {
        var result = new PGA3D();
        result[0] = a[0];
        result[1] = a[1];
        result[2] = a[2];
        result[3] = a[3];
        result[4] = a[4];
        result[5] = -a[5];
        result[6] = -a[6];
        result[7] = -a[7];
        result[8] = -a[8];
        result[9] = -a[9];
        result[10] = -a[10];
        result[11] = -a[11];
        result[12] = -a[12];
        result[13] = -a[13];
        result[14] = -a[14];
        result[15] = a[15];
        return result;
    }

    /// <summary>
    /// PGA3D.Dual : res = !a
    /// Poincare duality operator.
    /// </summary>
    public static PGA3D operator !(PGA3D a) {
        var result = new PGA3D();
        result[0] = a[15];
        result[1] = a[14];
        result[2] = a[13];
        result[3] = a[12];
        result[4] = a[11];
        result[5] = a[10];
        result[6] = a[9];
        result[7] = a[8];
        result[8] = a[7];
        result[9] = a[6];
        result[10] = a[5];
        result[11] = a[4];
        result[12] = a[3];
        result[13] = a[2];
        result[14] = a[1];
        result[15] = a[0];
        return result;
    }

    /// <summary>
    /// PGA3D.Conjugate : res = a.Conjugate()
    /// Clifford Conjugation
    /// </summary>
    public PGA3D Conjugate() {
        var result = new PGA3D();
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
    /// PGA3D.Involute : res = a.Involute()
    /// Main involution
    /// </summary>
    public PGA3D Involute() {
        var result = new PGA3D();
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
    /// PGA3D.Mul : res = a * b
    /// The geometric product.
    /// </summary>
    public static PGA3D operator *(PGA3D a, PGA3D b) {
        var result = new PGA3D();
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
    /// PGA3D.Wedge : res = a ^ b
    /// The outer product. (MEET)
    /// </summary>
    public static PGA3D operator ^(PGA3D a, PGA3D b) {
        var result = new PGA3D();
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
    /// PGA3D.Vee : res = a & b
    /// The regressive product. (JOIN)
    /// </summary>
    public static PGA3D operator &(PGA3D a, PGA3D b) {
        var result = new PGA3D();
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
    /// PGA3D.Dot : res = a | b
    /// The inner product.
    /// </summary>
    public static PGA3D operator |(PGA3D a, PGA3D b) {
        var result = new PGA3D();
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
    /// PGA3D.Add : res = a + b
    /// Multivector addition
    /// </summary>
    public static PGA3D operator +(PGA3D a, PGA3D b) {
        var result = new PGA3D();
        result[0] = a[0] + b[0];
        result[1] = a[1] + b[1];
        result[2] = a[2] + b[2];
        result[3] = a[3] + b[3];
        result[4] = a[4] + b[4];
        result[5] = a[5] + b[5];
        result[6] = a[6] + b[6];
        result[7] = a[7] + b[7];
        result[8] = a[8] + b[8];
        result[9] = a[9] + b[9];
        result[10] = a[10] + b[10];
        result[11] = a[11] + b[11];
        result[12] = a[12] + b[12];
        result[13] = a[13] + b[13];
        result[14] = a[14] + b[14];
        result[15] = a[15] + b[15];
        return result;
    }

    /// <summary>
    /// PGA3D.Sub : res = a - b
    /// Multivector subtraction
    /// </summary>
    public static PGA3D operator -(PGA3D a, PGA3D b) {
        var result = new PGA3D();
        result[0] = a[0] - b[0];
        result[1] = a[1] - b[1];
        result[2] = a[2] - b[2];
        result[3] = a[3] - b[3];
        result[4] = a[4] - b[4];
        result[5] = a[5] - b[5];
        result[6] = a[6] - b[6];
        result[7] = a[7] - b[7];
        result[8] = a[8] - b[8];
        result[9] = a[9] - b[9];
        result[10] = a[10] - b[10];
        result[11] = a[11] - b[11];
        result[12] = a[12] - b[12];
        result[13] = a[13] - b[13];
        result[14] = a[14] - b[14];
        result[15] = a[15] - b[15];
        return result;
    }

    /// <summary>
    /// PGA3D.smul : res = a * b
    /// scalar/multivector multiplication
    /// </summary>
    public static PGA3D operator *(float a, PGA3D b) {
        var result = new PGA3D();
        result[0] = a * b[0];
        result[1] = a * b[1];
        result[2] = a * b[2];
        result[3] = a * b[3];
        result[4] = a * b[4];
        result[5] = a * b[5];
        result[6] = a * b[6];
        result[7] = a * b[7];
        result[8] = a * b[8];
        result[9] = a * b[9];
        result[10] = a * b[10];
        result[11] = a * b[11];
        result[12] = a * b[12];
        result[13] = a * b[13];
        result[14] = a * b[14];
        result[15] = a * b[15];
        return result;
    }

    /// <summary>
    /// PGA3D.muls : res = a * b
    /// multivector/scalar multiplication
    /// </summary>
    public static PGA3D operator *(PGA3D a, float b) {
        var result = new PGA3D();
        result[0] = a[0] * b;
        result[1] = a[1] * b;
        result[2] = a[2] * b;
        result[3] = a[3] * b;
        result[4] = a[4] * b;
        result[5] = a[5] * b;
        result[6] = a[6] * b;
        result[7] = a[7] * b;
        result[8] = a[8] * b;
        result[9] = a[9] * b;
        result[10] = a[10] * b;
        result[11] = a[11] * b;
        result[12] = a[12] * b;
        result[13] = a[13] * b;
        result[14] = a[14] * b;
        result[15] = a[15] * b;
        return result;
    }

    /// <summary>
    /// PGA3D.sadd : res = a + b
    /// scalar/multivector addition
    /// </summary>
    public static PGA3D operator +(float a, PGA3D b) {
        var result = new PGA3D();
        result[0] = a + b[0];
        result[1] = b[1];
        result[2] = b[2];
        result[3] = b[3];
        result[4] = b[4];
        result[5] = b[5];
        result[6] = b[6];
        result[7] = b[7];
        result[8] = b[8];
        result[9] = b[9];
        result[10] = b[10];
        result[11] = b[11];
        result[12] = b[12];
        result[13] = b[13];
        result[14] = b[14];
        result[15] = b[15];
        return result;
    }

    /// <summary>
    /// PGA3D.adds : res = a + b
    /// multivector/scalar addition
    /// </summary>
    public static PGA3D operator +(PGA3D a, float b) {
        var result = new PGA3D();
        result[0] = a[0] + b;
        result[1] = a[1];
        result[2] = a[2];
        result[3] = a[3];
        result[4] = a[4];
        result[5] = a[5];
        result[6] = a[6];
        result[7] = a[7];
        result[8] = a[8];
        result[9] = a[9];
        result[10] = a[10];
        result[11] = a[11];
        result[12] = a[12];
        result[13] = a[13];
        result[14] = a[14];
        result[15] = a[15];
        return result;
    }

    /// <summary>
    /// PGA3D.ssub : res = a - b
    /// scalar/multivector subtraction
    /// </summary>
    public static PGA3D operator -(float a, PGA3D b) {
        var result = new PGA3D();
        result[0] = a - b[0];
        result[1] = -b[1];
        result[2] = -b[2];
        result[3] = -b[3];
        result[4] = -b[4];
        result[5] = -b[5];
        result[6] = -b[6];
        result[7] = -b[7];
        result[8] = -b[8];
        result[9] = -b[9];
        result[10] = -b[10];
        result[11] = -b[11];
        result[12] = -b[12];
        result[13] = -b[13];
        result[14] = -b[14];
        result[15] = -b[15];
        return result;
    }

    /// <summary>
    /// PGA3D.subs : res = a - b
    /// multivector/scalar subtraction
    /// </summary>
    public static PGA3D operator -(PGA3D a, float b) {
        var result = new PGA3D();
        result[0] = a[0] - b;
        result[1] = a[1];
        result[2] = a[2];
        result[3] = a[3];
        result[4] = a[4];
        result[5] = a[5];
        result[6] = a[6];
        result[7] = a[7];
        result[8] = a[8];
        result[9] = a[9];
        result[10] = a[10];
        result[11] = a[11];
        result[12] = a[12];
        result[13] = a[13];
        result[14] = a[14];
        result[15] = a[15];
        return result;
    }
    #endregion

    /// <summary>
    /// PGA3D.norm()
    /// Calculate the Euclidean norm. (strict positive).
    /// </summary>
    public float Norm() => (float)Math.Sqrt(Math.Abs((this * Conjugate())[0]));

    /// <summary>
    /// PGA3D.inorm()
    /// Calculate the Ideal norm. (signed)
    /// </summary>
    public float SignedNorm() => this[1] != 0.0f ? this[1] : this[15] != 0.0f ? this[15] : (!this).Norm();

    /// <summary>
    /// PGA3D.normalized()
    /// Returns a normalized (Euclidean) element.
    /// </summary>
    public PGA3D Normalized() => this * (1 / Norm());

    // PGA is plane based. Vectors are planes. (think linear functionals)
    public static readonly PGA3D e0 = new(1f, 1);
    public static readonly PGA3D e1 = new(1f, 2);
    public static readonly PGA3D e2 = new(1f, 3);
    public static readonly PGA3D e3 = new(1f, 4);

    // PGA lines are bivectors.
    public static readonly PGA3D e01 = e0 ^ e1;
    public static readonly PGA3D e02 = e0 ^ e2;
    public static readonly PGA3D e03 = e0 ^ e3;
    public static readonly PGA3D e12 = e1 ^ e2;
    public static readonly PGA3D e31 = e3 ^ e1;
    public static readonly PGA3D e23 = e2 ^ e3;

    // PGA points are trivectors.
    public static readonly PGA3D e123 = e1 ^ e2 ^ e3; // the origin
    public static readonly PGA3D e032 = e0 ^ e3 ^ e2;
    public static readonly PGA3D e013 = e0 ^ e1 ^ e3;
    public static readonly PGA3D e021 = e0 ^ e2 ^ e1;

    /// <summary>
    /// PGA3D.plane(a,b,c,d)
    /// A plane is defined using its homogenous equation ax + by + cz + d = 0
    /// </summary>
    public static PGA3D Plane(float a, float b, float c, float d)
        => a * e1 + b * e2 + c * e3 + d * e0;

    /// <summary>
    /// PGA3D.point(x,y,z)
    /// A point is just a homogeneous point, Euclidean coordinates plus the origin
    /// </summary>
    public static PGA3D Point(float x, float y, float z)
        => e123 + x * e032 + y * e013 + z * e021;

    /// <summary>
    /// Rotors (Euclidean lines)
    /// </summary>
    public static PGA3D Rotor(float angle, PGA3D line)
        => (float)Math.Cos(angle / 2.0f) + (float)Math.Sin(angle / 2.0f) * line.Normalized();

    /// <summary>
    /// Translators (ideal lines)
    /// </summary>
    public static PGA3D Translator(float distance, PGA3D line)
        => 1.0f + distance / 2.0f * line;

    // for our toy problem (generate points on the surface of a torus)
    // we start with a function that generates motors.
    // circle(t) with t going from 0 to 1.
    public static PGA3D Circle(float t, float radius, PGA3D line)
        => Rotor(t * 2.0f * (float)Math.PI, line) * Translator(radius, e1 * e0);

    // a torus is now the product of two circles. 
    public static PGA3D Torus(float s, float t, float r1, PGA3D l1, float r2, PGA3D l2)
        => Circle(s, r2, l2) * Circle(t, r1, l1);

    /// string cast
    public override string ToString() {
        var sb = new StringBuilder();
        var n = 0;
        for (var i = 0; i < 16; ++i) {
            if (_mVec[i] != 0.0f) {
                _ = sb.Append($"{_mVec[i]}{(i == 0 ? string.Empty : _basis[i])} + ");
                n++;
            }
        }
        if (n == 0) _ = sb.Append('0');
        return sb.ToString().TrimEnd(' ', '+');
    }
}

public static class Program {
    private static PGA3D PointOnTorus(float s, float t) {
        var to = PGA3D.Torus(s, t, 0.25f, PGA3D.e12, 0.6f, PGA3D.e31);
        return to * PGA3D.e123 * ~to;
    }

    public static void Main() {
        // Elements of the even subalgebra (scalar + bivector + pss) of unit length are motors
        var rot = PGA3D.Rotor((float)Math.PI / 2.0f, PGA3D.e1 * PGA3D.e2);

        // The outer product ^ is the MEET. Here we intersect the yz (x=0) and xz (y=0) planes.
        var ax_z = PGA3D.e1 ^ PGA3D.e2;

        // line and plane meet in point. We intersect the line along the z-axis (x=0,y=0) with the xy (z=0) plane.
        var origin = ax_z ^ PGA3D.e3;

        // We can also easily create points and join them into a line using the regressive (vee, &) product.
        var px = PGA3D.Point(1, 0, 0);
        var line = origin & px;

        // Lets also create the plane with equation 2x + z - 3 = 0
        var p = PGA3D.Plane(2, 0, 1, -3);

        // rotations work on all elements
        var rotated_plane = rot * p * ~rot;
        var rotated_line = rot * line * ~rot;
        var rotated_point = rot * px * ~rot;

        // See the 3D PGA Cheat sheet for a huge collection of useful formulas
        var point_on_plane = (p | px) * p;

        // Some output
        Console.WriteLine("a point       : " + px);
        Console.WriteLine("a line        : " + line);
        Console.WriteLine("a plane       : " + p);
        Console.WriteLine("a rotor       : " + rot);
        Console.WriteLine("rotated line  : " + rotated_line);
        Console.WriteLine("rotated point : " + rotated_point);
        Console.WriteLine("rotated plane : " + rotated_plane);
        Console.WriteLine("point on plane: " + point_on_plane.Normalized());
        Console.WriteLine("point on torus: " + PointOnTorus(0.0f, 0.0f));
    }
}
