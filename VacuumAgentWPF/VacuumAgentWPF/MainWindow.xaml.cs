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

            // Wait for both thread to join back main thread
            //agent.Join();
            //environment.Join();
        }

        public void UpdateRobotPosition(int x, int y)
        {
            if(RobotImage.Visibility == Visibility.Collapsed)
            {
                RobotImage.Visibility = Visibility.Visible;
            }
            Grid.SetRow(RobotImage, x);
            Grid.SetColumn(RobotImage, y);
        }

        private void OnClose(object sender, System.EventArgs e)
        {
            agent.Abort();
            environment.Abort();
        }
    }
}
