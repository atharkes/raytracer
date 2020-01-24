using System.Diagnostics;

namespace WhittedRaytracer.Utilities {
    /// <summary> Statistics for the raytracer </summary>
    class Statistics {
        /// <summary> Amount of rays traced </summary>
        public int RaysTraced { get; private set; } = 0;
        /// <summary> Amount of rays traced in the previous tick </summary>
        public int RaysTracedLastTick { get; private set; } = 0;

        /// <summary> A log for the frame times </summary>
        public readonly TimeLog FrameTime = new TimeLog();
        /// <summary> A log for the time used by OpenTK </summary>
        public readonly TimeLog OpenTKTime = new TimeLog();
        /// <summary> A log for the overhead created by multithreading </summary>
        public readonly TimeLog MultithreadingOverhead = new TimeLog();
        /// <summary> A log for the time required by tracing rays </summary>
        public readonly TimeLog TracingTime = new TimeLog();
        /// <summary> A log for the time required by drawing on the screen </summary>
        public readonly TimeLog DrawingTime = new TimeLog();

        readonly Stopwatch frameTimer = Stopwatch.StartNew();
        readonly Stopwatch taskTimer = Stopwatch.StartNew();

        /// <summary> Log how many rays where traced in this tick </summary>
        /// <param name="tickRayAmount">The amount of rays traced this tick</param>
        public void LogTickRays(int tickRayAmount) {
            RaysTraced += tickRayAmount;
            RaysTracedLastTick = tickRayAmount;
        }

        /// <summary> Log the time used for this frame </summary>
        public void LogFrameTime() {
            FrameTime.LogTickTime(frameTimer.Elapsed);
            frameTimer.Restart();
        }

        /// <summary> Log how much time the previous task used </summary>
        /// <param name="taskLog">The task that was done</param>
        public void LogTaskTime(TimeLog taskLog) {
            taskLog.LogTickTime(taskTimer.Elapsed);
            taskTimer.Restart();
        }
    }
}
