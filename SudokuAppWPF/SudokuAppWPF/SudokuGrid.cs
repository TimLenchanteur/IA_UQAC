using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SudokuAppWPF
{
    /// <summary>
    /// Classe representant l'environement dans lequel l'IA evolue
    /// </summary>
    class SudokuGrid
    {
        // Grille contenant les valeurs du sudoku
        int[,] m_grid;
        public int[,] Grid { get { return m_grid; } }

        // Pour afficher un print des fichiers .ss 
        char m_verticalSeparator = '!';
        char m_horizontalSeparator = '-';
        char m_emptyCell = '.';

        // Variable identifiant les cases vides
        int m_emptyGridCell = -1;

        // Communication avec la partie graphique de l'application
        MainWindow m_registeredMainWindow;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="registerWindow">Classe depuis laquelle l'objet a ete construit</param>
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

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="file">Fichier a charger</param>
        /// <param name="registerWindow">Classe depuis laquelle l'objet a ete construit</param>
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

        /// <summary>
        /// Lit le fichier fourni
        /// </summary>
        /// <param name="file">Adresse du fichier a lire</param>
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

        /// <summary>
        /// Resolu le sudoku actuellement charger
        /// </summary>
        public void Solve()
        {
            // Chronometre l'algorithme
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // Construction et appel de l'IA pouvant resoudre la sudoku
            SudokuSolver solver = new SudokuSolver(m_grid, (int)Math.Sqrt(m_grid.Length));
            int[,] res = solver.Solve();
            stopWatch.Stop();
            if (res != null)
            {
                m_grid = res;
                //PrintGrid(m_grid);
                DisplaySolution(m_grid);
                // Recupere le temps passe entre le dernier start et stop du chronometre
                TimeSpan ts = stopWatch.Elapsed;
                // Formated le temps recuperé.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                m_registeredMainWindow.UpdateResultText(false, true, elapsedTime);
            }
            else m_registeredMainWindow.UpdateResultText(false, false, "");
        }

        
        /// <summary>
        /// Affiche la grille dans l'application
        /// </summary>
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

        /// <summary>
        /// Affiche la solution du sudoku dans l'application
        /// </summary>
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

        /// <summary>
        /// Affiche la grille courante dans la console
        /// </summary>
        /// <param name="grid"></param>
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