using System;
using System.Runtime.InteropServices;

namespace PathTracer.Multithreading {
    /// <summary> Class to set cpu affinity of a thread </summary>
    class CPUaffinity {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll")]
        static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        /// <summary> Set the current thread to run on a specific core </summary>
        /// <param name="cpu">The core to run the thread on</param>
        public static void RunOnCore(int cpu) {
            var ptr = GetCurrentThread();
            SetThreadAffinityMask(ptr, new IntPtr(1 << cpu));
        }
    }
}