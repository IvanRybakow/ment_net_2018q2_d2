using System;
using System.Linq;
using System.Threading.Tasks;

namespace Task_3
{
    public class MatrixMultiplier
    {
        public int[][] Multiply(int[][] matrixA, int[][] matrixB)
        {
            int rowsA = matrixA.Length;
            int rowsB = matrixB.Length;
            int colsA = matrixA[0].Length;
            int colsB = matrixB[0].Length;
            if (colsA != rowsB)
            {
                throw new ArgumentException("Theese matrices can not be multiplied");
            }
            int[][] result = MatrixUtils.CreateMatrix(rowsA, colsB, true);
            Parallel.For(0, rowsA, i =>
            {
                Parallel.For(0, colsB, j =>
                {
                    int[] col = matrixB.Select(row => row[j]).ToArray();
                    result[i][j] = VectorProduct(matrixA[i], col);
                });
            });
            return result;
        }


        private int VectorProduct(int[] row, int[] col)
        {
            int result = 0;
            for (int i = 0; i < row.Length; i++)
            {
                result += row[i] * col[i];
            }

            return result;
        }
    }
    
}
