using System;
using System.Linq;
using System.Threading.Tasks;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task[] taskArray = Enumerable.Range(0, 100).Select(i => Task.Factory.StartNew(() =>
            {
                for (int j = 1; j <= 1000; j++)
                {
                    Console.WriteLine($"Task #{i} – {j}");
                }
            })).ToArray();

            Task.WaitAll(taskArray);
            Console.WriteLine("Finished");
        }
    }
}
