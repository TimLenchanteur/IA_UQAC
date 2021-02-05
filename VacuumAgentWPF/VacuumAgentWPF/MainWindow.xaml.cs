﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Hello World!");

            // Create two the two thread here but wait for start agent thread in environment thread
            Thread agent = new Thread(new ThreadStart(VacuumAgent.VaccumProc));
            Thread environment = new Thread(Environment.EnvironmentProc);

            environment.Start();
            agent.Start();

            // Wait for both thread to join back main thread
            agent.Join();
            environment.Join();
        }
    }
}
