﻿using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using System;

namespace PathTracer.Pathtracing.Distributions.Direction {
    public struct SphericalDiffuse : IDirectionDistribution {
        public Normal3 Orientation { get; }
        public bool ContainsDelta => false;
        public double DomainSize => 4 * Math.PI;

        public SphericalDiffuse(Normal3 orientation) {
            Orientation = orientation;
        }

        public bool Contains(Normal3 sample) => true;

        public double ProbabilityDensity(Normal3 sample) => 1 / DomainSize;

        public Normal3 Sample(Random random) {
            ISphere sphere = new UnitSphere(Position3.Origin);
            return ((Direction3)sphere.SurfacePosition(random)).Normalized();
        }
    }
}
