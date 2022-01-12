using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    /// <summary> Used for the surface GGX distribution for visible normals </summary>
    public struct SurfaceHalfEllipsoid : IDirectionDistribution, IEquatable<SurfaceHalfEllipsoid> {
        public Normal3 Orientation { get; }
        public float Roughness { get; }
        public Normal3 IncomingDirection { get; }

        public bool ContainsDelta => Roughness.Equals(0d);
        public double DomainSize => Roughness.Equals(0d) ? 0d : 2 * Math.PI;

        public SurfaceHalfEllipsoid(Normal3 orientation, float roughness, Normal3 incomingDirection) {
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
            if (Normal3.Similarity(Orientation, IncomingDirection) < -0.999f) {
                return Normal3.Perpendicular(Orientation, IncomingDirection);
            }
            Normal3 sample = new(SampleVNDF(random.NextSingle(), random.NextSingle()));
            while (!Normal3.Similar(sample, Orientation)) {
                sample = new(SampleVNDF(random.NextSingle(), random.NextSingle()));
            }
            return sample;
        }

        public override bool Equals(object? obj) => obj is SurfaceHalfEllipsoid sggx && Equals(sggx);
        public bool Equals(IProbabilityDistribution<Normal3>? other) => other is SurfaceHalfEllipsoid sggx && Equals(sggx);
        public bool Equals(SurfaceHalfEllipsoid other) => Orientation.Equals(other.Orientation) && Roughness.Equals(other.Roughness) && IncomingDirection.Equals(other.IncomingDirection);
        public override int GetHashCode() => HashCode.Combine(395397523, Orientation, Roughness, IncomingDirection);

        public static bool operator ==(SurfaceHalfEllipsoid left, SurfaceHalfEllipsoid right) => left.Equals(right);
        public static bool operator !=(SurfaceHalfEllipsoid left, SurfaceHalfEllipsoid right) => !(left == right);

        Vector3 SampleVNDF(float r1, float r2) {
            var rotation = new OpenTK.Mathematics.Quaternion(Vector3.Cross(Orientation.Vector, Vector3.UnitZ), Vector3.Dot(Orientation.Vector, Vector3.UnitZ) + 1).Normalized();
            Vector3 Ve = rotation * -IncomingDirection.Vector; 

            // Section 3.2: transforming the view direction to the hemisphere configuration
            Vector3 Vh = new Vector3(Roughness * Ve.X, Roughness * Ve.Y, Ve.Z).Normalized();

            // Section 4.1: orthonormal basis (with special case if cross product is zero)
            float lensq = Vh.X * Vh.X + Vh.Y * Vh.Y;
            Vector3 T1 = lensq > 0 ? new Vector3(-Vh.Y, Vh.X, 0) / (float)Math.Sqrt(lensq) : new Vector3(1f, 0f, 0f);
            Vector3 T2 = Vector3.Cross(Vh, T1);

            // Section 4.2: parameterization of the projected area
            float r = (float)Math.Sqrt(r1);
            float phi = 2.0f * (float)Math.PI * r2;
            float t1 = r * (float)Math.Cos(phi);
            float t2 = r * (float)Math.Sin(phi);
            float s = 0.5f * (1f + Vh.Z);
            t2 = (1f - s) * (float)Math.Sqrt(1f - t1 * t1) + s * t2;

            // Section 4.3: reprojection onto hemisphere
            Vector3 Nh = t1 * T1 + t2 * T2 + (float)Math.Sqrt(Math.Max(0f, 1f - t1 * t1 - t2 * t2)) * Vh;

            // Section 3.4: transforming the normal back to the ellipsoid configuration
            Vector3 Ne = new(Roughness * Nh.X, Roughness * Nh.Y, (float)Math.Max(0.0, Nh.Z));

            return rotation.Inverted() * Ne;
        }
    }
}
