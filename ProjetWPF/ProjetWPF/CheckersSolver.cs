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

        // Profondeur maximum que l'agent peut parcourir dans l'arbre de decision
        int m_depthMax;

        public CheckersSolver(MainWindow appDisplayer, Board board, int depthMax)
        {
            m_appDisplayer = appDisplayer;
            m_board = board;
            m_depthMax = depthMax;
        }

        #region Capteurs
        /// <summary>
        /// Applique les capteurs de l'agent et recupere l'etat actuelle de l'environnement
        /// </summary>
        CheckersState CaptureSignals()
        {
            // L'etat associe au dernier coup du joueur adverse
            CheckersState currentState = new CheckersState(m_board, Token.TokenColor.White);
            return currentState;
        }
        #endregion

        #region Effecteurs
        // Signature d'un effecteur
        public class Effector
        {
            TokenMoveSequence m_sequence;
            public Effector(TokenMoveSequence sequence)
            {
                m_sequence = sequence;
            }

            /// <summary>
            /// Execute l'action
            /// </summary>
            public void Execute(Board board)
            {
                while(m_sequence != null && !m_sequence.Empty())
                {
                    board.ExecuteTokenMove(m_sequence.TokenAttached, m_sequence.PlayMove());
                }
            }
        }
        #endregion

        /// <summary>
        /// Execute l'effecteur le plus approprie selon l'agent
        /// </summary>
        public void ExecuteAMove()
        {
            // Observe l’environnement
            CheckersState currentState = CaptureSignals();

            // Construit l'arbre ? (Uniquement si pb de performance)

            // Recupere toute les actions possible
            // Defini le but actuel prioritaire
            // Pour toute les actions possible on recupere celle qui est associe a la mesure d'utilite la plus forte
            Effector nextMove = MinimaxDecision(currentState);

            // Execute le mouvement
            nextMove.Execute(m_board);
        }

        /// <summary>
        /// Retourne la decision effectue par l'algorithme pour la meilleure action a prendre
        /// </summary>
        /// /// <param name="currentState">L'etat dans lequel l'environnement est actuellement</param>
        /// <returns>L'action la plus logique par rapport a l'etat donne</returns>
        Effector MinimaxDecision(CheckersState currentState) {

            CheckersState bestNextState = MaxValue(currentState, 0);
            // On retourne l'action dans les successeurs de l'etat qui retourne la valeur la plus importante
            return bestNextState.Action;
        }

        /// <summary>
        /// Retourne le meilleur etat successeur(pour l'agent) a l'etat propose
        /// </summary>
        /// <param name="state">L'etat dont on cherche les successeurs</param>
        /// <param name="depth">Profondeur de l'arbre a laquelle on se trouve</param>
        /// <returns>Le meilleur etats successeurs</returns>
        CheckersState MaxValue(CheckersState state, int depth)
        {
            // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (state.Terminal || depth >= m_depthMax) { return state; }

            int utility = int.MinValue;
            CheckersState bestSuccessor = null;
            // Pour tous les successeurs de l'etat on recupere le successeur qui renvoie la plus grande utilite minimum
            foreach (CheckersState nextState in state.Successors()) {
                int successorUtility = MinValue(nextState, depth + 1).Utility;
                if (utility < successorUtility) {
                    utility = successorUtility;
                    bestSuccessor = nextState;
                }
            }
            return bestSuccessor;
        }

        /// <summary>
        /// Retourne le pire etat successeurs(pour l'agent) a l'etat propose
        /// </summary>
        /// <param name="state">L'etat propose</param>
        /// <param name="depth">La profondeur dans l'arbre a laquelle on se trouve</param>
        /// <returns>Le pire etat successeurs</returns>
        CheckersState MinValue(CheckersState state, int depth)
        {   // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (state.Terminal || depth >= m_depthMax) { return state; }

            int utility = int.MaxValue;
            CheckersState bestSuccessor = null;
            // Pour tout les successeurs de l'etat on recupere le successeur qui renvoie la plus petite utilite maximum
            foreach (CheckersState nextState in state.Successors())
            {
                int successorUtility = MaxValue(nextState, depth + 1).Utility;
                if (utility > successorUtility)
                {
                    utility = successorUtility;
                    bestSuccessor = nextState;
                }
            }
            return bestSuccessor;
        }

    }
}
