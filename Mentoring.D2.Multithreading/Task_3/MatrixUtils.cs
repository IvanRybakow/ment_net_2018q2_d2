using System;

namespace Task_3
{
    class MatrixUtils
    {
        public static void PrintMatrix(int[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[0].Length; j++)
                {
                    Console.Write($"   {matrix[i][j]}");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public static int[][] CreateMatrix(int rows, int cols, bool isEmpty)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int[][] result = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new int[cols];
                if (!isEmpty)
                {
                    for (int j = 0; j < result[i].Length; j++)
                    {
                        result[i][j] = rnd.Next(0, 9);
                    }
                }
            }

            return result;
        }
    }
}
