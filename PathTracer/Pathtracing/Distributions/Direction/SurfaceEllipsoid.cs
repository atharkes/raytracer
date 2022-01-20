using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    /// <summary> Used for the symmetric surface GGX distribution for visible normals </summary>
    public struct SurfaceEllipsoid : IDirectionDistribution, IEquatable<SurfaceEllipsoid> {
        public Normal3 Orientation { get; }
        public float Roughness { get; }
        public Normal3 IncomingDirection { get; }

        public bool ContainsDelta => Roughness.Equals(0d);
        public double DomainSize => Roughness.Equals(0d) ? 0d : 4 * Math.PI;

        public SurfaceEllipsoid(Normal3 orientation, float roughness, Normal3 incomingDirection) {
            Orientation = orientation;
            Roughness = roughness;
            IncomingDirection = incomingDirection;
        }

        public bool Contains(Normal3 sample) {
            throw new NotImplementedException();
        }

        public double ProbabilityDensity(Normal3 sample) {
            throw new NotImplementedException();
        }

        public Normal3 Sample(Random random) {
            return new Normal3(SampleVNDF(random.NextSingle(), random.NextSingle()));
        }

        public override bool Equals(object? obj) => obj is SurfaceEllipsoid ssggx && Equals(ssggx);
        public bool Equals(IProbabilityDistribution<Normal3>? other) => other is SurfaceEllipsoid ssggx && Equals(ssggx);
        public bool Equals(SurfaceEllipsoid other) => Orientation.Equals(other.Orientation) && Roughness.Equals(other.Roughness) && IncomingDirection.Equals(other.IncomingDirection);
        public override int GetHashCode() => HashCode.Combine(579526993, Orientation, Roughness, IncomingDirection);

        public static bool operator ==(SurfaceEllipsoid left, SurfaceEllipsoid right) => left.Equals(right);
        public static bool operator !=(SurfaceEllipsoid left, SurfaceEllipsoid right) => !(left == right);

        Vector3 SampleVNDF(float r1, float r2) {
            // Compute S for surface-like SGGX
            float roughness2 = Roughness * Roughness;
            float Sxx = Orientation.X * Orientation.X + roughness2 * (Orientation.Y * Orientation.Y + Orientation.Z * Orientation.Z);
            float Sxy = Orientation.X * Orientation.Y + roughness2 * (-Orientation.X * Orientation.Y);
            float Sxz = Orientation.X * Orientation.Z + roughness2 * (-Orientation.X * Orientation.Z);
            float Syy = Orientation.Y * Orientation.Y + roughness2 * (Orientation.X * Orientation.X + Orientation.Z * Orientation.Z);
            float Syz = Orientation.Y * Orientation.Z + roughness2 * (-Orientation.Y * Orientation.Z);
            float Szz = Orientation.Z * Orientation.Z + roughness2 * (Orientation.X * Orientation.X + Orientation.Y * Orientation.Y);

            // Generate sample (u, v, w)
            float r = (float)Math.Sqrt(r1);
            float phi = 2.0f * (float)Math.PI * r2;
            float u = r * (float)Math.Cos(phi);
            float v = r * (float)Math.Sin(phi);
            float w = (float)Math.Sqrt(1.0f - u * u - v * v);

            // Build orthonormal basis
            Vector3 wi = -IncomingDirection.Vector;
            Vector3 wk, wj;
            if (wi.Z < -0.9999999f) {
                wk = new Vector3(0.0f, -1.0f, 0.0f);
                wj = new Vector3(-1.0f, 0.0f, 0.0f);
            } else {
                float a = 1.0f / (1.0f + wi.Z);
                float b = -wi.X * wi.Y * a;
                wk = new Vector3(1.0f - wi.X * wi.X * a, b, -wi.X);
                wj = new Vector3(b, 1.0f - wi.Y * wi.Y * a, -wi.Y);
            }

            // Project S in this basis
            float Skk = wk.X * wk.X * Sxx + wk.Y * wk.Y * Syy + wk.Z * wk.Z * Szz + 2.0f * (wk.X * wk.Y * Sxy + wk.X * wk.Z * Sxz + wk.Y * wk.Z * Syz);
            float Sjj = wj.X * wj.X * Sxx + wj.Y * wj.Y * Syy + wj.Z * wj.Z * Szz + 2.0f * (wj.X * wj.Y * Sxy + wj.X * wj.Z * Sxz + wj.Y * wj.Z * Syz);
            float Sii = wi.X * wi.X * Sxx + wi.Y * wi.Y * Syy + wi.Z * wi.Z * Szz + 2.0f * (wi.X * wi.Y * Sxy + wi.X * wi.Z * Sxz + wi.Y * wi.Z * Syz);
            float Skj = wk.X * wj.X * Sxx + wk.Y * wj.Y * Syy + wk.Z * wj.Z * Szz + (wk.X * wj.Y + wk.Y * wj.X) * Sxy + (wk.X * wj.Z + wk.Z * wj.X) * Sxz + (wk.Y * wj.Z + wk.Z * wj.Y) * Syz;
            float Ski = wk.X * wi.X * Sxx + wk.Y * wi.Y * Syy + wk.Z * wi.Z * Szz + (wk.X * wi.Y + wk.Y * wi.X) * Sxy + (wk.X * wi.Z + wk.Z * wi.X) * Sxz + (wk.Y * wi.Z + wk.Z * wi.Y) * Syz;
            float Sji = wj.X * wi.X * Sxx + wj.Y * wi.Y * Syy + wj.Z * wi.Z * Szz + (wj.X * wi.Y + wj.Y * wi.X) * Sxy + (wj.X * wi.Z + wj.Z * wi.X) * Sxz + (wj.Y * wi.Z + wj.Z * wi.Y) * Syz;

            // Compute normal
            float sqrtDetSkji = (float)Math.Sqrt(Math.Abs(Skk * Sjj * Sii - Skj * Skj * Sii - Ski * Ski * Sjj - Sji * Sji * Skk + 2.0f * Skj * Ski * Sji));
            float inv_sqrtS_ii = 1.0f / (float)Math.Sqrt(Sii);
            float tmp = (float)Math.Sqrt(Sjj * Sii - Sji * Sji);
            Vector3 Mk = new(sqrtDetSkji/ tmp, 0.0f, 0.0f);
            Vector3 Mj = new(-inv_sqrtS_ii * (Ski * Sji - Skj * Sii) / tmp, inv_sqrtS_ii* tmp, 0);
            Vector3 Mi = new(inv_sqrtS_ii* Ski, inv_sqrtS_ii* Sji, inv_sqrtS_ii* Sii);
            Vector3 wm_kji = (u * Mk + v * Mj + w * Mi).Normalized();

            // Rotate back to world basis
            return wm_kji.X * wk + wm_kji.Y * wj + wm_kji.Z * wi;
        }
    }
}
