using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SudokuAppWPF
{
    class SudokuGrid
    {
        // Grid containing the value of sudoku
        int[,] m_grid;
        public int[,] Grid { get { return m_grid; } }

        // For display and .ss files
        char m_verticalSeparator = '!';
        char m_horizontalSeparator = '-';
        char m_emptyCell = '.';

        int m_emptyGridCell = -1;


        MainWindow m_registeredMainWindow;

        public SudokuGrid(MainWindow registerWindow)
        {

            m_registeredMainWindow = registerWindow;
            m_grid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    m_grid[i, j] = m_emptyGridCell;
                }
            }
        }

        public SudokuGrid(string file, MainWindow registerWindow)
        {
            m_registeredMainWindow = registerWindow;
            m_grid = new int[9, 9];
            ReadFile(file);
        }

        public SudokuGrid(SudokuGrid sudokuGrid)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    m_grid[i, j] = sudokuGrid.m_grid[i, j];
                }
            }
        }

        void ReadFile(string file)
        {
            string ending = file.Substring(file.Length - 3);
            if (!ending.Equals(".ss"))
            {
                Console.WriteLine("Error: File needs to be .ss");
                return;
            }

            string[] lines = System.IO.File.ReadAllLines(file);
            int l = 0;
            int c = 0;
            foreach (string line in lines)
            {
                if (line[0] == m_horizontalSeparator)
                {
                    continue;
                }

                foreach (char character in line)
                {
                    if (character != m_verticalSeparator)
                    {
                        if (character == m_emptyCell)
                        {
                            m_grid[l, c] = m_emptyGridCell;
                        }
                        else
                        {
                            m_grid[l, c] = (int)Char.GetNumericValue(character);
                        }
                        c++;
                    }
                }
                c = 0;
                l++;
            }
        }

        public void Solve()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            m_grid = RecursiveBackTracking(new SudokuCSP(m_grid), m_grid);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.WriteLine("RunTime " + elapsedTime);
            if (m_grid != null) {
                PrintGrid(m_grid);
                DisplaySolution(m_grid);
            }
            else Debug.WriteLine("Couldn't solve sudoku");
        }

        int[,] RecursiveBackTracking(SudokuCSP csp, int[,] assignement)
        {
            if (IsComplete(csp)) return assignement;

            //List<SudokuCSP.CSPVariable> changedVariables = csp.ACThree();

            SudokuCSP.CSPVariable toAssign = SelectUnassignedVariable(csp);
            if (toAssign == null) {
                //csp.ResetDomains(changedVariables);
                return null;
            }


            int[] neighbourDomains = toAssign.NeighorsDomains();
            List<int> nodeDomain = new List<int>(toAssign.NodeDomain);

            while (nodeDomain.Count != 0)
            {
                int value = csp.LeastConstrainingValue(nodeDomain, neighbourDomains);
                csp.SetValue(toAssign, value);
                assignement[toAssign.Position.Item1, toAssign.Position.Item2] = value;
                nodeDomain.Remove(value);
                int[,] result = RecursiveBackTracking(csp, assignement);
                if (result != null) {
                    return result;
                } 
                // Reset with old value
                csp.ResetValue(toAssign);
            }

            return null;
        }

        private bool IsComplete(SudokuCSP csp)
        {
            return csp.RemainingVariable == 0;
        }

        private SudokuCSP.CSPVariable SelectUnassignedVariable(SudokuCSP csp)
        {
            var mrvValues = csp.MinimumRemainingValues();
            SudokuCSP.CSPVariable toAssign = csp.DegreeHeuristic(mrvValues)[0];

            if (toAssign.DomainSize == 0) return null;
            return toAssign;
        }

        public void DisplayGrid()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (m_grid[i, j] != m_emptyGridCell)
                    {
                        m_registeredMainWindow.UpdateCase(i, j, m_grid[i, j]);
                    }
                }
            }

        }

        public void DisplaySolution(int[,] solution)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    m_registeredMainWindow.UpdateCaseSolution(i, j, solution[i, j]);
                }
            }

        }

        public void PrintGrid(int[,] grid)
        {
            string result = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] == m_emptyGridCell)
                    {
                        result += m_emptyCell;
                    }
                    else
                    {
                        result += m_grid[i, j].ToString();
                    }

                    // Vertical separator
                    if (j == 2 || j == 5)
                    {
                        result += m_verticalSeparator;
                    }
                }
                result += "\n";

                // Horizontal separator
                if (i == 2 || i == 5)
                {
                    for (int k = 0; k < 11; k++)
                    {
                        result += m_horizontalSeparator;
                    }
                    result += "\n";
                }
            }
            System.Diagnostics.Debug.WriteLine(result);
        }
    }
}
