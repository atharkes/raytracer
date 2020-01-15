using System;

namespace WhittedRaytracer.Utilities {
    /// <summary> A log for times </summary>
    class TimeLog {
        /// <summary> The total time spend </summary>
        public TimeSpan Total { get; private set; }
        /// <summary> Amount of time spend in the last frame </summary>
        public TimeSpan LastTick { get; private set; }

        /// <summary> Log a new tick time </summary>
        /// <param name="tickTime">The tick time to be logged</param>
        public void LogTickTime(TimeSpan tickTime) {
            Total += tickTime;
            LastTick = tickTime;
        }
    }
}
