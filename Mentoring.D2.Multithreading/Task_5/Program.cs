using System;
using System.Threading;

namespace Task_5
{
    class Program
    {
        private static int _start;
        private static readonly Semaphore semaphore = new Semaphore(0,1);
        static void Main(string[] args)
        {
            bool validInput = false;
            do
            {
                Console.WriteLine("Please enter a valid number:");
                var input = Console.ReadLine();
                validInput = int.TryParse(input, out _start);
            } while (!validInput);

            ThreadPool.QueueUserWorkItem(Countdown, _start);
            semaphore.WaitOne();
            Console.WriteLine("Finish");
            Console.ReadKey();


        }


        private static void Countdown(object counter)
        {
            var c = (int)counter;
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}. Counter: {--c}");
            if (_start - c == 10)
            {
                semaphore.Release(1);
                return;
            }
            ThreadPool.QueueUserWorkItem(Countdown, c);
        }
    }
}
