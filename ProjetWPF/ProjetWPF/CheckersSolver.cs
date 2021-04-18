using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    /// <summary>
    /// Agent base sur l'utilite
    /// </summary>
    class CheckersSolver
    {
        // Environement dans lequel evolue l'agent
        Board m_board;

        // Classe permetant d'afficher l'environement dans l'application
        MainWindow m_appDisplayer;


        public CheckersSolver(MainWindow appDisplayer, Board board)
        {
            m_appDisplayer = appDisplayer;
            m_board = board;
        }

        #region Capteurs
        /// <summary>
        /// Applique les capteurs de l'agent et recupere l'etat actuelle de l'environnement
        /// </summary>
        int[,] CaptureSignals()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Effecteurs
        // Signature d'un effecteur
        abstract class Effector
        {


            public Effector(Vector2 position)
            { }

            /// <summary>
            /// Execute l'action
            /// </summary>
            public abstract void Execute();
        }
        #endregion

        /// <summary>
        /// Execute l'effecteur le plus approprie selon l'agent
        /// </summary>
        public void ExecuteAMove()
        {
            // Observe l’environnement
            int[,] currentState = CaptureSignals();

            // Construit l'arbre ?


            // Recupere toute les actions possible
            // Defini le but actuel prioritaire

            // Pour toute les actions possible on recupere celle qui est associe a la mesure d'utilite la plus forte

            // Affiche le nouvel etat
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retourne la decision effectue par l'algorithme pour la meilleure action a prendre
        /// </summary>
        /// <returns>L'action la plus logique par rapport a l'etat donne</returns>
        Effector MinimaxDecision() {

            int utility = MaxValue(/*Etat*/);
            // On retourne l'action dans les successeurs de l'etat qui retourne la valeur la plus importante
            throw new NotImplementedException();

        }

        int MaxValue(/*Etat*/)
        {
            // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (false /*etat terminal*/) { /*retourne utilite associe a l'etat*/}
            int utility = int.MinValue;

            // Pour tout les successeurs de l'etat on recupere le successeur qui renvoie la plus grande utilite minimum
            //for (Successeurs) { 
                utility = Math.Max(utility, MinValue(/*Etat*/));
            //}
            throw new NotImplementedException();
            return utility;
        }

        int MinValue(/*Etat*/)
        {   // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (false /*etat terminal*/) { /*retourne utilite associe a l'etat*/}
            int utility = int.MaxValue;
            // Pour tout les successeurs de l'etat on recupere le successeur qui renvoie la plus petite utilite maximum
            //for (Successeurs) { 
                 utility = Math.Min(utility, MaxValue(/*Etat*/));
            //}

            throw new NotImplementedException();
            return utility;
        }

    }
}
