using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PathTracer.Multithreading;

/// <summary> Class to set cpu affinity of a thread </summary>
internal partial class CPUaffinity {
    [LibraryImport("kernel32.dll")]
    private static partial IntPtr GetCurrentThread();

    [LibraryImport("kernel32.dll")]
    private static partial int GetCurrentThreadId();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

    /// <summary> Set the current thread to run on a specific core </summary>
    /// <param name="coreIndex">The core to run the thread on</param>
    public static void RunOnCore(int coreIndex) {
        var threadPointer = GetCurrentThread();
        SetThreadAffinityMask(threadPointer, new IntPtr(1 << coreIndex));
    }

    private static ProcessThread CurrentProcessThread() {
        var threadId = GetCurrentThreadId();
        return Process.GetCurrentProcess().Threads.Cast<ProcessThread>().Single(t => t.Id == threadId);
    }
}