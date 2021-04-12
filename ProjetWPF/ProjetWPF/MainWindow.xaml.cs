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
        IInputElement m_selectedItem;

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
                    else
                    {
                        SetRectangle(i, j, Colors.White);
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
                m_selectedItem = element;

                Token token = m_board.Tokens[Grid.GetRow((UIElement)m_selectedItem), Grid.GetColumn((UIElement)m_selectedItem)];
                if(token != null)
                {
                    Debug.WriteLine("Token position = " + token.Position);
                    foreach (Vector2 move in m_board.PossibleMoves(token))
                    {
                        Debug.WriteLine("Move = " + move);
                    }
                }
            }
        }
    }
}
