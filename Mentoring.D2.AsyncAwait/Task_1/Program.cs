using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource source = null;
            while (true)
            {
                Console.WriteLine("Enter a number");
                BigInteger n = BigInteger.Parse(Console.ReadLine());
                if (source == null) source = new CancellationTokenSource();
                else
                {
                    source.Cancel();
                    source.Dispose();
                    source = new CancellationTokenSource();

                }
                GetSum(n, source.Token);

            }
        }

        private static async Task GetSum(BigInteger n, CancellationToken token)
        {
            await Task.Run(() =>
            {
                BigInteger res = 0;
                for (long i = 0; i <= n; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine($"{n} canceled");
                        return;
                    }
                    res += i;
                }
                Console.WriteLine($"Result for {n} is {res}");
            });
        }
    }
}

