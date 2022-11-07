using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace PathTracer.Multithreading {
    /// <summary> Class to set cpu affinity of a thread </summary>
    class CPUaffinity {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        /// <summary> Set the current thread to run on a specific core </summary>
        /// <param name="coreIndex">The core to run the thread on</param>
        public static void RunOnCore(int coreIndex) {
            var threadPointer = GetCurrentThread();
            SetThreadAffinityMask(threadPointer, new IntPtr(1 << coreIndex));
        }

        static ProcessThread CurrentProcessThread() {
            int threadId = GetCurrentThreadId();
            return Process.GetCurrentProcess().Threads.Cast<ProcessThread>().Single(t => t.Id == threadId);
        }
    }
}