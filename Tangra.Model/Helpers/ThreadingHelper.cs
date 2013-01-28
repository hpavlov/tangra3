using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tangra.Model.Helpers
{
    public static class ThreadingHelper
    {
        public static void RunTaskWaitAndDoEvents(ThreadStart methodToInvoke, int pollTimeMs)
        {
            var syncEvent = new ManualResetEvent(true);

            syncEvent.Reset();

            ThreadPool.QueueUserWorkItem(
                delegate(object state)
                {
                    try
                    {
                        methodToInvoke();
                    }
                    finally
                    {
                        syncEvent.Set();
                    }
                });

            do
            {
                bool releaded = syncEvent.WaitOne(pollTimeMs);

                if (releaded)
                    break;

                Application.DoEvents();
            }
            while (true);
        }
    }
}
