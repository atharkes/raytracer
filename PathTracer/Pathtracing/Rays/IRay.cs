﻿using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Pathtracing.Rays {
    /// <summary> An interface for an <see cref="IRay"/> traversing through space </summary>
    public interface IRay {
        /// <summary> The starting position of the <see cref="IRay"/> </summary>
        Vector3 Origin { get; }
        /// <summary> The direction the <see cref="IRay"/> is travelling in </summary>
        Vector3 Direction { get; }
        /// <summary> The total distance the <see cref="IRay"/> is travelling </summary>
        float Length { get; set; }
        /// <summary> The destination of the <see cref="IRay"/> </summary>
        Vector3 Destination => Travel(Length);

        /// <summary> The inverted <see cref="Direction"/>. Useful for quick <see cref="AxisAlignedBox"/> intersections </summary>
        Vector3 InvDirection => new(1 / Direction.X, 1 / Direction.Y, 1 / Direction.Z);
        /// <summary> Whether the individual components of the <see cref="InvDirection"/> of the <see cref="IRay"/> are negative </summary>
        Vector3i Sign => new(InvDirection.X < 0 ? 1 : 0, InvDirection.Y < 0 ? 1 : 0, InvDirection.Z < 0 ? 1 : 0);

        /// <summary> Get a point by travelling from the <see cref="Origin"/> a specified <paramref name="distance"/> towards the <see cref="Direction"/> </summary>
        /// <param name="distance">The distance to travel along the <see cref="IRay"/></param>
        /// <returns>A point on the <see cref="IRay"/> after travelling the specified <paramref name="distance"/></returns>
        Vector3 Travel(float distance) {
            return Origin + Direction * distance;
        }

        /// <summary> Check whether the specified <paramref name="distance"/> is within the <see cref="IRay"/> interval </summary>
        /// <param name="distance">The specified distance to check</param>
        /// <returns>Whether the <paramref name="distance"/> falls in the <see cref="IRay"/> interval</returns>
        bool WithinBounds(float distance) {
            return 0 < distance && distance < Length;
        }

        /// <summary> Check whether the <see cref="IRay"/> enters given a specified surface <paramref name="normal"/> </summary>
        /// <param name="normal">The surface normal to check</param>
        /// <returns>Whether the <see cref="IRay"/> enters for the specified surface <paramref name="normal"/></returns>
        bool Enters(Vector3 normal) {
            return Vector3.Dot(Direction, normal) < 0;
        }

        /// <summary> Check whether the <see cref="IRay"/> exits given a specified surface <paramref name="normal"/> </summary>
        /// <param name="normal">The surface normal to check</param>
        /// <returns>Whether the <see cref="IRay"/> exits for the specified surface <paramref name="normal"/></returns>
        bool Exits(Vector3 normal) {
            return Vector3.Dot(Direction, normal) > 0;
        }
    }
}
