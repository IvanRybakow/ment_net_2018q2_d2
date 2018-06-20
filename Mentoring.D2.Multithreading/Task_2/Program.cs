using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                var rnd = new Random();
                var result = Enumerable.Repeat(0, 10).Select(i => rnd.Next(0, 100)).ToArray();
                Console.WriteLine("10 Random Numbers: \n");
                PrintNums(result);
                return result;
            }).ContinueWith((t) =>
            {
                var rnd = new Random();
                var multiplier = rnd.Next(1, 10);
                Console.WriteLine($"Multiplier: {multiplier} \n");
                var result = t.Result.Select(i => i * multiplier).ToArray();
                Console.WriteLine("10 Random Numbers multiplied by Multiplier: \n");
                PrintNums(result);
                return result;
            }).ContinueWith((t) =>
            {
                var result = t.Result.OrderBy(i => i).ToArray();
                Console.WriteLine("10 Random numbers multiplied and Sorted: \n");
                PrintNums(result);
                return result;
            }).ContinueWith((t) =>
            {
                Console.WriteLine("Average of 10 Random multiplied numbers: \n");
                Console.WriteLine(t.Result.Average());
            });

            Console.ReadKey();

        }

        private static void PrintNums(IEnumerable<int> nums)
        {
            foreach (int item in nums)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
        }
    }
}
