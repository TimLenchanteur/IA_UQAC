using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

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
        static MainWindow m_appDisplayer;

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
            CheckersState currentState = new CheckersState(m_board, Token.TokenColor.Black);
            return currentState;
        }
        #endregion

        #region Effecteurs
        // Signature d'un effecteur
        public class Effector
        {
            TokenMoveSequence m_sequence;

            // Sequence de mouvement associe a une reine creer pendant le mouvement dans m_sequence
            TokenMoveSequence m_newQueenSequence;

            public Effector(TokenMoveSequence sequence)
            {
                m_sequence = sequence;
                m_newQueenSequence = null;
            }

            public void AddNewQueenSequence(TokenMoveSequence newQueenSequence) {
                m_newQueenSequence = newQueenSequence;
            }

            /// <summary>
            /// Execute l'action
            /// </summary>
            public void Execute(Board board)
            {
                // On veut garder intacte la séquence initiale
                TokenMoveSequence copySequence = new TokenMoveSequence(m_sequence);
                copySequence.TokenAttached = board.Tokens[copySequence.OriginPosition.X, copySequence.OriginPosition.Y];
                while(copySequence != null && !copySequence.Empty())
                {
                    Thread.Sleep(250);
                    TokenMove move = copySequence.PlayMove();
                    board.ExecuteTokenMove(copySequence.TokenAttached, move, copySequence.Empty());
                    m_appDisplayer.DisplayBoardFromThread();
                }
            }

            /// <summary>
            /// Execute l'action pour tester ses consequences
            /// </summary>
            public void MockExecute(Board board)
            {
                // On veut garder intacte la séquence initiale
                TokenMoveSequence copySequence = new TokenMoveSequence(m_sequence);
                copySequence.TokenAttached = board.Tokens[copySequence.OriginPosition.X, copySequence.OriginPosition.Y];
                while (copySequence != null && !copySequence.Empty())
                {
                    TokenMove move = copySequence.PlayMove();
                    board.ExecuteTokenMove(copySequence.TokenAttached, move, copySequence.Empty());
                }
            }
        }
        #endregion

        /// <summary>
        /// Execute l'effecteur le plus approprie selon l'agent
        /// </summary>
        /// <returns>Faux si l'agent n'a plus de mouvement</returns>
        public bool ExecuteAMove()
        {
            // Observe l’environnement
            CheckersState currentState = CaptureSignals();

            // Construit l'arbre ? (Uniquement si pb de performance)

            // Recupere toute les actions possible
            // Defini le but actuel prioritaire
            // Pour toute les actions possible on recupere celle qui est associe a la mesure d'utilite la plus forte
            Effector nextMove = AlphaBetaSearch(currentState);//MinimaxDecision(currentState);

            // Execute le mouvement
            if (nextMove == null) return false;
            nextMove.Execute(m_board);
            return true;
        }

        /// <summary>
        /// Retourne la decision effectue par l'algorithme pour la meilleure action a prendre
        /// </summary>
        /// /// <param name="currentState">L'etat dans lequel l'environnement est actuellement</param>
        /// <returns>L'action la plus logique par rapport a l'etat donne</returns>
        Effector MinimaxDecision(CheckersState currentState)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            CheckersState bestNextState = MaxValue(currentState, 0);
            stopwatch.Stop();
            Debug.WriteLine("Elapsed Time is {0} s", stopwatch.ElapsedMilliseconds / 1000f);
            // On retourne l'action dans les successeurs de l'etat qui retourne la valeur la plus importante
            return bestNextState.Action;
        }

        Effector AlphaBetaSearch(CheckersState currentState)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            CheckersState bestNextState = MaxValue(currentState, int.MinValue, int.MaxValue, 0);
            stopwatch.Stop();
            Debug.WriteLine("Elapsed Time is {0} s", stopwatch.ElapsedMilliseconds/1000f);
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
            CheckersState bestSuccessor = state;
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

        CheckersState MaxValue(CheckersState state, int alpha, int beta, int depth)
        {
            // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (state.Terminal || depth >= m_depthMax) { return state; }

            int utility = int.MinValue;
            CheckersState bestSuccessor = state;
            // Pour tous les successeurs de l'etat on recupere le successeur qui renvoie la plus grande utilite minimum
            foreach (CheckersState nextState in state.Successors())
            {
                int successorUtility = MinValue(nextState, alpha, beta, depth + 1).Utility;
                if (utility < successorUtility)
                {
                    utility = successorUtility;
                    bestSuccessor = nextState;
                }
                if (utility > beta)
                {
                    return bestSuccessor;
                }
                alpha = Math.Max(alpha, utility);
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
            CheckersState bestSuccessor = state;
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

        CheckersState MinValue(CheckersState state, int alpha, int beta, int depth)
        {   // Si l'etat est un etat terminal de l'arbre on retourne une valeur
            if (state.Terminal || depth >= m_depthMax) { return state; }

            int utility = int.MaxValue;
            CheckersState bestSuccessor = state;
            // Pour tout les successeurs de l'etat on recupere le successeur qui renvoie la plus petite utilite maximum
            foreach (CheckersState nextState in state.Successors())
            {
                int successorUtility = MaxValue(nextState, depth + 1).Utility;
                if (utility > successorUtility)
                {
                    utility = successorUtility;
                    bestSuccessor = nextState;
                }
                if (utility > alpha)
                {
                    return bestSuccessor;
                }
                beta = Math.Min(utility, beta);
            }
            return bestSuccessor;
        }

    }
}
