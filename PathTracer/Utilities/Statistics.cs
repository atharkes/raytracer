using System.Diagnostics;

namespace PathTracer.Utilities {
    /// <summary> Statistics for the <see cref="IRenderer"/> </summary>
    public class Statistics {
        /// <summary> A log for the frame times </summary>
        public readonly TimeLog FrameTime = new();
        /// <summary> A log for the time used by OpenTK </summary>
        public readonly TimeLog OpenTKTime = new();
        /// <summary> A log for the time required by tracing rays </summary>
        public readonly TimeLog IntegratorTime = new();
        /// <summary> A log for the time required by drawing on the screen </summary>
        public readonly TimeLog DrawingTime = new();
        /// <summary> The total amount of samples traced </summary>
        public int SampleCount { get; set; }
        /// <summary> The amount of samples traced last tick </summary>
        public int SampleCountLastTick { get; set; }

        readonly Stopwatch frameTimer = Stopwatch.StartNew();
        readonly Stopwatch taskTimer = Stopwatch.StartNew();

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
