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

        public void Initialize()
        {
            for(int i = 0; i < 10; i++)
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
                        }
                        // White
                        else if (i >= 6)
                        {
                            Token token = new Token(Token.TokenColor.White);
                            token.Position = new Vector2(j, i);
                            m_tokens[i, j] = token;
                        }
                        else
                        {
                            m_tokens[i, j] = null;
                        }
                    }
                }
            }
        }

        public List<Vector2> PossibleMoves(Token token)
        {
            List<Vector2> moves = new List<Vector2>();

            Vector2 downLeft;
            Vector2 downRight;
            Vector2 topLeft;
            Vector2 topRight;

            // Moves
            if (token.Color == Token.TokenColor.Black)
            {
                // Down left
                downLeft = new Vector2(token.Position.X - 1, token.Position.Y + 1);
                if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null)
                {
                    moves.Add(downLeft);
                }
                // Down right
                downRight = new Vector2(token.Position.X + 1, token.Position.Y + 1);
                if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null)
                {
                    moves.Add(downRight);
                }
            }
            else
            {
                // Top left
                topLeft = new Vector2(token.Position.X - 1, token.Position.Y - 1);
                if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null)
                {
                    moves.Add(topLeft);
                }
                // Top right
                topRight = new Vector2(token.Position.X + 1, token.Position.Y - 1);
                if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null)
                {
                    moves.Add(topRight);
                }
            }

            return moves;
        }

        public List<Tuple<Vector2, Token>> PossibleCaptures(Token token)
        {
            List<Tuple<Vector2, Token>> captures = new List<Tuple<Vector2, Token>>();
            Vector2 downLeft;
            Vector2 downRight;
            Vector2 topLeft;
            Vector2 topRight;

            // Down left
            downLeft = new Vector2(token.Position.X - 2, token.Position.Y + 2);
            if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null &&
                m_tokens[downLeft.Y - 1, downLeft.X + 1] != null && m_tokens[downLeft.Y - 1, downLeft.X + 1].Color != token.Color)
            {
                captures.Add(Tuple.Create(downLeft, m_tokens[downLeft.Y - 1, downLeft.X + 1]));
            }
            // Down right
            downRight = new Vector2(token.Position.X + 2, token.Position.Y + 2);
            if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null &&
                m_tokens[downRight.Y - 1, downRight.X - 1] != null && m_tokens[downRight.Y - 1, downRight.X - 1].Color != token.Color)
            {
                captures.Add(Tuple.Create(downRight, m_tokens[downRight.Y - 1, downRight.X - 1]));
            }
            // Top left
            topLeft = new Vector2(token.Position.X - 2, token.Position.Y - 2);
            if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null &&
                m_tokens[topLeft.Y + 1, topLeft.X + 1] != null && m_tokens[topLeft.Y + 1, topLeft.X + 1].Color != token.Color)
            {
                captures.Add(Tuple.Create(topLeft, m_tokens[topLeft.Y + 1, topLeft.X + 1]));
            }
            // Top right
            topRight = new Vector2(token.Position.X + 2, token.Position.Y - 2);
            if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null &&
                m_tokens[topRight.Y + 1, topRight.X - 1] != null && m_tokens[topRight.Y + 1, topRight.X - 1].Color != token.Color)
            {
                captures.Add(Tuple.Create(topRight, m_tokens[topRight.Y + 1, topRight.X - 1]));
            }

            return captures;
        }

        public void MoveToken(Token token, Vector2 destination)
        {
            m_tokens[token.Position.Y, token.Position.X] = null;
            m_tokens[destination.Y, destination.X] = token;
            token.Position = destination;
        }

        public void RemoveToken(Token token)
        {
            m_tokens[token.Position.Y, token.Position.X] = null;
        }
    }
}
