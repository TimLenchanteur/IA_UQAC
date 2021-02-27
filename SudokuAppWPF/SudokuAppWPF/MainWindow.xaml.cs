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
    /// Interaction logic for MainWindow.xaml
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
            m_CurrentSudoku.PrintGrid();
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

        public void UpdateCase(int posX, int posY, int value) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(value);
            m_DisplayGrid[posX, posY].Visibility = Visibility.Visible;
            m_DisplayGrid[posX, posY].Text = stringBuilder.ToString();
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
            m_CurrentSudoku.DisplayGrid();
        }

        private void LoadSudokuClick(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sudoku files (*.ss)|*.ss";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (openFileDialog.ShowDialog() == true) {
                m_CurrentSudoku = new SudokuGrid(openFileDialog.FileName, this);
                ClearSudoku();
                m_CurrentSudoku.DisplayGrid();
            }
        }
    }
}
