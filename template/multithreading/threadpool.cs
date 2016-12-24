using System;
using System.Threading;

namespace template.multithreading
{
	class threadpool
	{
		Thread[] workerThreads;
		EventWaitHandle[] go;
		bool[] done;
		int remaining;
		Action[] tasks;
		int threadCount;

		public threadpool()
		{
			threadCount = getCoreCount();
			remaining = 0;
			workerThreads = new Thread[threadCount];
			go = new EventWaitHandle[threadCount];
			done = new bool[threadCount];
			for (var i = 0; i < threadCount; i++)
			{
				int threadNumber = i;
				go[i] = new EventWaitHandle(false, EventResetMode.AutoReset);
				done[i] = true;
				Thread workerThread = new Thread(() => threadMain(threadNumber));
				workerThread.Start();
			}
		}

		public void threadMain(int i)
		{
			int threadId = i;
			while (true)
			{
				// Wait for the Go Signal
				go[threadId].WaitOne();
				//System.Diagnostics.Debug.WriteLine(threadId + ") Im doing something");
				// Lock to Core
				cpuaffinity.RunOnCore(threadId);
				// Do Tasks
				while (remaining > 0)
				{
					int task = Interlocked.Decrement(ref remaining);
					if (task >= 0)
					{
						//System.Diagnostics.Debug.WriteLine(threadId + ") Im doing task: " + task);
						tasks[task].Invoke();
					}
				}
				// Signal Done
				done[threadId] = true;
			}
		}

		public void doTasks(Action[] tasks)
		{
			// Early Out if Still Busy
			if (remaining > 0)
			{
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

		public bool workDone()
		{
			//System.Diagnostics.Debug.WriteLine("check");
			for (var i = 0; i < threadCount; i++)
				if (!done[i])
					return false;
			return true;
		}

		public int getCoreCount()
		{
			return Environment.ProcessorCount;
		}
	}
}
