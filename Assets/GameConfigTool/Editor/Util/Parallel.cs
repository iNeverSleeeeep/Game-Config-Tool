using System;
using System.Collections.Generic;
using System.Threading;

namespace GCT
{
    public class Parallel
    {
        public static void ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                throw new ArgumentNullException("enumerable");
            if (action == null)
                throw new ArgumentNullException("action");

            var resetEvents = new List<ManualResetEvent>();

            foreach (var item in items)
            {
                var evt = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem((i) =>
                {
                    action((T)i);
                    evt.Set();
                }, item);
                resetEvents.Add(evt);
            }

            WaitHandle.WaitAll(resetEvents.ToArray());
        }
    }
}