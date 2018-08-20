using System;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


namespace Mentoring.D2.Profiling.Task_1
{
    public class PasswordHashBenchmark
    {
        private byte[] salt;
        private string passwordText;
        private int iterations;

        [GlobalSetup]
        public void Setup()
        {
            salt = Encoding.Unicode.GetBytes("5a4ccc325aa664d61d8326deb882cf93");
            passwordText = "password";
            iterations = 10000;
        }

        [Benchmark]
        public string GeneratePasswordHashUsingSaltOptimized()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterations);
            Span<byte> byteArr = stackalloc byte[36];
            byteArr.AddRange(salt, 0, 16);
            byteArr.AddRange(pbkdf2.GetBytes(20), 16, 36);
            var passwordHash = Convert.ToBase64String(byteArr);
            return passwordHash;
        }

        [Benchmark]
        public string GeneratePasswordHashUsingSalt()
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }


    }
    public static class Extension
    {
        public static void AddRange<T>(this Span<T> destination, T[] input, int start, int finish)
        {
            for (int i = start, j = 0; i < finish; i++, j++)
            {
                destination[i] = input[j];
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PasswordHashBenchmark>();
            Console.ReadKey();
        }
    }
}
