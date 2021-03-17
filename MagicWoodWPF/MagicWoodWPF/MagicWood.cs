using System;
using System.Collections.Generic;
using System.Text;

namespace MagicWoodWPF
{
    /// <summary>
    /// Environnement dans lequel évolue l'intelligence artificielle
    /// </summary>
    class MagicWood
    {
        // Modelisation des objets et agent présents dans la forêt
        // La modélisation permet de verifier la présence de ces élements à partir d'une operation sur les bits de l'entier
        public const int NONE = 0;
        public const int CREVASSE = 1;
        public const int WIND = 2;
        public const int MONSTER = 4;
        public const int SMELL = 8;
        public const int PORTAL = 10;

        // Taille et grille modelisant le bois
        int _sqrtSize;
        public int SqrtSize { 
            get => _sqrtSize;
        }
        int[,] _woodGrid;
        public int[,] Grid {
            get => _woodGrid;
        }

        // Classe permetant d'afficher l'environement dans l'application
        MainWindow _appDisplayer;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sqrtSize">Taille d'une ligne ou d'une colonne</param>
        public MagicWood(MainWindow appDisplayer, int sqrtSize) {
            // Creer attributs de la classe
            _sqrtSize = sqrtSize;
            _woodGrid = new int[sqrtSize, sqrtSize];
            _appDisplayer = appDisplayer;

            // Peuple l'environnement
            PopulateWood();
            _appDisplayer.DisplayWood(_woodGrid);
        }

        /// <summary>
        /// Peuple le bois de crevasse et monstre
        /// </summary>
        void PopulateWood()
        {
            // Juste pour le test, à modifier
            _woodGrid[2, 0] = MONSTER;
            _woodGrid[1, 0] = SMELL;
            _woodGrid[2, 1] = SMELL;
            _woodGrid[2, 2] = WIND;
            _woodGrid[1, 1] = WIND;
            _woodGrid[0, 2] = WIND;
            _woodGrid[1, 2] = CREVASSE;
            _woodGrid[0, 1] = PORTAL;
        }

        /// <summary>
        /// Place l'agent sur une case qui n'a pas de crevasse
        /// </summary>
        /// <returns>La position de depart de l'agent</returns>
        public Vector2 PlaceAgent() {
            throw new NotImplementedException();
        }
    }
}
