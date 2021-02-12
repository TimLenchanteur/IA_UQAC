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
using System.Threading;

namespace VacuumAgentWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread agent;
        Thread environment;

        Image[,] _dustImages;
        Image[,] _jewelImages;

        public static MainWindow Instance;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            Console.WriteLine("Program started");
            this.Show();

            // Create two the two thread here but wait for start agent thread in environment thread
            this.agent = new Thread(new ThreadStart(VacuumAgent.VacuumProc));
            this.environment = new Thread(Environment.EnvironmentProc);
            this.Closed += OnClose;

            environment.Start();
            agent.Start();

        }

        public void UpdateRobotPosition(int x, int y)
        {
            if(RobotImage.Visibility == Visibility.Collapsed)
            {
                RobotImage.Visibility = Visibility.Visible;
            }
            Grid.SetRow(RobotImage, x);
            Grid.SetColumn(RobotImage, y);
            Grid.SetZIndex(RobotImage, 100);
        }

        public void UpdateEnvironment()
        {
            for (int x = 0; x < Environment._gridDim.X; x++)
            {
                for (int y = 0; y < Environment._gridDim.Y; y++)
                {
                    _dustImages[x, y].Visibility = Visibility.Collapsed;
                    _jewelImages[x, y].Visibility = Visibility.Collapsed;
                    if(Environment._grid[x, y] == Environment.DIRT)
                    {
                        _dustImages[x, y].Visibility = Visibility.Visible;
                    }
                    else if (Environment._grid[x, y] == Environment.JEWEL)
                    {
                        _jewelImages[x, y].Visibility = Visibility.Visible;
                    }
                    else if (Environment._grid[x, y] == 3)
                    {
                        _dustImages[x, y].Visibility = Visibility.Visible;
                        _jewelImages[x, y].Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public void InitializeEnvironmentImage()
        {
            _dustImages = new Image[Environment._gridDim.X, Environment._gridDim.Y];
            _jewelImages = new Image[Environment._gridDim.X, Environment._gridDim.Y];
            for (int x = 0; x < Environment._gridDim.X; x++)
            {
                for (int y = 0; y < Environment._gridDim.Y; y++)
                {
                    // Dust
                    Image dust = CreateImage("dust.png");
                    GridRoot.Children.Add(dust);
                    _dustImages[x, y] = dust;
                    Grid.SetRow(dust, x);
                    Grid.SetColumn(dust, y);

                    // Jewel
                    Image jewel = CreateImage("jewel.png");
                    GridRoot.Children.Add(jewel);
                    _jewelImages[x, y] = jewel;
                    Grid.SetRow(jewel, x);
                    Grid.SetColumn(jewel, y);
                }
            }
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

        private void OnClose(object sender, System.EventArgs e)
        {
            agent.Abort();
            environment.Abort();
        }
    }
}
