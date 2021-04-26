﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    class CheckersState
    {

        // Plateau de jeux dans l'etat actuel
        Board m_board;
        public Board Board { get { return m_board; } }

        // Le joueur qui a joué pour atteindre cet etat
        Token.TokenColor m_playerColor;

        // L'etat est il associe a une fin du jeu ?
        public bool Terminal{
            get => m_board.WhiteCount == 0 || m_board.BlackCount == 0;
        }

        // L'utilite associe a cet etat pour l'agent
        int m_utility;
        public int Utility {
            get => m_utility;
        }

        // Action qui a permis d'atteindre cet etat
        CheckersSolver.Effector m_action;
        public CheckersSolver.Effector Action { 
            get => m_action;
        }

        /// <summary>
        /// Constructeur pour la racine de l'arbre d'etat
        /// </summary>
        /// <param name="board">Le plateau de jeu apres le dernier coup</param>
        /// <param name="playerColor">Le joueur qui a joue le dernier coup</param>
        public CheckersState(Board board, Token.TokenColor playerColor)
        {
            // Pas d'action car ce constructeur ne construit que les racines de l'arbre
            m_action = null;
            m_playerColor = playerColor;
            m_board = new Board(board);
            m_utility = ComputeUtility();
        }

        /// <summary>
        /// Constructeur interne utilise pour creer les successeurs d'un etat
        /// </summary>
        /// <param name="parent">L'etat precedent cet etat</param>
        /// <param name="action">L'action a execute sur cet etat</param>
        CheckersState(CheckersState parent, CheckersSolver.Effector action)
        {
            m_action = action;
            // On change la couleur du joueur
            if(parent.m_playerColor == Token.TokenColor.Black) m_playerColor = Token.TokenColor.White;
            else if (parent.m_playerColor == Token.TokenColor.White) m_playerColor = Token.TokenColor.Black;
            m_board = new Board(parent.Board);
            // On effectue l'action 
            ExecuteAction();
            m_utility = ComputeUtility();
        }

        /// <summary>
        /// Execute une action dans l'etat
        /// </summary>
        void ExecuteAction()
        {
            m_action.MockExecute(m_board);
        }

        /// <summary>
        /// Les successeurs d'un etat
        /// </summary>
        /// <returns>Tous les successeurs d'un etat</returns>
        public List<CheckersState> Successors() {
            List<CheckersState> nextStates = new List<CheckersState>();

            // Cherche tous les mouvements possibles pour le joueur actuel
            var tokenMoves = m_board.PrioritaryTokens(m_playerColor);

            // Creer chaque etat pour chaque mouvement possible du joueur
            foreach(var sequences in tokenMoves.Values)
            {
                foreach(var sequence in sequences)
                {
                    nextStates.Add(new CheckersState(this, new CheckersSolver.Effector(sequence)));
                }
            }

            return nextStates;
        }

        /// <summary>
        /// Calcul l'utilite a associe a l'etat
        /// </summary>
        /// <returns>L'utilite associe a l'etat</returns>
        int ComputeUtility() {
            // Heuristique de base
            int blackPawns = 0;
            int blackQueens = 0;
            float avgAdvance = 0;
            int whitePawns = 0;
            int whiteQueens = 0;
            foreach(Token t in m_board.BlackTokens)
            {
                if(t is Queen)
                {
                    blackQueens += 1;
                }
                else
                {
                    blackPawns += 1;
                }
                avgAdvance += t.Position.Y;
            }
            foreach (Token t in m_board.WhiteTokens)
            {
                if (t is Queen)
                {
                    whiteQueens += 1;
                }
                else
                {
                    whitePawns += 1;
                }
                avgAdvance += t.Position.Y;
            }
            avgAdvance /= -(m_board.BlackCount + m_board.WhiteCount);
            avgAdvance += 4.5f;
            if (m_playerColor == Token.TokenColor.Black)
            {
                if(m_board.BlackCount == 0)
                {
                    return int.MinValue;
                }
                else if (m_board.WhiteCount == 0)
                {
                    return int.MaxValue;
                }
                return (int)(10 * (blackPawns - whitePawns) + 100 * (blackQueens - whiteQueens) + avgAdvance);
            }
            else
            {
                if (m_board.BlackCount == 0)
                {
                    return int.MaxValue;
                }
                else if (m_board.WhiteCount == 0)
                {
                    return int.MinValue;
                }
                return -(int)(10 * (blackPawns - whitePawns) + 100 * (blackQueens - whiteQueens) + avgAdvance);
            }
        }

        /*int ComputeUtility()
        {
            int 
            // Heuristique de base
            if (m_playerColor == Token.TokenColor.Black)
            {
                return m_board.BlackCount - m_board.WhiteCount;
            }
            else
            {
                return m_board.WhiteCount - m_board.BlackCount;
            }
        }*/
    }
}
