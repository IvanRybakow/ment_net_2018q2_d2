using System;
using System.Threading;

namespace Task_4
{
    class Program
    {
        private static int _start;
        static void Main(string[] args)
        {
            bool validInput = false;
            do
            {
                Console.WriteLine("Please enter a valid number:");
                var input = Console.ReadLine();
                validInput = int.TryParse(input, out _start);
            } while (!validInput);

            var t = new Thread(Countdown);
            t.Start(_start);
            t.Join();
            Console.WriteLine("Finish");
            Console.ReadKey();
        }


        private static void Countdown(object counter)
        {
            var c = (int)counter;
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}. Counter: {--c}");
            if (_start - c == 10)
            {
                return;
            }
            var t = new Thread(Countdown);
            t.Start(c);
            t.Join();
        }
    }
}
