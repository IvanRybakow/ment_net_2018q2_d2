using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Task_6
{
    class Program
    {
        private static readonly List<int> nums = new List<int>();
        private static readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent AutoResetEvent2 = new AutoResetEvent(true);
        static void Main(string[] args)
        {
            Task[] tasks = new Task[2];
            tasks[0] = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    AutoResetEvent2.WaitOne();
                    nums.Add(i);
                    AutoResetEvent.Set();
                }
            });

            tasks[1] = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    bool finished = !AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1));
                    if (finished) break;
                    foreach (var i in nums)
                    {
                        Console.WriteLine(i);
                    }

                    AutoResetEvent2.Set();
                }
            });

            Task.WaitAll(tasks);
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
