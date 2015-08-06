﻿using System;
using System.Collections.Generic;
using System.Threading;

using MineLib.Core.Wrappers;

namespace MineLib.Android.WrapperInstances
{
    public class ThreadWrapperInstance : IThreadWrapper
    {
        private List<Thread> Threads { get; set; }

        public ThreadWrapperInstance() { Threads = new List<Thread>(); }

        public int StartThread(Action action, bool isBackground, string threadName)
        {
            var thread = new Thread(() => { action(); Threads.Remove(Thread.CurrentThread); });
            thread.Name = threadName;
            thread.IsBackground = isBackground;
            thread.Start();

            Threads.Add(thread);
            return Threads.IndexOf(thread);
        }

        public void AbortThread(int id)
        {
            if (Threads.Count >= id)
                return;

            Threads[id].Abort();
            Threads.RemoveAt(id);
        }

        public bool IsRunning(int id)
        {
            if (Threads.Count >= id)
                return false;

            return Threads[id].IsAlive;
        }
    }
}
