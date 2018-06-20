using System;

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            int rowsMatrixA = RequestNumber("number of rows for 1 matrix");
            int colsMatrixA = RequestNumber("number of columns for 1 matrix");
            int rowsMatrixB = colsMatrixA;
            int colsMatrixB = RequestNumber("number of columns for 2 matrix");

            MatrixMultiplier multiplier = new MatrixMultiplier();
            int[][] matrixA = MatrixUtils.CreateMatrix(rowsMatrixA, colsMatrixA, false);
            Console.WriteLine("First Matrix:");
            MatrixUtils.PrintMatrix(matrixA);
            int[][] matrixB = MatrixUtils.CreateMatrix(rowsMatrixB, colsMatrixB, false);
            Console.WriteLine("Second Matrix:");
            MatrixUtils.PrintMatrix(matrixB);
            try
            {
                int[][] result = multiplier.Multiply(matrixA, matrixB);
                Console.WriteLine("Matrix Product:");
                MatrixUtils.PrintMatrix(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        private static int RequestNumber(string name)
        {
            int result;
            bool invalidInput = true;
            do
            {
                Console.WriteLine($"Please enter a {name} in 1-9 range:");
                var input = Console.ReadLine();
                if(Int32.TryParse(input, out result) && result > 0 && result < 10) invalidInput = false;
            } while (invalidInput);

            return result;
        }
    }
}
