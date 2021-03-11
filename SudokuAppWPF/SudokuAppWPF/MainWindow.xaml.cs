using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SudokuAppWPF
{
    /// <summary>
    /// Class utilise pour realiser la logique de la partie graphique de l'application
    /// </summary>
    public partial class MainWindow : Window
    {

        TextBox[,] m_DisplayGrid;
        SudokuGrid m_CurrentSudoku;

        public MainWindow()
        {
            InitializeComponent();
            InitGrid();

            Show();

            m_CurrentSudoku = new SudokuGrid(@"Grids/grid1.ss", this);
            m_CurrentSudoku.DisplayGrid();
        }

        void InitGrid() {
            m_DisplayGrid = new TextBox[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    TextBox displayValue = new TextBox();
                    displayValue.FontSize = 30.0;
                    displayValue.TextAlignment = TextAlignment.Center;
                    displayValue.Visibility = Visibility.Collapsed;
                    displayValue.BorderBrush = new SolidColorBrush(new Color());
                    displayValue.BorderThickness = new Thickness(2);
                    if (x > 5) {
                        if (y > 5)
                        {
                            GridCase22.Children.Add(displayValue);
                        }
                        else if (y > 2)
                        {
                            GridCase12.Children.Add(displayValue);
                        }
                        else
                        {
                            GridCase02.Children.Add(displayValue);
                        }
                    }
                    else if (x > 2){
                        if (y > 5)
                        {
                            GridCase21.Children.Add(displayValue);
                        }
                        else if (y > 2)
                        {
                            GridCase11.Children.Add(displayValue);
                        }
                        else
                        {
                            GridCase01.Children.Add(displayValue);
                        }
                    }
                    else {
                        if (y > 5)
                        {
                            GridCase20.Children.Add(displayValue);
                        }
                        else if (y > 2)
                        {
                            GridCase10.Children.Add(displayValue);
                        }
                        else
                        {
                            GridCase00.Children.Add(displayValue);
                        }
                    }
                    
                    Grid.SetRow(displayValue, x%3);
                    Grid.SetColumn(displayValue, y%3);
                    m_DisplayGrid[x, y] = displayValue;
                }
            }
        }

        public void UpdateResultText(bool reset, bool couldSolve, string elapsedTime) {
            if (reset) ResultText.Text = "";
            else if (couldSolve) ResultText.Text = "Résolu en " + elapsedTime;
            else ResultText.Text = "Impossible de résoudre le sudoku";
        }

        public void UpdateCase(int posX, int posY, int value) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(value);
            m_DisplayGrid[posX, posY].Visibility = Visibility.Visible;
            m_DisplayGrid[posX, posY].Foreground = Brushes.Black;
            m_DisplayGrid[posX, posY].Text = stringBuilder.ToString();
        }

        public void UpdateCaseSolution(int posX, int posY, int value)
        {
            if (m_DisplayGrid[posX, posY].Visibility != Visibility.Visible) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(value);
                m_DisplayGrid[posX, posY].Visibility = Visibility.Visible;
                m_DisplayGrid[posX, posY].Foreground = Brushes.Blue;
                m_DisplayGrid[posX, posY].Text = stringBuilder.ToString();
            }
        }

        public void ClearSudoku() {
            for (int x = 0; x < 9; x++){
                for (int y = 0; y < 9; y++){
                    m_DisplayGrid[x, y].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SolveCurrent(object sender, RoutedEventArgs e)
        {
            m_CurrentSudoku.Solve();
        }

        /// <summary>
        /// Charge un nouveau sudoku
        /// </summary>
        /// <param name="sender">Le button ayant appele cette fonction</param>
        /// <param name="e">Parametre supplementaire si necessaire</param>
        private void LoadSudokuClick(object sender, RoutedEventArgs e)
        {
            UpdateResultText(true, false, "");
            // Ouvre une fenetre de dialogue permetant d'allez chercher le sudoku
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sudoku files (*.ss)|*.ss";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (openFileDialog.ShowDialog() == true) {
                // Affichage et sauvegarde temporaire du sudoku chargé
                m_CurrentSudoku = new SudokuGrid(openFileDialog.FileName, this);
                ClearSudoku();
                m_CurrentSudoku.DisplayGrid();
            }
        }
    }
}
