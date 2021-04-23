using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    class CheckersState
    {

        // Plateau de jeux dans l'etat actuel
        Token[,] m_board = new Token[10, 10];
        public Token[,] Board { get { return m_board; } }

        // Le joueur qui a joué pour atteindre cet etat
        Token.TokenColor m_playerColor;

        // Pions restants sur le plateau pour chaque joueur
        List<Token> m_whiteTokens;
        List<Token> m_blackTokens;

        // L'etat est il associe a une fin du jeu ?
        public bool Terminal{
            get => m_whiteTokens.Count == 0 || m_blackTokens.Count == 0;
        }

        // L'utilite associe a cet etat pour l'agent
        int m_utility;
        public int Utility {
            get => m_utility;
        }

        // Action qui a permis d'atteindre cet etat
        CheckersSolver.Effector m_action;
        public CheckersSolver.Effector Action { 
            get =>  m_action;
        }

        /// <summary>
        /// Constructeur pour la racine de l'arbre d'etat
        /// </summary>
        /// <param name="board">Le plateau de jeu apres le dernier coup</param>
        /// <param name="playerColor">Le joueur qui a joue le dernier coup</param>
        public CheckersState(Board board, Token.TokenColor playerColor) {

            // Pas d'action car ce constructeur ne construit que les racines de l'arbre
            m_action = null;
            m_playerColor = playerColor;
            CopyBoard(board.Tokens);
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
            CopyBoard(parent.m_board);
            // On effectue l'action 
            ExecuteAction();
            m_utility = ComputeUtility();
        }

        /// <summary>
        /// Copie un plateau de jeu dans la memoire interne de l'etat
        /// </summary>
        /// <param name="board">Le plateau a copier</param>
        void CopyBoard(Token[,] board) {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (board[i, j] != null)
                    {
                        Token newToken = null;
                        if (board[i, j] is Queen) newToken = new Queen(board[i, j] as Queen);
                        else newToken = new Token(board[i, j]);

                        if (newToken.Color == Token.TokenColor.White) m_whiteTokens.Add(newToken);
                        else if (newToken.Color == Token.TokenColor.Black) m_blackTokens.Add(newToken);
                        m_board[i, j] = newToken;
                    }
                    else m_board[i, j] = null;
                }
            }
        }

        /// <summary>
        /// Execute une action dans l'etat
        /// </summary>
        void ExecuteAction() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Les successeurs d'un etat
        /// </summary>
        /// <returns>Tous les successeurs d'un etat</returns>
        public List<CheckersState> Successors() {
            // Cherche tous les mouvements possibles pour le joueur actuel

            // Creer chaque etat pour chaque mouvement possible du joueur

            throw new NotImplementedException();
        }

        /// <summary>
        /// Calcul l'utilite a associe a l'etat
        /// </summary>
        /// <returns>L'utilite associe a l'etat</returns>
        int ComputeUtility() {
            // Heuristique de base
            if(m_playerColor == Token.TokenColor.Black)
            {
                return m_blackTokens.Count - m_whiteTokens.Count;
            }
            else
            {
                return m_whiteTokens.Count - m_blackTokens.Count;
            }
        }
    }
}
