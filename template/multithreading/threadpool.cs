using System;
using System.Threading;

namespace template.multithreading {
    class Threadpool {
        Thread[] workerThreads;
        EventWaitHandle[] go;
        bool[] done;
        int remaining;
        Action[] tasks;
        int threadCount;

        public Threadpool() {
            threadCount = GetCoreCount();
            remaining = 0;
            workerThreads = new Thread[threadCount];
            go = new EventWaitHandle[threadCount];
            done = new bool[threadCount];
            for (var i = 0; i < threadCount; i++) {
                int threadNumber = i;
                go[i] = new EventWaitHandle(false, EventResetMode.AutoReset);
                done[i] = true;
                Thread workerThread = new Thread(() => ThreadMain(threadNumber));
                workerThread.Start();
            }
        }

        public void ThreadMain(int i) {
            int threadId = i;
            while (true) {
                // Wait for the Go Signal
                go[threadId].WaitOne();
                // Lock to Core
                CPUaffinity.RunOnCore(threadId);
                // Do Tasks
                while (remaining > 0) {
                    int task = Interlocked.Decrement(ref remaining);
                    if (task >= 0) {
                        tasks[task].Invoke();
                    }
                }
                // Signal Done
                done[threadId] = true;
            }
        }

        public void DoTasks(Action[] tasks) {
            // Early Out if Still Busy
            if (remaining > 0) {
                Console.WriteLine("Still Busy: Skipped Work");
                return;
            }
            // Ready new Tasks
            this.tasks = tasks;
            remaining = tasks.Length;
            // Set Done to False
            for (var i = 0; i < threadCount; i++)
                done[i] = false;
            // Give Go Signal
            foreach (EventWaitHandle waitHandle in go)
                waitHandle.Set();
        }

        public bool WorkDone() {
            for (var i = 0; i < threadCount; i++)
                if (!done[i])
                    return false;
            return true;
        }

        public int GetCoreCount() {
            return Environment.ProcessorCount;
        }
    }
}