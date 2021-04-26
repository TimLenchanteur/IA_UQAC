using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ProjetWPF
{
    class Board
    {
        Token[,] m_tokens = new Token[10, 10];
        public Token[,] Tokens { get { return m_tokens; } }

        // Tous les pions en jeu d'une certaine couleur
        List<Token> m_whiteTokens;
        List<Token> m_blackTokens;
        public int WhiteCount
        {
            get => m_whiteTokens.Count;
        }
        public int BlackCount {
            get => m_blackTokens.Count;
        }
        public List<Token> WhiteTokens
        {
            get => m_whiteTokens;
        }
        public List<Token> BlackTokens
        {
            get => m_blackTokens;
        }

        /// <summary>
        /// Copie un plateau de jeu dans la memoire interne de l'etat
        /// </summary>
        /// <param name="board">Le plateau a copier</param>
        public Board(Board copy)
        {
            m_blackTokens = new List<Token>();
            m_whiteTokens = new List<Token>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (copy.Tokens[i, j] != null)
                    {
                        Token newToken;
                        if (copy.Tokens[i, j] is Queen) newToken = new Queen(copy.Tokens[i, j] as Queen);
                        else newToken = new Token(copy.Tokens[i, j]);

                        if (newToken.Color == Token.TokenColor.White) m_whiteTokens.Add(newToken);
                        else if (newToken.Color == Token.TokenColor.Black) m_blackTokens.Add(newToken);
                        m_tokens[i, j] = newToken;
                    }
                    else m_tokens[i, j] = null;
                }
            }
        }

        // Initialize the board with black and white tokens
        public Board()
        {
            m_blackTokens = new List<Token>();
            m_whiteTokens = new List<Token>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0)
                    {
                        // Black
                        if(i <= 3)
                        {
                            Token token = new Token(Token.TokenColor.Black);
                            token.Position = new Vector2(j, i);
                            m_tokens[i, j] = token;
                            m_blackTokens.Add(token);
                        }
                        // White
                        else if (i >= 6)
                        {
                            Token token = new Token(Token.TokenColor.White);
                            token.Position = new Vector2(j, i);
                            m_tokens[i, j] = token;
                            m_whiteTokens.Add(token);
                        }
                        else
                        {
                            m_tokens[i, j] = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Effectue le mouvement decrit sur le tableau de jeu
        /// </summary>
        /// <param name="tokenToMove">Le pion a bouger</param>
        /// <param name="move">Le mouvement d'un pion</param>
        /// <returns>Le pion transformer en reine et ses sequences associes si transformation il y a eu</returns>
        public Queen ExecuteTokenMove(Token tokenToMove, TokenMove move) {

            m_tokens[tokenToMove.Position.Y, tokenToMove.Position.X] = null;
            Queen queen = null;

            // Si le pion arrive sur les bord, changer le pion en reine
            if (!(tokenToMove is Queen) && (move.Position.Y == 0 && tokenToMove.Color == Token.TokenColor.White) || (move.Position.Y == 9 && tokenToMove.Color == Token.TokenColor.Black))
            {
                queen = new Queen(tokenToMove.Color);
                queen.Position = move.Position;
                m_tokens[move.Position.Y, move.Position.X] = queen;
                if (tokenToMove.Color == Token.TokenColor.White)
                {
                    m_whiteTokens.Remove(tokenToMove);
                    m_whiteTokens.Add(queen);
                }
                else
                {
                    m_blackTokens.Remove(tokenToMove);
                    m_blackTokens.Add(queen);
                }
            }
            else
            {
                m_tokens[move.Position.Y, move.Position.X] = tokenToMove;
                tokenToMove.Position = move.Position;
            }

            // Si le mouvement est une capture de pion enlever tout les pions capturer
            if (move.Capture) {
                foreach (Token token in move.TokenAttached) {
                    m_tokens[token.Position.Y, token.Position.X] = null;
                    if (token.Color == Token.TokenColor.Black) m_blackTokens.Remove(token);
                    else m_whiteTokens.Remove(token);
                }
            }

            return queen;
        }

        /// <summary>
        /// Les pions prioritaires à bouger pour cette couleur
        /// </summary>
        /// <param name="color">La couleur du joueur</param>
        /// <returns>Une liste de tous les pions que le joueur doit bouger avec la plus haute priorites </returns>
        public Dictionary<Token, List<TokenMoveSequence>> PrioritaryTokens(Token.TokenColor color) {

            // Recuperer tous les pions de la couleur correspondante encore en jeu
            List<Token> tokens = new List<Token>();
            if (color == Token.TokenColor.White) tokens = m_whiteTokens;
            else tokens = m_blackTokens;

            // Pour chaque pions recuperer le nombre de pion qu'il peut capturer et garder uniquement ceux qui peuvent capturer le plus de pions
            int maxCapture = 0;
            Dictionary<Token, List<TokenMoveSequence>> prioritaryTokens = new Dictionary<Token, List<TokenMoveSequence>>();
            foreach (Token token in tokens) {
                List<TokenMoveSequence> captureSequence = BestMovesSequences(token);
                if (captureSequence.Count > 0) {
                    int canCapture = captureSequence[0].HowManyCaptured;
                    if (canCapture > maxCapture)
                    {
                        maxCapture = canCapture;
                        prioritaryTokens.Clear();
                        prioritaryTokens.Add(token, captureSequence);
                    }
                    else if (canCapture == maxCapture) prioritaryTokens.Add(token, captureSequence);
                }
            }

            return prioritaryTokens;
        }

        /// <summary>
        /// Meilleur sequences possible pour le prochain mouvement d'un pion
        /// </summary>
        /// <param name="token">Le pion attache a la sequence</param>
        /// <returns>Les meilleurs sequence de mouvement possible pour ce pions</returns>
        public List<TokenMoveSequence> BestMovesSequences(Token token)
        {
            if (token is Queen) return BestMovesSequences(token as Queen);

            List<TokenMoveSequence> possibleSequence = new List<TokenMoveSequence>();

            // Si capture possible on recupere les sequences permettant d'obtenir le plus de capture
            Vector2 downLeft = new Vector2(token.Position.X - 2, token.Position.Y + 2);
            Vector2 downRight = new Vector2(token.Position.X + 2, token.Position.Y + 2);
            Vector2 topLeft = new Vector2(token.Position.X - 2, token.Position.Y - 2);
            Vector2 topRight = new Vector2(token.Position.X + 2, token.Position.Y - 2);
            // Down left
            if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null &&
                   m_tokens[downLeft.Y - 1, downLeft.X + 1] != null && m_tokens[downLeft.Y - 1, downLeft.X + 1].Color != token.Color) {

                TokenMove newMove = new TokenMove(downLeft, m_tokens[downLeft.Y - 1, downLeft.X + 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(token);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, downLeft));
            }
            // Down right
            if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null &&
                m_tokens[downRight.Y - 1, downRight.X - 1] != null && m_tokens[downRight.Y - 1, downRight.X - 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(downRight, m_tokens[downRight.Y - 1, downRight.X - 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(token);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, downRight));
            }
            // Top left
            if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null &&
                m_tokens[topLeft.Y + 1, topLeft.X + 1] != null && m_tokens[topLeft.Y + 1, topLeft.X + 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(topLeft, m_tokens[topLeft.Y + 1, topLeft.X + 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(token);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, topLeft));
            }
            // Top right
            if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null &&
                m_tokens[topRight.Y + 1, topRight.X - 1] != null && m_tokens[topRight.Y + 1, topRight.X - 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(topRight, m_tokens[topRight.Y + 1, topRight.X - 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(token);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, topRight));
            }

            // Tri le nombre de capture par sequence et garde uniquement les sequence permettant le plus de capture
            List<TokenMoveSequence> toRemove = new List<TokenMoveSequence>();
            int maxCapture = 0;
            foreach (TokenMoveSequence sequence in possibleSequence) {
                if (maxCapture < sequence.HowManyCaptured) {
                    maxCapture = sequence.HowManyCaptured;
                }
            }
            if (maxCapture > 0)
            {
                foreach (TokenMoveSequence sequence in possibleSequence)
                {
                    if (maxCapture > sequence.HowManyCaptured) toRemove.Add(sequence);
                }
                foreach (TokenMoveSequence sequence in toRemove)
                {
                    possibleSequence.Remove(sequence);
                }

            }
            else {
                // Sinon on renvoie tout les mouvements possibles
                // Down left
                if (token.Color == Token.TokenColor.Black)
                {
                    downLeft = new Vector2(token.Position.X - 1, token.Position.Y + 1);
                    downRight = new Vector2(token.Position.X + 1, token.Position.Y + 1);
                    if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null)
                    {
                        TokenMove newMove = new TokenMove(downLeft);
                        TokenMoveSequence newSequence = new TokenMoveSequence(token);
                        newSequence.AddMove(newMove);
                        possibleSequence.Add(newSequence);
                    }
                    // Down right    
                    if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null)
                    {
                        TokenMove newMove = new TokenMove(downRight);
                        TokenMoveSequence newSequence = new TokenMoveSequence(token);
                        newSequence.AddMove(newMove);
                        possibleSequence.Add(newSequence);
                    }
                }
                else {
                    topLeft = new Vector2(token.Position.X - 1, token.Position.Y - 1);
                    topRight = new Vector2(token.Position.X + 1, token.Position.Y - 1);

                    // Top left
                    if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null)
                    {
                        TokenMove newMove = new TokenMove(topLeft);
                        TokenMoveSequence newSequence = new TokenMoveSequence(token);
                        newSequence.AddMove(newMove);
                        possibleSequence.Add(newSequence);
                    }
                    // Top right    
                    if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null)
                    {
                        TokenMove newMove = new TokenMove(topRight);
                        TokenMoveSequence newSequence = new TokenMoveSequence(token);
                        newSequence.AddMove(newMove);
                        possibleSequence.Add(newSequence);
                    }
                }
            }

            return possibleSequence;
        }

        /// <summary>
        /// Rempli la sequence de mouvement du pion jusqu'a ce qu'il n'y ai plus de capture possibles
        /// </summary>
        /// <param name="tokenSequence">Sequence de mouvement possible pour un pion</param>
        /// <param name="tokenPosition">Position du pion</param>
        /// <returns></returns>
        public List<TokenMoveSequence> TravelSequence(TokenMoveSequence tokenSequence, Vector2 tokenPosition) {

            List<TokenMoveSequence> possibleSequence = new List<TokenMoveSequence>();
            Token token = tokenSequence.TokenAttached;

            // Si capture possible on recupere les sequences permettant d'obtenir le plus de capture
            bool captured = false;
            Vector2 downLeft = new Vector2(tokenPosition.X - 2, tokenPosition.Y + 2);
            Vector2 downRight = new Vector2(tokenPosition.X + 2, tokenPosition.Y + 2);
            Vector2 topLeft = new Vector2(tokenPosition.X - 2, tokenPosition.Y - 2);
            Vector2 topRight = new Vector2(tokenPosition.X + 2, tokenPosition.Y - 2);
            // Down left
            if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null &&
                   m_tokens[downLeft.Y - 1, downLeft.X + 1] != null && !tokenSequence.AlreadyCaptured(new Vector2(downLeft.X + 1, downLeft.Y - 1)) && m_tokens[downLeft.Y - 1, downLeft.X + 1].Color != token.Color)
            {

                TokenMove newMove = new TokenMove(downLeft, m_tokens[downLeft.Y - 1, downLeft.X + 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(tokenSequence);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, downLeft));
                captured = true;
            }
            // Down right
            if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null &&
                m_tokens[downRight.Y - 1, downRight.X - 1] != null && !tokenSequence.AlreadyCaptured(new Vector2(downRight.X - 1, downRight.Y - 1)) && m_tokens[downRight.Y - 1, downRight.X - 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(downRight, m_tokens[downRight.Y - 1, downRight.X - 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(tokenSequence);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, downRight));
                captured = true;
            }
            // Top left
            if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null &&
                m_tokens[topLeft.Y + 1, topLeft.X + 1] != null && !tokenSequence.AlreadyCaptured(new Vector2(topLeft.X + 1, topLeft.Y + 1)) && m_tokens[topLeft.Y + 1, topLeft.X + 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(topLeft, m_tokens[topLeft.Y + 1, topLeft.X + 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(tokenSequence);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, topLeft));
                captured = true;
            }
            // Top right
            if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null &&
                m_tokens[topRight.Y + 1, topRight.X - 1] != null && !tokenSequence.AlreadyCaptured(new Vector2(topRight.X - 1, topRight.Y + 1)) && m_tokens[topRight.Y + 1, topRight.X - 1].Color != token.Color)
            {
                TokenMove newMove = new TokenMove(topRight, m_tokens[topRight.Y + 1, topRight.X - 1]);
                TokenMoveSequence newSequence = new TokenMoveSequence(tokenSequence);
                newSequence.AddMove(newMove);
                possibleSequence.AddRange(TravelSequence(newSequence, topRight));
                captured = true;
            }
            if (!captured) {
                possibleSequence.Add(tokenSequence);
            }
            return possibleSequence;
        }

        /// <summary>
        /// Meilleur sequences possible pour le prochain mouvement d'une reine
        /// </summary>
        /// <param name="token">La reine attachee a la sequence</param>
        /// <returns>Les meilleurs sequence de mouvement possible pour cette reine</returns>
        public List<TokenMoveSequence> BestMovesSequences(Queen queen)
        {
            List<TokenMoveSequence> possibleSequence = new List<TokenMoveSequence>();

            // Si capture possible on recupere les sequences permettant d'obtenir le plus de capture
            // Down left
            CheckCaptureInDiag(in possibleSequence, queen, -1, 1);
            // Down right
            CheckCaptureInDiag(in possibleSequence, queen, 1, 1);
            // Top left
            CheckCaptureInDiag(in possibleSequence, queen, -1, -1);
            // Top right
            CheckCaptureInDiag(in possibleSequence, queen, 1, -1);

            // Tri le nombre de capture par sequence et garde uniquement les sequence permettant le plus de capture
            List<TokenMoveSequence> toRemove = new List<TokenMoveSequence>();
            int maxCapture = 0;
            foreach (TokenMoveSequence sequence in possibleSequence)
            {
                if (maxCapture < sequence.HowManyCaptured)
                {
                    maxCapture = sequence.HowManyCaptured;
                }
            }
            if (maxCapture > 0)
            {
                foreach (TokenMoveSequence sequence in possibleSequence)
                {
                    if (maxCapture > sequence.HowManyCaptured) toRemove.Add(sequence);
                }
                foreach (TokenMoveSequence sequence in toRemove)
                {
                    possibleSequence.Remove(sequence);
                }

            }
            else
            {
                // Sinon on renvoie tout les mouvements possibles
                // Down left
                AddPosition(possibleSequence, queen, -1, 1);
                // Down right
                AddPosition(possibleSequence, queen, 1, 1);
                // Top left
                AddPosition(possibleSequence, queen, -1, -1);
                // Top right
                AddPosition(possibleSequence, queen, 1, -1);
            }
            return possibleSequence;
        }

        /// <summary>
        /// Ajoute toute les positions sur une diagonale
        /// </summary>
        /// <param name="possibleSequence">La liste a remplir qui ressence les sequence que la reine peut faire</param>
        /// <param name="queen">la reine associe a la capture</param>
        /// <param name="xOperation">L'operation a effectue sur les x (+ ou - un certains chiffre)</param>
        /// <param name="yOperation">L'operation a effectue sur les y (+ ou - un certains chiffre)</param>
        public void AddPosition(in List<TokenMoveSequence> possibleSequence,in Queen queen, int xOperation, int yOperation)
        {
            for (int i = queen.Position.X + xOperation, j = queen.Position.Y + yOperation; i >= 0 && i< 10 && j>= 0 && j < 10; i = i + xOperation, j = j + yOperation)
            {
                if (m_tokens[j, i] == null)
                {
                    TokenMove move = new TokenMove(new Vector2(i, j));
                    TokenMoveSequence sequence = new TokenMoveSequence(queen);
                    sequence.AddMove(move);
                    possibleSequence.Add(sequence);
                }
                else
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Verifie si une reine a la possibilite de capturer des pionsa partir d'une sequence
        /// </summary>
        /// <param name="possibleSequence">La liste a remplir qui ressence les sequence que la reine peut faire</param>
        /// <param name="queen">La reine associe a la capture</param>
        /// <param name="xOperation">L'operation a effectue sur les x (+ ou - un certains chiffre)</param>
        /// <param name="yOperation">L'operation a effectue sur les y (+ ou - un certains chiffre)</param>
        public void CheckCaptureInDiag(in List<TokenMoveSequence> possibleSequence, Queen queen, int xOperation, int yOperation) {
            Token captured = null;
            for (int i = queen.Position.X + xOperation, j = queen.Position.Y + yOperation; i >= 0 && i < 10 && j >= 0 && j < 10; i = i + xOperation, j = j + yOperation)
            {
                if (m_tokens[j, i] != null && m_tokens[j, i].Color != queen.Color && captured == null)
                {
                    captured = m_tokens[j, i];
                }
                else if (m_tokens[j, i] != null && (m_tokens[j, i].Color == queen.Color ||
                   (m_tokens[j, i].Color != queen.Color && captured != null))) break;
                else if (m_tokens[j, i] == null && captured != null)
                {
                    Vector2 capturePosition = new Vector2(i, j);
                    TokenMove move = new TokenMove(capturePosition, captured);
                    TokenMoveSequence newSequence = new TokenMoveSequence(queen);
                    newSequence.AddMove(move);
                    possibleSequence.AddRange(TravelQueenSequence(newSequence, capturePosition));
                }
            }
        }

        /// <summary>
        /// Parcours la sequence en cours pour la reine
        /// </summary>
        /// <param name="tokenSequence">La sequence en cours</param>
        /// <param name="tokenPosition">La position actuelle de la reine</param>
        /// <returns>Le sequence base sur la sequence en cours que la reine peut effectuer</returns>
        public List<TokenMoveSequence> TravelQueenSequence(TokenMoveSequence tokenSequence, Vector2 tokenPosition)
        {
            List<TokenMoveSequence> possibleSequence = new List<TokenMoveSequence>();
            Queen queen = tokenSequence.TokenAttached as Queen;

            // Si capture possible on recupere les sequences permettant d'obtenir le plus de capture
            bool captured = false;
            // Down left
            captured |= CheckCaptureInDiag(possibleSequence, queen, tokenSequence, tokenPosition.X, tokenPosition.Y, -1, 1);
            // Down right
            captured |= CheckCaptureInDiag(possibleSequence, queen, tokenSequence, tokenPosition.X, tokenPosition.Y, 1, 1);
            // Top left
            captured |= CheckCaptureInDiag(possibleSequence, queen, tokenSequence, tokenPosition.X, tokenPosition.Y, -1, -1);
            // Top right
            captured |= CheckCaptureInDiag(possibleSequence, queen, tokenSequence, tokenPosition.X, tokenPosition.Y, 1, -1);
            if (!captured) possibleSequence.Add(tokenSequence);
            return possibleSequence;
        }

        /// <summary>
        /// Verifie si une reine a la possibilite de capturer des pionsa partir d'une sequence
        /// </summary>
        /// <param name="possibleSequence">La liste a remplir qui ressence les sequence que la reine peut faire</param>
        /// <param name="queen">La reine associe a la capture</param>
        /// <param name="startSequence">La sequence a continue</param>
        /// <param name="currentX">La position x de la reine dans la sequence</param>
        /// <param name="currentY">La position y de la reine dans la sequence</param>
        /// <param name="xOperation">L'operation a effectue sur les x (+ ou - un certains chiffre)</param>
        /// <param name="yOperation">L'operation a effectue sur les y (+ ou - un certains chiffre)</param>
        /// <returns>Si la reine a reussi a cpaturer un pion</returns>
        public bool CheckCaptureInDiag(in List<TokenMoveSequence> possibleSequence, Queen queen, TokenMoveSequence startSequence, int currentX, int currentY, int xOperation, int yOperation)
        {
            Token captured = null;
            for (int i = currentX + xOperation, j = currentY + yOperation; i >= 0 && i < 10 && j >= 0 && j < 10; i = i + xOperation, j = j + yOperation)
            {
                if (m_tokens[j, i] != null && m_tokens[j, i].Color != queen.Color && !startSequence.AlreadyCaptured(new Vector2(i, j)) && captured == null)
                {
                    captured = m_tokens[j, i];
                }
                else if (m_tokens[j, i] != null && (m_tokens[j, i].Color == queen.Color ||
                   (m_tokens[j, i].Color != queen.Color && captured != null))) break;
                else if (m_tokens[j, i] == null && !startSequence.AlreadyCaptured(new Vector2(i, j)) && captured!=null)
                {
                    Vector2 capturePosition = new Vector2(i, j);
                    TokenMove move = new TokenMove(capturePosition, captured);
                    TokenMoveSequence newSequence = new TokenMoveSequence(startSequence);
                    newSequence.AddMove(move);
                    possibleSequence.AddRange(TravelQueenSequence(newSequence, capturePosition));
                }
            }
            return captured != null;
        }
    }

    /// <summary>
    /// Une sequence de mouvement possible effectuer par un pion
    /// </summary>
    class TokenMoveSequence {

        // Combien de pions seront captures apres la sequence
        int m_howManyCaptured;
        public int HowManyCaptured {
            get => m_howManyCaptured;
        }
        public bool IsCaptureSequence {
            get => m_howManyCaptured != 0;
        }

        // Le pion qui effectuera la sequence
        Token m_tokenAttached;
        public Token TokenAttached {
            get => m_tokenAttached;
            set => m_tokenAttached = value;
        }

        // La position initiale du pion avant de démarrer la séquence
        Vector2 m_origin;
        public Vector2 OriginPosition
        {
            get => m_origin;
        }

        // La sequence de mouvement
        List<TokenMove> m_moveQueue;

        public TokenMoveSequence(Token token) {
            m_tokenAttached = token;
            m_moveQueue = new List<TokenMove>();
            m_howManyCaptured = 0;
            m_origin = token.Position;
        }

        public TokenMoveSequence(TokenMoveSequence tokenSequence)
        {
            m_tokenAttached = tokenSequence.TokenAttached;
            m_moveQueue = new List<TokenMove>(tokenSequence.m_moveQueue);
            m_howManyCaptured = tokenSequence.m_howManyCaptured;
            m_origin = tokenSequence.OriginPosition;
        }

        // Ajoute un mouvement a la sequence
        public bool AddMove(TokenMove move) {
            if (m_moveQueue.Count == 0) {
                if (move.Capture) m_howManyCaptured = move.TokenAttached.Count;
            }
            else if (IsCaptureSequence) {
                m_howManyCaptured += move.TokenAttached.Count;
            }
            m_moveQueue.Add(move);
            return true;
        }

        // Regarde le prochain mouvement possible
        public TokenMove PeekMove() {
            if (m_moveQueue.Count == 0) return null;
            return m_moveQueue[0];
        }

        // Joue le prochain mouvement de la sequence
        public TokenMove PlayMove() {
            TokenMove move = m_moveQueue[0];
            m_moveQueue.RemoveAt(0);
            return move;
        }

        public bool Empty() {
            return m_moveQueue.Count == 0;
        }

        public bool AlreadyCaptured(Vector2 position) {
            if (!IsCaptureSequence) return false;
            FindToken findPredicate = new FindToken(position);
            foreach (TokenMove move in m_moveQueue) {
                if (move.Capture && move.TokenAttached.Find(findPredicate.Match) != null) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Classe utilise comme predicat pour retrouver un pion dans une liste
    /// </summary>
    class FindToken {
        Vector2 m_position;

        public FindToken(Vector2 position) {
            m_position = position;
        }

        public bool Match(Token token) {
            return token.Position.Equals(m_position);
        }
    }


    /// <summary>
    /// Movement possible pour un pion
    /// </summary>
    class TokenMove {

        // Position a laquelle se trouvera apres le mouvement
        Vector2 m_position;
        public Vector2 Position{
            get => m_position;
        }

        // Pion qui auront ete capturer apres le mouvement
        List<Token> m_tokenAttached;
        public List<Token> TokenAttached {
            get => m_tokenAttached;
        }
        // Le mouvement permet t'il la capture d'un ou plusieurs pions ?
        public bool Capture { 
            get => m_tokenAttached != null;
        }

        public TokenMove(Vector2 position, Token tokenAttached)
        {
            m_position = position;
            m_tokenAttached = new List<Token>();
            m_tokenAttached.Add(tokenAttached);
        }

        public TokenMove(Vector2 position, List<Token> tokenAttached = null) {
            m_position = position;
            m_tokenAttached = tokenAttached;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TokenMove)) return false;
            TokenMove move = obj as TokenMove;
            return m_position.Equals(move.m_position);
        }
    }
}
