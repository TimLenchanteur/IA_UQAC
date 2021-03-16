using System;
using System.Collections.Generic;
using System.Text;

namespace MagicWoodWPF
{
    /// <summary>
    /// Environement dans lequel evolue l'intelligence artificielle
    /// </summary>
    class MagicWood
    {
        // Modelisation des objets et agent present dans la forêt
        // La modelisation permet de verifier la presence de ces element a partir d'une operation sur les bits de l'entier
        public const int NONE = 0;
        public const int RIFT = 1;
        public const int WIND = 2;
        public const int MONSTER = 4;
        public const int SMELL = 8;

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

            // Peuple et affiche l'environement
            PopulateWood();
            _appDisplayer.DisplayWood(_woodGrid);
        }

        /// <summary>
        /// Peuple le bois de crevasse et monstre
        /// </summary>
        void PopulateWood() {
            throw new NotImplementedException();
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
