using System;
using System.Linq;
using System.Threading;

namespace PathTracer.Multithreading {
    /// <summary> A threadpool for dividing work between threads </summary>
    public class Threadpool : IDisposable {
        /// <summary> Amount of cores on this system </summary>
        public static int CoreCount => Environment.ProcessorCount;

        /// <summary> Amount of threads used for the threadpool </summary>
        public int ThreadCount { get; set; }
        /// <summary> Ideal amount of tasks per thread </summary>
        public int TasksPerThread { get; set; } = 16;
        /// <summary> The amount of tasks created for the threadpool </summary>
        public int MultithreadingTaskCount => ThreadCount * TasksPerThread;

        readonly Thread[] workerThreads;
        readonly EventWaitHandle[] go;
        readonly EventWaitHandle[] doneHandle;
        readonly bool[] done;
        int remaining;
        Action[]? tasks;
        bool active = true;
        bool disposed = false;

        /// <summary> Create a new threadpool </summary>
        /// <param name="threadCount">Amount of threads used for the threadpool</param>
        public Threadpool(int threadCount = 0) {
            ThreadCount = threadCount == 0 ? CoreCount : threadCount;
            remaining = 0;
            workerThreads = new Thread[ThreadCount];
            go = new EventWaitHandle[ThreadCount];
            doneHandle = new EventWaitHandle[ThreadCount];
            done = new bool[ThreadCount];
            for (var i = 0; i < ThreadCount; i++) {
                int threadNumber = i;
                go[i] = new EventWaitHandle(false, EventResetMode.AutoReset);
                doneHandle[i] = new EventWaitHandle(false, EventResetMode.AutoReset);
                done[i] = true;
                Thread workerThread = new(() => ThreadMain(threadNumber));
                workerThread.Start();
            }
        }

        /// <summary> Let the threadpool do some tasks </summary>
        /// <param name="tasks">The tasks to do with the threadpool</param>
        public void DoTasks(Action[] tasks) {
            if (remaining > 0) {
                Console.WriteLine("Still Busy: Waiting till earlier work is done");
                WaitTillDone();
            }
            this.tasks = tasks;
            remaining = tasks.Length;
            for (var i = 0; i < ThreadCount; i++) done[i] = false;
            // Give Go Signal
            foreach (EventWaitHandle waitHandle in go) waitHandle.Set();
        }

        /// <summary> Check if the work is done </summary>
        /// <returns>Whether the work is done</returns>
        public bool WorkDone() {
            return done.All(b => b);
        }

        /// <summary> Wait till all threads are done </summary>
        public void WaitTillDone() {
            WaitHandle.WaitAll(doneHandle);
        }

        /// <summary> Main method for the worker threads </summary>
        /// <param name="threadID">The identifier of the thread</param>
        void ThreadMain(int threadID) {
            while (active) {
                // Wait for the Go Signal
                go[threadID].WaitOne();
                // Lock to Core
                CPUaffinity.RunOnCore(threadID);
                // Do Tasks
                while (remaining > 0) {
                    int task = Interlocked.Decrement(ref remaining);
                    if (task >= 0) {
                        tasks?[task].Invoke();
                    }
                }
                // Signal Done
                done[threadID] = true;
                doneHandle[threadID].Set();
            }
        }

        /// <summary> Dispose the <see cref="ThreadPool"/> </summary>
        /// <param name="disposing">Whether to recursivly dispose managed resources</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                active = false;
                tasks = null;
                foreach (EventWaitHandle waitHandle in go) waitHandle.Set();
                disposed = true;
            }
        }

        /// <summary> Dispose the <see cref="ThreadPool"/> </summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Deconstruct the <see cref="ThreadPool"/> </summary>
        ~Threadpool() {
            Dispose(disposing: false);
        }
    }
}