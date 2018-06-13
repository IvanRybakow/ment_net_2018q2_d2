using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_7
{
    public static class Helper
    {
        private static List<Task> tasks = new List<Task>();
        private static readonly Action<Task> onlyIfNotRanToCompletion = task => Console.WriteLine($"I will be printed if previous task not ran to completion");
        private static readonly Action<Task> anyway = task => Console.WriteLine($"I will be printed anyway");
        private static readonly Action<Task> afterFailInTheSamethread = task =>
            Console.WriteLine($"I will be printed if previous task failed. On the same thread {Thread.CurrentThread.ManagedThreadId}");
        private static readonly Action<Task> afterCancelOutsideThreadPool = task =>
        Console.WriteLine($"I will be printed if previous task canceled. Outside of thread pool. Thread From Pool: {Thread.CurrentThread.IsThreadPoolThread}");
        public static void AddContinuations(Task t)
        {
            t.ContinueWith(anyway);
            t.ContinueWith(onlyIfNotRanToCompletion, TaskContinuationOptions.NotOnRanToCompletion);
            t.ContinueWith(afterFailInTheSamethread, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            t.ContinueWith(afterCancelOutsideThreadPool, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, new CustomTaskScheduler());
        }
    }
}
