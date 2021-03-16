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
            int sqrtSize = 3;
            GenerateWood(sqrtSize);
        }

        /// <summary>
        /// Genere un nouveau bois et l'agent qui va essayer de le resoudre 
        /// </summary>
        /// <param name="sqrtSize"> Taille des lignes et colonne du bois genere</param>
        void GenerateWood(int sqrtSize)
        {
            _currentWood = new MagicWood(this, sqrtSize);
        }

        /// <summary>
        /// Genere la grille correspondante au bois dans l'application
        /// </summary>
        /// <param name="sqrtSize"></param>
        void GenerateAppGrid(int sqrtSize) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Affiche la carte du bois sur l'application (Emplacement que l'agent ne connais pas grisé ?) 
        /// </summary>
        /// <param name="grid"></param>
        public void DisplayWood(int[,] grid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deplace l'agent dans l'application (Degrise les case dont il prend connaissance ?)
        /// </summary>
        /// <param name="newPosition">Nouvelle position de l'agent</param>
        public void UpdateAgentPosition(Vector2 newPosition)
        {
            throw new NotImplementedException();
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
    }
}
