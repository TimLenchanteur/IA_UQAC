using System;

namespace SudokuApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SudokuGrid grid = new SudokuGrid(@"Grids/grid1.ss");
            grid.PrintGrid();
        }
    }
}
