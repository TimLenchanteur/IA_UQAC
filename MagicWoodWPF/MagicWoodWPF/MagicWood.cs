﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public const int PORTAL = 16;

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
            DisplayWood();
        }

        /// <summary>
        /// Peuple le bois de crevasse et monstre
        /// </summary>
        void PopulateWood()
        {
            // Choisis une case aléatoire pour le portail
            var rand = new Random();
            _woodGrid[rand.Next(0, _sqrtSize), rand.Next(0, _sqrtSize)] = PORTAL;

            for(int i = 0; i < _sqrtSize; i++)
            {
                for (int j = 0; j < _sqrtSize; j++)
                {
                    // Ne rien ajouter sur la case portal
                    if (_woodGrid[i, j] == PORTAL) continue;

                    // 1 chance sur 10 d'avoir un monstre
                    if(rand.Next(0, 10) == 0) 
                    {
                        _woodGrid[i, j] += MONSTER;
                        AddValueAdjacent(i, j, SMELL);
                    }
                    // 1 chance sur 10 d'avoir une crevasse
                    else if (rand.Next(0, 10) == 0)
                    {
                        _woodGrid[i, j] += CREVASSE;
                        AddValueAdjacent(i, j, WIND);
                    }
                }
            }
        }

        /// <summary>
        /// Place l'agent sur une case qui n'a pas de crevasse
        /// </summary>
        /// <returns>La position de depart de l'agent</returns>
        public Vector2 PlaceAgent() {
            throw new NotImplementedException();
        }

        /// Ajoute une valeur sur les cases adjacentes, pour les monstres (smell) ou les crevasses (wind)
        private void AddValueAdjacent(int x, int y, int value)
        {
            foreach(Vector2 vector2 in AdjacentCells(x, y))
            {
                // La valeur n'a pas déjà été ajoutée
                if((_woodGrid[vector2.X, vector2.Y] & value) != value)
                {
                    _woodGrid[vector2.X, vector2.Y] += value;
                }
            }
        }

        private List<Vector2> AdjacentCells(int x, int y)
        {
            List<Vector2> cells = new List<Vector2>();
            // Haut gauche
            if (x == 0 && y == 0)
            {
                cells.Add(new Vector2(x + 1, y));
                cells.Add(new Vector2(x, y + 1));
            }
            // Haut droit
            else if (x == _sqrtSize - 1 && y == 0)
            {
                cells.Add(new Vector2(x - 1, y));
                cells.Add(new Vector2(x, y + 1));
            }
            // Bas gauche
            else if (x == 0 && y == _sqrtSize - 1)
            {
                cells.Add(new Vector2(x, y - 1));
                cells.Add(new Vector2(x + 1, y));
            }
            // Bas droit
            else if (x == _sqrtSize - 1 && y == _sqrtSize - 1)
            {
                cells.Add(new Vector2(x, y - 1));
                cells.Add(new Vector2(x - 1, y));
            }
            // Côté gauche
            else if (x == 0)
            {
                cells.Add(new Vector2(x, y - 1));
                cells.Add(new Vector2(x, y + 1));
                cells.Add(new Vector2(x + 1, y));
            }
            // Côté droit
            else if (x == _sqrtSize - 1)
            {
                cells.Add(new Vector2(x, y - 1));
                cells.Add(new Vector2(x, y + 1));
                cells.Add(new Vector2(x - 1, y));
            }
            // Côté haut
            else if (y == 0)
            {
                cells.Add(new Vector2(x - 1, y));
                cells.Add(new Vector2(x + 1, y));
                cells.Add(new Vector2(x, y + 1));
            }
            // Côté bas
            else if (y == _sqrtSize - 1)
            {
                cells.Add(new Vector2(x - 1, y));
                cells.Add(new Vector2(x + 1, y));
                cells.Add(new Vector2(x, y - 1));
            }
            // N'importe quelle case à l'intérieur de la grille
            else
            {
                cells.Add(new Vector2(x - 1, y));
                cells.Add(new Vector2(x + 1, y));
                cells.Add(new Vector2(x, y - 1));
                cells.Add(new Vector2(x, y + 1));
            }

            return cells;
        }

        public void DisplayWood()
        {
            string result = "";
            for (int i = 0; i < _sqrtSize; i++)
            {
                for (int j = 0; j < _sqrtSize; j++)
                {
                    result += _woodGrid[i, j] + "|";
                }
                result += "\n";
            }
            Debug.WriteLine(result);
        }
    }
}