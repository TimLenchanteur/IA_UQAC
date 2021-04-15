using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    /// <summary>
    /// Agent base sur les buts
    /// Son but est de battre le joueur de dame adverse
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
        void CaptureSignals()
        {
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
            CaptureSignals();

            // Tant que l'agent peut effectuer un mouvement il le fait
            while (false) { }

            // Affiche le nouvel etat
        }

    }
}
