using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("/////////Successfully ended task\\\\\\\\\\");
            var tcs1 = new TaskCompletionSource<bool>();
            tcs1.SetResult(true);
            Helper.AddContinuations(tcs1.Task);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Console.WriteLine("/////////Canceled task\\\\\\\\\\");
            var tcs2 = new TaskCompletionSource<bool>();
            tcs2.SetCanceled();
            Helper.AddContinuations(tcs2.Task);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Console.WriteLine("/////////Failed task\\\\\\\\\\");
            var t = Task.Factory.StartNew(() => {
                Console.WriteLine($"failed on thread: {Thread.CurrentThread.ManagedThreadId}");
                throw new Exception();
            });
            Helper.AddContinuations(t);
            Console.ReadKey();
        }
    }
}
