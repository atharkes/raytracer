﻿using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics {
    /// <summary> A perfect sphere <see cref="IShape"/> </summary>
    public struct Sphere : ISphere {
        /// <summary> Position of the <see cref="Sphere"/> </summary>
        public Position3 Position { get; }
        /// <summary> The radius of the <see cref="Sphere"/> </summary>
        public Vector1 Radius { get; }

        /// <summary> Create a new <see cref="Sphere"/> </summary>
        /// <param name="position">The position of the <see cref="Sphere"/></param>
        /// <param name="radius">The radius of the <see cref="Sphere"/></param>
        /// <param name="material">The material of the <see cref="Sphere"/></param>
        public Sphere(Position3 position, float radius = 1) {
            Position = position;
            Radius = radius;
        }
    }
}