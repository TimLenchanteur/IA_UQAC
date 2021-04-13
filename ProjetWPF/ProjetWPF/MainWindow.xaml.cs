using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;

namespace ProjetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board m_board = new Board();
        Token m_selectedToken;

        public MainWindow()
        {
            InitializeComponent();
            FillBoardColor();
            m_board.Initialize();
            DisplayBoard();
        }

        private void FillBoardColor()
        {
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0)
                    {
                        SetRectangle(i, j, Colors.Khaki);
                    }
                }
            }
        }

        private void SetRectangle(int x, int y, Color color)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(color);
            GridBoard.Children.Add(rectangle);
            Grid.SetColumn(rectangle, x);
            Grid.SetRow(rectangle, y);
        }

        private Image CreateImage(string imageName)
        {
            Image image = new Image();
            image.Stretch = Stretch.Fill;
            image.Visibility = Visibility.Collapsed;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.Source = new BitmapImage(new Uri("Images/" + imageName, UriKind.Relative));

            return image;
        }

        private void DisplayBoard()
        {
            ClearImages();
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(m_board.Tokens[i,j] != null)
                    {
                        Image token = CreateImage(m_board.Tokens[i,j].image);
                        token.Visibility = Visibility.Visible;
                        GridBoard.Children.Add(token);
                        Grid.SetRow(token, i);
                        Grid.SetColumn(token, j);
                    }
                }
            }
        }
        private void ClearImages()
        {
            UIElement[] collection = new UIElement[GridBoard.Children.Count];
            GridBoard.Children.CopyTo(collection, 0);
            foreach (UIElement child in collection)
            {
                if(!(child is Rectangle))
                    GridBoard.Children.Remove(child);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var element = Mouse.DirectlyOver;
            if(element != null)
            {
                // If a token was already selected, move it if possible
                if(m_selectedToken != null)
                {
                    List<Vector2> possibleActions = new List<Vector2>();
                    possibleActions.AddRange(m_board.PossibleMoves(m_selectedToken));
                    possibleActions.AddRange(m_board.PossibleCaptures(m_selectedToken));
                    Vector2 selectedCell = new Vector2(Grid.GetColumn((UIElement)element), Grid.GetRow((UIElement)element));
                    if(possibleActions.Contains(selectedCell))
                    {
                        m_board.MoveToken(m_selectedToken, selectedCell);
                        ChangeColorOfCells(possibleActions, Colors.Khaki);
                        m_selectedToken = null;
                        DisplayBoard();
                    }
                }
                else
                {
                    m_selectedToken = m_board.Tokens[Grid.GetRow((UIElement)element), Grid.GetColumn((UIElement)element)];
                    ChangeColorOfCells(m_board.PossibleMoves(m_selectedToken), Colors.CadetBlue);
                    ChangeColorOfCells(m_board.PossibleCaptures(m_selectedToken), Colors.CadetBlue);
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            // Reset selected token
            List<Vector2> possibleActions = new List<Vector2>();
            possibleActions.AddRange(m_board.PossibleMoves(m_selectedToken));
            possibleActions.AddRange(m_board.PossibleCaptures(m_selectedToken));
            ChangeColorOfCells(possibleActions, Colors.Khaki);
            m_selectedToken = null;
        }

        private void ChangeColorOfCells(List<Vector2> cells, Color color)
        {
            foreach(Vector2 cell in cells)
            {
                var element = GridBoard.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == cell.Y && Grid.GetColumn(e) == cell.X);
                if(element is Rectangle)
                {
                    Rectangle rectangle = element as Rectangle;
                    rectangle.Fill = new SolidColorBrush(color);
                }
            }
        }
    }
}
