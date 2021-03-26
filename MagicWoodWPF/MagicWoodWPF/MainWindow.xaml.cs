using System;
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

namespace MagicWoodWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Environment et agent genéré pour le niveau actuel
        MagicWood _currentWood;
        WoodTravelerAgent _currentAgent;

        public MainWindow()
        {
            InitializeComponent();

            // Genere le bois initial
            GenerateAppGrid(4);
            GenerateWood(4);
        }

        /// <summary>
        /// Genere un nouveau bois et l'agent qui va essayer de le resoudre 
        /// </summary>
        /// <param name="sqrtSize"> Taille des lignes et colonne du bois genere</param>
        void GenerateWood(int sqrtSize)
        {
            _currentWood = new MagicWood(this, sqrtSize);
            _currentAgent = new WoodTravelerAgent(this, _currentWood);
        }

        /// <summary>
        /// Genere la grille correspondante au bois dans l'application
        /// </summary>
        /// <param name="sqrtSize"></param>
        public void GenerateAppGrid(int sqrtSize)
        {
            ClearImages();
            GridEnvironment.ColumnDefinitions.Clear();
            GridEnvironment.RowDefinitions.Clear();
            for (int i = 0; i < sqrtSize; i++)
            {
                GridEnvironment.ColumnDefinitions.Add(new ColumnDefinition());
                GridEnvironment.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Affiche la carte du bois sur l'application (Emplacement que l'agent ne connais pas grisé ?) 
        /// </summary>
        /// <param name="grid"></param>
        public void DisplayWood(int[,] grid)
        { 
            for(int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Monster
                    if((grid[i,j] & MagicWood.MONSTER) == MagicWood.MONSTER)
                    {
                        Image monster = CreateImage("monster.png");
                        monster.Visibility = Visibility.Visible;
                        GridEnvironment.Children.Add(monster);
                        Grid.SetRow(monster, i);
                        Grid.SetColumn(monster, j);
                    }
                    // Smell
                    if ((grid[i, j] & MagicWood.SMELL) == MagicWood.SMELL)
                    {
                        Image poopangel = CreateImage("poopangel.png");
                        poopangel.Visibility = Visibility.Visible;
                        GridEnvironment.Children.Add(poopangel);
                        Grid.SetRow(poopangel, i);
                        Grid.SetColumn(poopangel, j);
                    }
                    // Crevasse
                    if ((grid[i, j] & MagicWood.CREVASSE) == MagicWood.CREVASSE)
                    {
                        Image crevasse = CreateImage("crevasse.png");
                        crevasse.Visibility = Visibility.Visible;
                        GridEnvironment.Children.Add(crevasse);
                        Grid.SetRow(crevasse, i);
                        Grid.SetColumn(crevasse, j);
                    }
                    // Wind
                    if ((grid[i, j] & MagicWood.WIND) == MagicWood.WIND)
                    {
                        Image cloud = CreateImage("cloud.png");
                        cloud.Visibility = Visibility.Visible;
                        GridEnvironment.Children.Add(cloud);
                        Grid.SetRow(cloud, i);
                        Grid.SetColumn(cloud, j);
                    }
                    // Portal
                    if ((grid[i, j] & MagicWood.PORTAL) == MagicWood.PORTAL)
                    {
                        Image portal = CreateImage("portal.png");
                        portal.Visibility = Visibility.Visible;
                        GridEnvironment.Children.Add(portal);
                        Grid.SetRow(portal, i);
                        Grid.SetColumn(portal, j);
                    }
                }
            }
        }

        /// <summary>
        /// Deplace l'agent dans l'application (Degrise les case dont il prend connaissance ?)
        /// </summary>
        /// <param name="newPosition">Nouvelle position de l'agent</param>
        public void UpdateAgentPosition(Vector2 newPosition)
        {
            if (AgentImage.Visibility == Visibility.Collapsed)
            {
                AgentImage.Visibility = Visibility.Visible;
            }

            Grid.SetRow(AgentImage, newPosition.X);
            Grid.SetColumn(AgentImage, newPosition.Y);
            Grid.SetZIndex(AgentImage, 100);
        }

        /// <summary>
        /// Envoie une instrcution pour specifier a l'agent de se deplacer
        /// </summary>
        /// <param name="sender">Le button ayant appele cette fonction</param>
        /// <param name="e">Parametre supplementaire si necessaire</param>
        private void MoveAgent(object sender, RoutedEventArgs e)
        {
            _currentAgent.ExecuteMove();
        }

        /// Create an image from a string
        private Image CreateImage(string imageName)
        {
            Image image = new Image();
            image.Stretch = Stretch.Fill;
            image.Visibility = Visibility.Collapsed;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.Source = new BitmapImage(new Uri("Images/" + imageName, UriKind.Relative));

            return image;
        }

        private void ClearImages()
        {
            UIElement[] collection = new UIElement[GridEnvironment.Children.Count];
            GridEnvironment.Children.CopyTo(collection, 0);
            foreach (UIElement child in collection)
            {
                if(child != AgentImage)
                    GridEnvironment.Children.Remove(child);
            }
        }
    }
}
