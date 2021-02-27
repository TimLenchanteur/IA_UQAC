﻿using System;
using System.Collections.Generic;
using System.Text;

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

        public void DisplayGrid() {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (m_grid[i, j] != m_emptyGridCell) {
                        m_registeredMainWindow.UpdateCase(i, j, m_grid[i,j]);
                    }
                }
            }
                  
        }

        public void PrintGrid()
        {
            string result = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (m_grid[i, j] == m_emptyGridCell)
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
            Console.WriteLine(result);
        }

        public void Solve()
        {
            m_grid = RecursiveBackTracking(new SudokuCSP(m_grid));
            PrintGrid();
        }

        int[,] RecursiveBackTracking(SudokuCSP csp)
        {
            bool isComplete = true;
            foreach (int i in csp.Grid)
            {
                if (i == -1)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                return csp.Grid;
            }
            Tuple<int, int> toAssign = csp.MinimumRemainingValue();
            if(csp.Domains[toAssign.Item1, toAssign.Item2].Count == 0)
            {
                return null;
            }
            foreach (int value in csp.Domains[toAssign.Item1, toAssign.Item2])
            {
                csp.SetValue(toAssign.Item1, toAssign.Item2, value);
                int[,] result = RecursiveBackTracking(csp);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}