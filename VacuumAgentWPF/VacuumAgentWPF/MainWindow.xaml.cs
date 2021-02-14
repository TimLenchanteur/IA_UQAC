using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using System.Text.RegularExpressions;

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
            if (RobotImage.Visibility == Visibility.Collapsed)
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
                    GridEnvironment.Children.Add(dust);
                    _dustImages[x, y] = dust;
                    Grid.SetRow(dust, x);
                    Grid.SetColumn(dust, y);

                    // Jewel
                    Image jewel = CreateImage("jewel.png");
                    GridEnvironment.Children.Add(jewel);
                    _jewelImages[x, y] = jewel;
                    Grid.SetRow(jewel, x);
                    Grid.SetColumn(jewel, y);
                }
            }
        }

        public void AddLearnedAction(int nbActions, float performance)
        {
            TextBlock newTextBlock = new TextBlock();
            newTextBlock.Text = "Actions : " + nbActions + ", Performance : " + performance;
            LearningPanel.Children.Add(newTextBlock);
        }

        public void ReserLearningStack()
        {
            LearningPanel.Children.Clear();
            TextBlock header = new TextBlock();
            header.Text = "Actions Evalués:";
            header.FontWeight = FontWeights.Bold;
            LearningPanel.Children.Add(header);
        }

        public void UpdateComputingState(String state) {
            PrintComputing.Text = state;
            Console.WriteLine(state);
        }

        public void UpdateOptimalActions()
        {
            OptimalActions.Text = "Nombres optimal d'actions a programmées " + VacuumAgent._optimalActionCycle;
            ReserLearningStack();
        }

        public void UpdateActionCycle() {
            ActionCycle.Text = "Nombre d'actions programmées " + VacuumAgent._actionCycle;
        }

        public void DisplayClean()
        {
            Grid.SetRow(CleanImage, VacuumAgent._pos.X);
            Grid.SetColumn(CleanImage, VacuumAgent._pos.Y);
            Grid.SetZIndex(CleanImage, 101);
            CleanImage.Visibility = Visibility.Visible;
            DelayRemoveImage(2000, CleanImage);
        }

        public void DisplayGrab()
        {
            Grid.SetRow(GrabImage, VacuumAgent._pos.X);
            Grid.SetColumn(GrabImage, VacuumAgent._pos.Y);
            Grid.SetZIndex(GrabImage, 101);
            GrabImage.Visibility = Visibility.Visible;
            DelayRemoveImage(2000, GrabImage);
        }

        public void DelayRemoveImage(int milliseconds, Image image)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate
            {
                image.Visibility = Visibility.Collapsed;
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            timer.Start();
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

        public void UpdateAlgo(String algorithm) {
            Algorithme.Header = "Algorithme : " + algorithm;
        }

        private void ToBFSAlgo(object sender, RoutedEventArgs e)
        {
            VacuumAgent.ChangeExplorationAlgo(VacuumAgent.Algorithm.BFS);
        }

        private void ToAStarAlgo(object sender, RoutedEventArgs e)
        {
            VacuumAgent.ChangeExplorationAlgo(VacuumAgent.Algorithm.ASTAR);
        }


        private void ThreeLearningCycle(object sender, RoutedEventArgs e)
        {
            LearningRound.Header = "Cycle d'apprentissage : 3";
            VacuumAgent.ChangeLearningCycle(3);
        }

        private void FiveLearningCycle(object sender, RoutedEventArgs e)
        {
            LearningRound.Header = "Cycle d'apprentissage : 5";
            VacuumAgent.ChangeLearningCycle(5);
        }

        private void TenLearningCycle(object sender, RoutedEventArgs e)
        {
            LearningRound.Header = "Cycle d'apprentissage : 10";
            VacuumAgent.ChangeLearningCycle(10);
        }

        private void FifteenLearningCycle(object sender, RoutedEventArgs e)
        {
            LearningRound.Header = "Cycle d'apprentissage : 15";
            VacuumAgent.ChangeLearningCycle(15);
        }

    }
}
