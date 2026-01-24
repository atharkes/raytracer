using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace UnitTests.Pathtracing.Spectra;

[TestClass]
public class RGBSpectrumTest {
    [TestMethod]
    public void Contructor() => _ = new RGBSpectrum(Random.Shared.Vector());

    [TestMethod]
    public void Equality() {
        RGBSpectrum a = new(1, 1, 1);
        RGBSpectrum b = new(1, 1, 1);
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void Inequality() {
        RGBSpectrum a = new(1, 1, 1);
        RGBSpectrum b = new(0, 0, 0);
        Assert.AreNotEqual(b, a);
    }
}
