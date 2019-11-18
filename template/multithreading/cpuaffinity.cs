using System;
using System.Runtime.InteropServices;

namespace Raytracer.Multithreading {
    class CPUaffinity {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll")]
        static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        public static void RunOnCore(int cpu) {
            var ptr = GetCurrentThread();
            SetThreadAffinityMask(ptr, new IntPtr(1 << cpu));
        }
    }
}