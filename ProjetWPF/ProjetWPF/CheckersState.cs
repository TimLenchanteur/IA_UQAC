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
        /// <param name="board">Le plateau de jeu apres le dernier coup</param>
        /// <param name="playerColor">Le joueur qui a joue le dernier coup</param>
        /// <param name="action">Le dernier coup jouer</param>
        CheckersState(Board board, Token.TokenColor playerColor, CheckersSolver.Effector action)
        {
            m_action = action;
            m_board = board;
            // On change la couleur du joueur
            if (playerColor == Token.TokenColor.Black) m_playerColor = Token.TokenColor.White;
            else if (playerColor == Token.TokenColor.White) m_playerColor = Token.TokenColor.Black;
            m_utility = ComputeUtility();
        }

        /// <summary>
        /// Execute une action dans l'etat
        /// </summary>
        /// <param name="sequence">La sequence de mouvement a execute</param>
       /// <returns>Tous les tableau qui peuvent resulter de cette etat (plusieurs car transformation en reine)</returns>
        List<KeyValuePair<Board, CheckersSolver.Effector>> ExecuteAction(TokenMoveSequence sequence)
        {
            List<KeyValuePair<Board, CheckersSolver.Effector>> boards = new List<KeyValuePair<Board, CheckersSolver.Effector>>();

            Board originalBoard = new Board(m_board);
            CheckersSolver.Effector action = new CheckersSolver.Effector(sequence);
            Queen newQueen = action.MockExecute(originalBoard);
            // Si jamais le pion est devenue une reine pendant le test on doit tester toute les nouveaux mouvements possible avec la reine
            if (newQueen != null) {
                List<TokenMoveSequence> newQueenSequence = originalBoard.BestMovesSequences(newQueen);
                if (newQueenSequence.Count > 0 && newQueenSequence[0].IsCaptureSequence){
                    foreach (TokenMoveSequence queenSequence in newQueenSequence) {
                        CheckersSolver.Effector newAction = new CheckersSolver.Effector(sequence);
                        newAction.AddNewQueenSequence(queenSequence);
                        Board sonBoard = new Board(originalBoard);
                        newAction.NewQueenMockExecute(sonBoard);
                        boards.Add(new KeyValuePair<Board, CheckersSolver.Effector>(sonBoard, newAction));
                    }
                    return boards;
                }
            }
            boards.Add(new KeyValuePair<Board, CheckersSolver.Effector>(originalBoard, action));

            return boards;
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
                    foreach (KeyValuePair<Board, CheckersSolver.Effector> state in ExecuteAction(sequence)) {
                        nextStates.Add(new CheckersState(state.Key, m_playerColor, state.Value));
                    }
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
            if(m_playerColor == Token.TokenColor.Black)
            {
                return m_board.BlackCount - m_board.WhiteCount;
            }
            else
            {
                return m_board.WhiteCount - m_board.BlackCount;
            }
        }
    }
}
