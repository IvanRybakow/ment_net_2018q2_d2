using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Cell
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Age { get; set; }
        public int NextAge { get; set; }

        public bool IsAlive { get; set; }
        public bool NextIsAlive { get; set; }
        public int NeighboursCount { get; set; }
        public int NextNeighboursCount { get; set; }


        public Cell(int row, int column, int age, bool alive)
        {
            PositionX = row * 5;
            PositionY = column * 5;
            Age = age;
            IsAlive = alive;
            NextAge = 0;
            NextIsAlive = false;

        }
    }
}