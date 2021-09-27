namespace PathTracer.Pathtracing.Boundaries {
    public interface IBoundaryInterval {
        /// <summary> The entry point of the <see cref="IBoundaryInterval"/> </summary>
        IBoundaryPoint Entry { get; }
        /// <summary> The exit point of the <see cref="IBoundaryInterval"/> </summary>
        IBoundaryPoint Exit { get; }

        /// <summary> Whether the <see cref="IBoundaryInterval"/> is a valid interval </summary>
        public bool Valid => Entry.Distance <= Exit.Distance;
        /// <summary> Whether the <see cref="IBoundaryInterval"/> is volumetric </summary>
        public bool Volumetric => Entry.Distance < Exit.Distance;
        /// <summary> Whether the <see cref="IBoundaryInterval"/> is planar </summary>
        public bool Planar => Entry.Distance == Exit.Distance;

        /// <summary> Check whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls inside the <see cref="IBoundaryInterval"/></returns>
        public bool Inside(double distance) => Entry.Distance <= distance && distance <= Exit.Distance;

        /// <summary> Check whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryInterval"/> </summary>
        /// <param name="distance">the specified distance to check for</param>
        /// <returns>Whether the specified <paramref name="distance"/> falls outside the <see cref="IBoundaryInterval"/></returns>
        public bool Outside(double distance) => distance < Entry.Distance || Exit.Distance < distance;
    }
}
